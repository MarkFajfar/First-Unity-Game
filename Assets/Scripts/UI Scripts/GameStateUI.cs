using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using static NavajoWars.GameStateFunction;

namespace NavajoWars
{
    public enum GameStateFunction 
    { 
        AP, CP, MP, Morale, 
        EnemyAP, EnemyFerocity, 
        TradeGoodsHeld, SheepHeld, HorsesHeld, Firearms, 
        HasDrought, HasCorn, HasMission, HasRancho, HasFort,
        IsActive, Man, Woman, Child, Horse, Ferocity, IsWhere
    }

    // TODO: add Elder List - foldout of sliders??
    // TODO: for Family - add Ferocity (dropdown)
    // TODO: if Family not active, show only toggle to make active, then populate and show other toggles

    public class GameStateUI : MonoBehaviour
    {
        protected GameManager gm;
        protected GameState gs;

        delegate void DelegateInt(int v);
        delegate void DelegateTerritoryBool(Territory t, bool b);
        delegate void DelegateFamilyBool(Family f, bool b);
        delegate void DelegateFamilyInt(Family f, int v);
        delegate void DelegateFamilyBoolObj(Family f, bool b, gsFuncObj obj);
        
        VisualElement statusPanel;
        Button status;

        string FoldoutClassName = Foldout.toggleUssClassName;

        class gsFuncObj
        {
            public GameStateFunction gsFunc;
            public Delegate d;
            public VisualElement ve;
            public int? v = null;
            public bool? b = null;
            public Family f = null;
            public Territory t = Territory.Default;
            public List<Territory> listT = null;
            public VisualElement p = null;

            public gsFuncObj() { }

            public static explicit operator DelegateInt(gsFuncObj obj) => obj.d as DelegateInt;
            //{ return obj.d as DelegateInt; }
        }

        List<gsFuncObj> gsFuncObjects = new();
        List<gsFuncObj> familyActiveCheck = new();

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
        }

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            statusPanel = root.Q<VisualElement>("StatusPanel");
            status = root.Q<Button>("Status");
            status.clicked += Refresh;

