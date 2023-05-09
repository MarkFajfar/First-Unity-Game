using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public class GameStateUI : MonoBehaviour
    {
        GameManager gm;
        GameState gs;

        delegate void Delegate(gsFuncObj obj);
        delegate int DelegateInt();
        delegate bool DelegateBool();
        delegate Territory DelegateTerritory();
        delegate List<Territory> DelegateListTerritory();

        VisualElement statusPanel;
        Button status;

        // string FoldoutClassName = Foldout.toggleUssClassName;

        class gsFuncObj
        {
            public GameStateFunction gsFunc;
            public Delegate callback;
            public VisualElement ve;
            public DelegateInt getV = null;
            public int setV;
            public DelegateBool getB = null;
            public bool setB;
            public Family f = null;
            public Territory t = Territory.Default;
            public DelegateTerritory getT = null; // necessary?
            public DelegateListTerritory getListT = null; // necessary
            public VisualElement p = null;

            public gsFuncObj() { }
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
                callback = new((gsFuncObj obj) => gs.AP = obj.setV),
                // setting gs.AP value is different from setting obj.v!!
                getV = delegate { return gs.AP; },
                ve = elem(AP)
            } );
            gsFuncObjects.Add(new()
            {
                gsFunc = CP,
                callback = new((gsFuncObj obj) => gs.CP = obj.setV),
                getV = delegate { return gs.CP; },
                ve = elem(CP)
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = MP,
                callback = new((gsFuncObj obj) => gs.MP = obj.setV),
                getV = delegate { return gs.MP; },
                ve = elem(MP)
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = Morale,
                callback = new((gsFuncObj obj) => gs.Morale = obj.setV),
                getV = delegate { return gs.Morale; },
                ve = elem(Morale)
            }); 
            gsFuncObjects.Add(new()
            {
                gsFunc = EnemyFerocity,
                callback = new((gsFuncObj obj) => gs.EnemyFerocity = obj.setV),
                getV = delegate { return gs.EnemyFerocity; },
                ve = elem(EnemyFerocity)
            });

            foreach (Family family in gs.AllFamilies)
            {
                VisualElement foldout = elem(family.Name.Replace(" ", ""));
                gsFuncObjects.Add(new()
                {
                    gsFunc = IsActive,
                    callback = new ((gsFuncObj obj) => { obj.f.IsActive = obj.setB;
                        if (obj.setB) showChildren(obj.p);
                        else
                        {
                            hideChildren(obj.p);
                            obj.ve.style.display = DisplayStyle.Flex;
                        }
                    }),
                    getB = delegate { return family.IsActive; },
                    f = family,
                    ve = elem(IsActive, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Man,
                    callback = new((gsFuncObj obj) => obj.f.HasMan = obj.setB),
                    getB = delegate { return family.HasMan; },
                    f = family,
                    ve = elem(Man, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Woman,
                    callback = new((gsFuncObj obj) => obj.f.HasWoman = obj.setB),
                    getB = delegate { return family.HasWoman; },
                    f = family,
                    ve = elem(Woman, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Child,
                    callback = new((gsFuncObj obj) => obj.f.HasChild = obj.setB),
                    getB = delegate { return family.HasChild; },
                    f = family,
                    ve = elem(Child, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = Horse,
                    callback = new((gsFuncObj obj) => obj.f.HasHorse = obj.setB),
                    getB = delegate { return family.HasHorse; },
                    f = family,
                    ve = elem(Horse, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = IsWhere,
                    callback = new ((gsFuncObj obj) => obj.f.IsWhere = (Territory)obj.setV + 1),
                    getV = delegate { return (int)family.IsWhere; },
                    f = family,
                    ve = elem(IsWhere, foldout),
                    p = (Foldout)foldout
                });
            }

            for (int i = 1; i < (int)Territory.Default; i++)
            {
                Territory terr = (Territory)i;
                VisualElement foldout = elem(terr.ToString());

                gsFuncObjects.Add(new()
                {
                    gsFunc = HasDrought,
                    callback = new((gsFuncObj obj) =>
                        toggleTerritoryValue(gs.HasDrought, obj.t, obj.setB)),
                    t = terr,
                    getB = delegate { return gs.HasDrought.Contains(terr); },
                    ve = elem(HasDrought, foldout),
                    p = (Foldout)foldout
                });
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasCorn,
                    callback = new((gsFuncObj obj) =>
                        toggleTerritoryValue(gs.HasCorn, obj.t, obj.setB)),
                    t = terr,
                    getB = delegate { return gs.HasCorn.Contains(terr); },
                    ve = elem(HasCorn, foldout),
                    p = (Foldout)foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasMission,
                    callback = new((gsFuncObj obj) =>
                        toggleTerritoryValue(gs.HasMission, obj.t, obj.setB)),
                    t = terr,
                    getB = delegate { return gs.HasMission.Contains(terr); },
                    ve = elem(HasMission, foldout),
                    p = (Foldout)foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasRancho,
                    callback = new((gsFuncObj obj) =>
                        toggleTerritoryValue(gs.HasRancho, obj.t, obj.setB)),
                    t = terr,
                    getB = delegate { return gs.HasRancho.Contains(terr); },
                    ve = elem(HasRancho, foldout),
                    p = (Foldout)foldout
                }); 
                gsFuncObjects.Add(new()
                {
                    gsFunc = HasFort,
                    callback = new((gsFuncObj obj) =>
                        toggleTerritoryValue(gs.HasFort, obj.t, obj.setB)),
                    t = terr,
                    getB = delegate { return gs.HasFort.Contains(terr); },
                    ve = elem(HasFort, foldout),
                    p = (Foldout)foldout
                });
            }
            void toggleTerritoryValue(List<Territory> list, Territory t, bool b)
            {
                if (b && !list.Contains(t)) list.Add(t);
                if (!b) list.Remove(t); // else would remove whenever the list contains
            }

            // make list of IsActive obj to toggle close/open
            familyActiveCheck = gsFuncObjects.Where(obj => obj.gsFunc == IsActive).ToList();

            assignCallBacks();
        }

        void assignCallBacks()
        {
            foreach (var obj in gsFuncObjects)
            {
                if (obj.ve is DropdownField dropdown)
                {
                    dropdown.RegisterValueChangedCallback((evt) =>
                    { obj.setV = dropdown.index; obj.callback(obj); });
                }
                else if (obj.ve is INotifyValueChanged<int> eInt)
                {
                    eInt.RegisterValueChangedCallback((evt) =>
                    { obj.setV = evt.newValue; obj.callback(obj); });
                }
                else if (obj.ve is INotifyValueChanged<bool> eBool)
                {
                    eBool.RegisterValueChangedCallback(evt =>
                    { obj.setB = evt.newValue; obj.callback(obj); });
                }
            }
        }

        void Refresh()
        {
            foreach (var obj in gsFuncObjects)
            { 
                if (obj.ve is DropdownField dropdown)
                { dropdown.index = obj.getV() - 1; }
                /*else if (obj.t != Territory.Default) // not necessary using obj.getB
                {
                    var eBool = (INotifyValueChanged<bool>)obj.ve;
                    eBool.value = obj.getListT().Contains(obj.t);
                }*/
                else if (obj.ve is INotifyValueChanged<int> eInt)
                { eInt.value = obj.getV(); }
                else if (obj.ve is INotifyValueChanged<bool> eBool)
                { eBool.value = obj.getB(); }
            }

            // iterate through IsActive on each family
            foreach (var familyObj in familyActiveCheck)
            {
                if (familyObj.getB()) showChildren(familyObj.p);
                else 
                {
                    hideChildren(familyObj.p);
                    familyObj.ve.style.display = DisplayStyle.Flex;
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

        VisualElement elem(GameStateFunction gsFunc) 
        {
            VisualElement ve = statusPanel.Query(className: gsFunc.ToString());
            if (ve != null) return ve; 
            else 
            {
                Debug.LogError($"No VisualElement found for class {gsFunc}");
                return null; 
            }
        }

        VisualElement elem(string s)
        {
            VisualElement ve = statusPanel.Query(className: s);
            if (ve != null) return ve;
            else
            {
                Debug.LogError($"No VisualElement found for class {s}");
                return null;
            }
        }

        VisualElement elem(GameStateFunction gsFunc, VisualElement p)
        {
            VisualElement ve;
            var list = statusPanel.Query(className: gsFunc.ToString()).ToList();
            if (list == null || list.Count == 0)
            {
                Debug.LogError($"No VisualElement found anywhere with class {gsFunc}");
                return null;
            }
            else
            {
                ve = list.Where(e => e.parent == p).FirstOrDefault();
                if (ve != null) return ve;
                else
                {
                    Debug.LogError($"Class {gsFunc} found, but none found in foldout {p}");
                    return null;
                }
            }
        }
    }
}