            initializeFunctions();
        }

        void initializeFunctions()
        {
            gsFuncObjects.Add(new()
            {
                gsFunc = AP,
                d = new DelegateInt((int v) => gs.AP = v),
                v = gs.AP,
                ve = statusPanel.Query(className: AP.ToString())
            } );
            gsFuncObjects.Add(new()
            {
                gsFunc = CP,
                d = new DelegateInt((int v) => gs.CP = v),
                v = gs.CP,
                ve = statusPanel.Query(className: CP.ToString())
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = MP,
                d = new DelegateInt((int v) => gs.MP = v),
                v = gs.MP,
                ve = statusPanel.Query(className: MP.ToString())
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = Morale,
                d = new DelegateInt((int v) => gs.Morale = v),
                v = gs.Morale,
                ve = statusPanel.Query(className: Morale.ToString())
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = EnemyFerocity,
                d = new DelegateInt((int v) => gs.EnemyFerocity = v),
                v = gs.EnemyFerocity,
                ve = statusPanel.Query(className: EnemyFerocity.ToString())
            });

            foreach (Family family in gs.AllFamilies)
            {
                Foldout foldout = (Foldout)statusPanel.Query(className: family.Name.Replace(" ", ""));
                gsFuncObjects.Add(new()
                {
                    gsFunc = IsActive,
                    d = new DelegateFamilyBoolObj((Family f, bool b, gsFuncObj o) => { 
                        f.IsActive = b;
                        if (b == false) 
                        {
                            hideChildren(o.p);
                            o.ve.style.display = DisplayStyle.Flex;
                        }
                        else 
                        { 
                            showChildren(o.p);
                        }
                        }),
                    b = family.IsActive,
                    f = family,
                    ve = statusPanel.Query(className: IsActive.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Man,
                    d = new DelegateFamilyBool((Family f, bool b) => f.HasMan = b),
                    b = family.HasMan,
                    f = family,
                    ve = statusPanel.Query(className: Man.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Woman,
                    d = new DelegateFamilyBool((Family f, bool b) => f.HasWoman = b),
                    b = family.HasWoman,
                    f = family,
                    ve = statusPanel.Query(className: Woman.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Child,
                    d = new DelegateFamilyBool((Family f, bool b) => f.HasChild = b),
                    b = family.HasChild,
                    f = family,
                    ve = statusPanel.Query(className: Child.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Horse,
                    d = new DelegateFamilyBool((Family f, bool b) => f.HasHorse = b),
                    b = family.HasHorse,
                    f = family,
                    ve = statusPanel.Query(className: Horse.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = IsWhere,
                    d = new DelegateFamilyInt((Family f, int v) => f.IsWhere = (Territory)v + 1),
                    t = family.IsWhere,
                    f = family,
                    ve = statusPanel.Query(className: IsWhere.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
            }

            for (int i = 1; i < (int)Territory.Default; i++)
            {
                Territory terr = (Territory)i;
                Foldout foldout = (Foldout)statusPanel.Query(className: terr.ToString());

                gsFuncObjects.Add(new()
                {
                    gsFunc = HasDrought,
                    d = new DelegateTerritoryBool((Territory t, bool b) =>
                        toggleTerritoryValue(gs.HasDrought, t, b)),
                    t = terr,
                    listT = gs.HasDrought,
                    ve = statusPanel.Query(className: HasDrought.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasCorn,
                    d = new DelegateTerritoryBool((Territory t, bool b) =>
                    { if (b) gs.HasCorn.Add(t); else gs.HasCorn.Remove(t); }),
                    t = terr,
                    listT = gs.HasCorn,
                    ve = statusPanel.Query(className: HasCorn.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasMission,
                    d = new DelegateTerritoryBool((Territory t, bool b) =>
                    { if (b) gs.HasMission.Add(t); else gs.HasMission.Remove(t); }),
                    t = terr,
                    listT = gs.HasMission,
                    ve = statusPanel.Query(className: HasMission.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasRancho,
                    d = new DelegateTerritoryBool((Territory t, bool b) =>
                    { if (b) gs.HasRancho.Add(t); else gs.HasRancho.Remove(t); }),
                    t = terr,
                    listT = gs.HasRancho,
                    ve = statusPanel.Query(className: HasRancho.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasFort,
                    d = new DelegateTerritoryBool((Territory t, bool b) =>
                    { if (b) gs.HasFort.Add(t); else gs.HasFort.Remove(t); }),
                    t = terr,
                    listT = gs.HasFort,
                    ve = statusPanel.Query(className: HasFort.ToString()).Where(e => e.parent == foldout),
                    p = foldout
                });
            }
            void toggleTerritoryValue(List<Territory> list, Territory t, bool b)
            {
                if (b) list.Add(t);
                else list.Remove(t);
            }

            // make list of IsActive obj to toggle close/open
            familyActiveCheck = gsFuncObjects.Where(obj => obj.gsFunc == IsActive).ToList();

            assignFunctions();
        }

        void assignFunctions()
        {
            foreach (var obj in gsFuncObjects)
            {
                switch (obj.ve.GetType().Name)
                {
                    case nameof(SliderInt):
                        var slider = obj.ve as SliderInt;
                        DelegateInt sliderDel = (DelegateInt)obj.d;
                        var altDel = (DelegateInt)obj; // uses explicit conversion
                        slider.RegisterValueChangedCallback(v => sliderDel(v.newValue));
                        break;

                    case nameof(Toggle):
                        if (!obj.ve.GetClasses().ToList().Contains(FoldoutClassName))
                        {
                            var toggle = obj.ve as Toggle;

                            if (obj.f != null)
                            {
                                if (obj.gsFunc == IsActive)
                                {
                                    var del = (DelegateFamilyBoolObj)obj.d;
                                    toggle.RegisterValueChangedCallback(v => del(obj.f, v.newValue, obj));
                                }
                                else
                                {
                                    var del = (DelegateFamilyBool)obj.d;
                                    toggle.RegisterValueChangedCallback(v => del(obj.f, v.newValue));
                                }
                            }

                            if (obj.t != Territory.Default)
                            {
                                var del = (DelegateTerritoryBool)obj.d;
                                toggle.RegisterValueChangedCallback(v => del(obj.t, v.newValue));
                            }
                        }
                        break;

                    case nameof(DropdownField):
                        var dropdown = obj.ve as DropdownField;
                        if (obj.f != null)
                        {
                            var del = (DelegateFamilyInt)obj.d;
                            dropdown.RegisterValueChangedCallback(v => del(obj.f, dropdown.index));
                        }
                        break;
                    default: break;
                }                        
            }
            
            Refresh();
        }

        void Refresh()
        {
            foreach (var obj in gsFuncObjects)
            {
                if (obj.b != null)
                {
                    var toggle = (Toggle)obj.ve;
                    toggle.value = (bool)obj.b;
                }
                else if (obj.v != null) // always a Slider?
                {
                    var slider = (SliderInt)obj.ve;
                    slider.value = (int)obj.v;
                }
                else if (obj.t != Territory.Default)
                {
                    if (obj.f == null)
                    {
                        var toggle = (Toggle)obj.ve;
                        toggle.value = obj.listT.Contains(obj.t);
                    }
                    else // family location
                    {
                        var dropdown = (DropdownField)obj.ve;
                        dropdown.index = (int)obj.t;
                    }
                }

                // iterate through IsActive on each family
                foreach (var familyObj in familyActiveCheck)
                {
                    if (familyObj.b == false)
                    {
                        hideChildren(familyObj.p);
                        familyObj.ve.style.display = DisplayStyle.Flex;
                    }
                    if (familyObj.b == true)
                    {
                        showChildren(familyObj.p);
                    }
                }
            }
        }

        void hideChildren(VisualElement ve)
        {
            foreach (var child in ve.Children())
            { child.style.display = DisplayStyle.None; }
        }

        void showChildren(VisualElement ve)
        {
            foreach (var child in ve.Children())
            { child.style.display = DisplayStyle.Flex; }
        }

        void RefreshWithDictionary()
        { 
            Family fToFind = gs.Families.Where(f => f.Name == "Family A").FirstOrDefault();
            gsFuncObj foundit = gsFuncObjects.Where(obj =>
            obj.f == gs.Families.Where(f => f.Name == "Family A").FirstOrDefault() &&
            obj.b == obj.f.HasMan).FirstOrDefault();

            /*public int? v = null;
            public bool? b = null;
            public Family f = null;
            public Territory? t = null;
            public List<Territory> listT = null;*/

            // find each element that matches a gs value and set it
            var APslider = (SliderInt)statusPanel.Query(className: AP.ToString()).First();
            APslider.value = gs.AP;
            var CPslider = (SliderInt)statusPanel.Query(className: CP.ToString()).First();
            CPslider.value = gs.CP;
            var MPslider = (SliderInt)statusPanel.Query(className: MP.ToString()).First();
            MPslider.value = gs.MP;
            var Moraleslider = (SliderInt)statusPanel.Query(className: Morale.ToString()).First();
            Moraleslider.value = gs.Morale;
            var Ferocityslider = (SliderInt)statusPanel.Query(className: Ferocity.ToString()).First();
            Ferocityslider.value = gs.EnemyFerocity;

            //GameStateFunction[] familyFunctions = { Man, Woman, Child, Horse };
            foreach (var family in gs.AllFamilies)
            {
                //var foldout = statusPanel.Query<Foldout>(family.Name.Replace(" ", "")).First();
                var foldout = statusPanel.Query<Foldout>().Where(elem => elem.text == family.Name).First();
                var man = (Toggle)foldout.Query(className: Man.ToString()).First();
                man.value = family.HasMan;
                var woman = (Toggle)foldout.Query(className: Woman.ToString()).First();
                woman.value = family.HasWoman;
                var child = (Toggle)foldout.Query(className: Child.ToString()).First();
                child.value = family.HasChild;
                var horse = (Toggle)foldout.Query(className: Horse.ToString()).First();
                horse.value = family.HasHorse;
                /*foreach (var toggle in (Toggle[])foldout.Children())
                {
                    GameStateFunction match = Functions.
                }*/
            }

            // HasDrought, HasCorn, HasMission, HasRancho, HasFort
            //foreach (string terrString in Enum.GetNames(typeof(Territory)))
            // skip first and last Territory (Santa Fe and Default)
            for (int i = 1; i < (int)Territory.Default; i++)
            {
                Territory territory = (Territory) i;
                var foldout = statusPanel.Query<Foldout>(territory.ToString()).First();
                
                var drought = (Toggle)foldout.Query(className: HasDrought.ToString()).First();
                if (gs.HasDrought.Contains(territory)) drought.value = true;
                else drought.value = false;

                drought.value = gs.HasDrought.Contains(territory);

                var corn = (Toggle)foldout.Query(className: HasCorn.ToString()).First();
                if (gs.HasCorn.Contains(territory)) corn.value = true;
                else corn.value = false;
                
                var mission = (Toggle)foldout.Query(className: HasMission.ToString()).First();
                if (gs.HasMission.Contains(territory)) mission.value = true;
                else mission.value = false;
                
                var rancho = (Toggle)foldout.Query(className: HasRancho.ToString()).First();
                if (gs.HasRancho.Contains(territory)) rancho.value = true;
                else rancho.value = false;
                
                var fort = (Toggle)foldout.Query(className: HasFort.ToString()).First();
                if (gs.HasFort.Contains(territory)) fort.value = true;
                else fort.value = false;
                /*foreach (var toggle in (Toggle[])foldout.Children())
                {
                    GameStateFunction match = Functions.
                }*/
            }
        }

        (List<string>, Foldout) getParentClassesAndFoldout(VisualElement ve)
        {
            List<string> parentClasses = ve.parent.GetClasses().ToList();
            // check if parent is a Foldout
            if (parentClasses.Contains(FoldoutClassName)) return (parentClasses, ve.parent as Foldout);
            else return (parentClasses, null);
        }

        Dictionary<GameStateFunction, int> IntProperties;
        Dictionary<GameStateFunction, bool> BoolProperties;
        // need to asign a function to each bool
        Dictionary<Tuple<Family, bool>, GameStateFunction> FamilyProperties;
        // create a Dictionary of enum name/Delegate
        Dictionary<GameStateFunction, Delegate> Functions;

        void initializeFunctionsByDictionary()
        {
            IntProperties = new Dictionary<GameStateFunction, int>()
            {
                { AP, gs.AP },
                { CP, gs.CP }
            };

            BoolProperties = new Dictionary<GameStateFunction, bool>()
            {
                // doesn't work with bool in object or list
                { Man, gs.Families[0].HasMan }
            };

            FamilyProperties = new Dictionary<Tuple<Family, bool>, GameStateFunction>();

            foreach (Family f in gs.AllFamilies)
            {
                var man = new Tuple<Family, bool>(f, f.HasMan);
                FamilyProperties.Add(man, Man);
            }

            foreach (Family f in gs.AllFamilies)
            {
                /*gsFuncObjects.Add(new(Man, new DelegateFamilyBool((Family f, bool b) => f.HasMan = b), f.HasMan, f));
                gsFuncObjects.Add(new(Woman, new DelegateFamilyBool((Family f, bool b) => f.HasWoman = b), f.HasWoman, f));
                gsFuncObjects.Add(new(Child, new DelegateFamilyBool((Family f, bool b) => f.HasChild = b), f.HasChild, f));
                gsFuncObjects.Add(new(Horse, new DelegateFamilyBool((Family f, bool b) => f.HasHorse = b), f.HasHorse, f));*/
            }

            Family fToFind = gs.Families.Where(f => f.Name == "Family A").FirstOrDefault();
            gsFuncObj foundit = gsFuncObjects.Where(obj =>
                obj.f == gs.Families.Where(f => f.Name == "Family A").FirstOrDefault() &&
                obj.b == obj.f.HasMan).FirstOrDefault();

            // verbose set up of delegate and method
            DelegateInt APdel = setAP;
            void setAP(int v)
            { IntProperties[AP] = v; }

            Functions = new Dictionary<GameStateFunction, Delegate>()
            {
                { AP, APdel },
                { CP, new DelegateInt((int v) => IntProperties[CP] = v) },
                { MP, new DelegateInt((int v) => gs.MP = v) },
                { Morale, new DelegateInt((int v) => gs.Morale = v) },
                { Ferocity, new DelegateInt((int v) => gs.EnemyFerocity = v) },

                // EnemyRaid, TradeGoodsHeld, SheepHeld, HorsesHeld, Firearms
                
                // could call function in delegate declaration
                { HasDrought, new DelegateTerritoryBool((Territory t, bool b) =>
                toggleTerritoryValue(gs.HasDrought, t, b)) },
                { HasCorn, new DelegateTerritoryBool((Territory t, bool b) =>
                { if (b) gs.HasCorn.Add(t); else gs.HasCorn.Remove(t); }) },
                { HasMission, new DelegateTerritoryBool((Territory t, bool b) =>
                { if (b) gs.HasMission.Add(t); else gs.HasMission.Remove(t); }) },
                { HasRancho, new DelegateTerritoryBool((Territory t, bool b) =>
                { if (b) gs.HasRancho.Add(t); else gs.HasRancho.Remove(t); }) },
                { HasFort, new DelegateTerritoryBool((Territory t, bool b) =>
                { if (b) gs.HasFort.Add(t); else gs.HasFort.Remove(t); }) },

                { Man, new DelegateFamilyBool((Family f, bool b) => f.HasMan = b) },
                { Woman, new DelegateFamilyBool((Family f, bool b) => f.HasWoman = b) },
                { Child, new DelegateFamilyBool((Family f, bool b) => f.HasChild = b) },
                { Horse, new DelegateFamilyBool((Family f, bool b) => f.HasHorse = b) }
            };

            void toggleTerritoryValue(List<Territory> list, Territory t, bool b)
            {
                if (b) list.Add(t);
                else list.Remove(t);
            }

            print("Functions initialized: " + Functions.Count);
        }

        void assignFunctionsWithoutObject()
        {
            foreach (var (function, del) in Functions)
            {
                List<VisualElement> elements =
                    statusPanel.Query(className: function.ToString()).ToList();

                string elementType = elements[0].GetType().Name;

                print($"Function {function} is type: {elementType}, there are {elements.Count}");

                switch (elementType)
                {
                    case nameof(SliderInt):
                        foreach (SliderInt slider in elements)
                        {
                            var sliderDel = (DelegateInt)del;
                            slider.RegisterValueChangedCallback(v => sliderDel(v.newValue));
                            print($"Registering Callback for {slider.name}");
                        }
                        break;

                    case nameof(Toggle):
                        foreach (Toggle toggle in elements)
                        {
                            if (!toggle.GetClasses().ToList().Contains(Foldout.toggleUssClassName))
                            {
                                //make separate method to check parent against Territories, etc.
                                //this assumes all toggles are in foldouts
                                var foldout = toggle.parent as Foldout;
                                var parentClasses = toggle.parent.GetClasses().ToList();

                                // check if the foldout parent matches a Family
                                Family foldoutFamily = gs.AllFamilies
                                    .Where(f => f.Name == foldout.text).FirstOrDefault();
                                if (foldoutFamily != null)
                                {
                                    var toggleDel = (DelegateFamilyBool)del;
                                    toggle.RegisterValueChangedCallback(v => toggleDel(foldoutFamily, v.newValue));
                                }

                                // check if the foldout parent matches a Territory
                                string terr = Enum.GetNames(typeof(Territory))
                                    .Where(t => parentClasses.Contains(t)).FirstOrDefault();
                                Territory foldoutTerritory = Territory.Default;
                                if (terr != null)
                                { foldoutTerritory = (Territory)Enum.Parse(typeof(Territory), terr); }
                                if (foldoutTerritory != Territory.Default)
                                {
                                    var toggleDel = (DelegateTerritoryBool)del;
                                    toggle.RegisterValueChangedCallback(v => toggleDel(foldoutTerritory, v.newValue));
                                }
                            }
                            print($"Registering Callback for {toggle.name}");
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /*void assignFunctionsByEnum()
        {
            foreach (string function in Enum.GetNames(typeof(GameStateFunction)))
            {
                VisualElement element = statusPanel.Query(className: function);
                var key = (GameStateFunction)Enum.Parse(typeof(GameStateFunction), function);

                if (element is SliderInt slider)
                {
                    var sliderFunc = (DelegateInt)Functions[key];
                    slider.RegisterValueChangedCallback(v => sliderFunc(v.newValue));
                }

                if (element is Toggle toggle &&
                    !element.GetClasses().ToList().
                    Contains(Foldout.toggleUssClassName))
                {
                    //make separate method to check parent against Territories, etc.
                    var foldout = toggle.parent as Foldout;
                    var parentClasses = toggle.parent.GetClasses().ToList();
                    string parentClass = "";
                    Family foldoutFamily = null;
                    foreach (string terr in Enum.GetNames(typeof(Territory)))
                    {
                        if (parentClasses.Contains(terr)) parentClass = terr;
                        // could register callback here
                    }
                    foreach (Family family in gs.AllFamilies)
                    {
                        string name = family.Name.Replace(" ", "");
                        if (parentClasses.Contains(name))
                        {
                            parentClass = name;
                            foldoutFamily = family;
                        }
                    }
                    // assign a family to test
                    foldoutFamily = (Family)gs.AllFamilies.Where(f => f.Name == foldout.text);
                    foldoutFamily = gs.Families[1]; // delete after testing
                    if (foldoutFamily != null)
                    {
                        // register callback that passes v and foldoutFamily
                    }
                    if (parentClass != "")
                    {
                        // make delegate that takes v and parentClass
                        var toggleFunc = (DelegateFamilyBool)Functions[key];
                        toggle.RegisterValueChangedCallback(v => toggleFunc(foldoutFamily, v.newValue, parentClass));
                    }
                }
            }
        }*/

        /* CONSTRUCTORS:
         * public gsFuncObj(GameStateFunction gsFunc, Delegate d, int v)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.v = v;
}

public gsFuncObj(GameStateFunction gsFunc, Delegate d, bool b)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.b = b;
}

public gsFuncObj(GameStateFunction gsFunc, Delegate d, bool b, Family f)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.b = b;
    this.f = f;
}

public gsFuncObj(GameStateFunction gsFunc, Delegate d, bool b, Territory t)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.b = b;
    this.t = t;
}

public gsFuncObj(GameStateFunction gsFunc, Delegate d, Territory t, Family f)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.t = t;
    this.f = f;
}

public gsFuncObj(GameStateFunction gsFunc, Delegate d, Territory t, List<Territory> listT)
{
    this.gsFunc = gsFunc;
    this.d = d;
    this.t = t;
    this.listT = listT;
}*/

    }
}
