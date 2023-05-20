using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static NavajoWars.GameStateFunction;
using gsfo = NavajoWars.GameStateUI.GameStateFunctionObject;

namespace NavajoWars
{
    public enum GameStateFunction 
    { 
        AP, CP, MP, Morale, 
        EnemyAP, EnemyFerocity, 
        TradeGoodsHeld, SheepHeld, HorsesHeld, Firearms, 
        HasDrought, HasCorn, HasMission, HasRancho, HasFort,
        IsActive, Man, Woman, Child, Horse, Ferocity, IsWhere,
        PassageDisplay, ElderDisplay, GoodsDisplay, 
        AddPersonToPassage, AddGood
    }

    // TODO: add Elder List - foldout of sliders??

    public class GameStateUI : MonoBehaviour
    {
        GameManager gm;
        GameState gs;

        public delegate void Delegate(GameStateFunctionObject obj);
        public delegate int DelegateInt();
        public delegate bool DelegateBool();
        public delegate Territory DelegateTerritory();
        public delegate List<Territory> DelegateListTerritory();

        VisualElement statusPanel;
        Button status;

        // string FoldoutClassName = Foldout.toggleUssClassName;

        public class GameStateFunctionObject
        {
            public GameStateFunction tag;
            public Delegate callback;
            public VisualElement ve = null;
            public DelegateInt getValue = null;
            public int setValue;
            public DelegateBool getBool = null;
            public bool setBool;
            public Family f = null;
            public Territory t = Territory.Default;
            // public DelegateTerritory getT = null; // necessary?
            // public DelegateListTerritory getListT = null; // necessary
            public VisualElement parent = null;

            public GameStateFunctionObject() { }

            public GameStateFunctionObject(GameStateFunction tag) 
            { this.tag = tag; }
        }

        List<GameStateFunctionObject> gsfObjects = new();
        List<GameStateFunctionObject> familyActiveCheck = new();
        string[] personStyles = { "ButtonMan", "ButtonWoman", "ButtonChild", "ButtonElder" };

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
            gsfo[] toplevelObjects =
            {
                new(AP)
                {
                    callback = new((gsfo obj) => gs.AP = obj.setValue),
                    getValue = delegate { return gs.AP; }
                },
                new(CP)
                {
                    callback = new((gsfo obj) => gs.CP = obj.setValue),
                    getValue = delegate { return gs.CP; }
                },
                new(MP)
                {
                    callback = new((gsfo obj) => gs.MP = obj.setValue),
                    getValue = delegate { return gs.MP; }
                },
                new(Morale)
                {
                    callback = new((gsfo obj) => gs.Morale = obj.setValue),
                    getValue = delegate { return gs.Morale; }
                },
                new(EnemyFerocity)
                {
                    callback = new((gsfo obj) => gs.EnemyFerocity = obj.setValue),
                    getValue = delegate { return gs.EnemyFerocity; }
                }
            };

            foreach (gsfo obj in toplevelObjects)
            {
                obj.ve = elem(obj.tag);
                gsfObjects.Add(obj);
            }

            foreach (Family family in gs.AllFamilies)
            {
                Foldout foldout = (Foldout)elem(family.Name.Replace(" ", ""));
                foldout.value = false; // start closed

                gsfo[] familyObjects =
                {
                    new(IsActive)
                    {
                        callback = new((gsfo obj) => { obj.f.IsActive = obj.setBool;
                            if (obj.setBool) showChildren(obj.parent);
                            else
                            {
                                hideChildren(obj.parent);
                                obj.ve.style.display = DisplayStyle.Flex;
                            }
                        }),
                        getBool = delegate { return family.IsActive; }
                    },
                    new(Man)
                    {
                        callback = new((gsfo obj) => obj.f.HasMan = obj.setBool),
                        getBool = delegate { return family.HasMan; }
                    },
                    new(Woman)
                    {
                        callback = new((gsfo obj) => obj.f.HasWoman = obj.setBool),
                        getBool = delegate { return family.HasWoman; }
                    },
                    new(Child)
                    {
                        callback = new((gsfo obj) => obj.f.HasChild = obj.setBool),
                        getBool = delegate { return family.HasChild; }
                    },
                    new(Horse)
                    {
                        callback = new((gsfo obj) => obj.f.HasHorse = obj.setBool),
                        getBool = delegate { return family.HasHorse; }
                    },
                    new(IsWhere)
                    {
                        callback = new((gsfo obj) => obj.f.IsWhere = (Territory)obj.setValue + 1),
                        getValue = delegate { return (int)family.IsWhere; }
                    },
                    new(Ferocity) 
                    {
                        callback = new((gsfo obj) => obj.f.Ferocity = obj.setValue),
                        getValue = delegate { return family.Ferocity; }
                    }
                };

                foreach (gsfo obj in familyObjects)
                {
                    obj.f = family;
                    obj.ve = elem(obj.tag, foldout);
                    obj.parent = foldout;
                    gsfObjects.Add(obj);
                }
            }

            // make list of IsActive obj to toggle close/open
            familyActiveCheck = gsfObjects.Where(obj => obj.tag == IsActive).ToList();

            for (int i = 1; i < (int)Territory.Default; i++)
            {
                Territory territory = (Territory)i;
                Foldout foldout = (Foldout)elem($"{territory}");
                foldout.value = false; // start closed

                gsfo[] territoryObjects =
                {
                    new(HasDrought)
                    {
                        callback = new((gsfo obj) =>
                            toggleTerritoryValue(gs.HasDrought, obj.t, obj.setBool)),
                        getBool = delegate { return gs.HasDrought.Contains(territory); }
                    },
                    new(HasCorn)
                    {
                    callback = new((gsfo obj) =>
                        toggleTerritoryValue(gs.HasCorn, obj.t, obj.setBool)),
                    getBool = delegate { return gs.HasCorn.Contains(territory); }
                    },
                    new(HasMission)
                    {
                        callback = new((gsfo obj) =>
                            toggleTerritoryValue(gs.HasMission, obj.t, obj.setBool)),
                        getBool = delegate { return gs.HasMission.Contains(territory); }
                    },
                    new(HasRancho)
                    {
                        callback = new((gsfo obj) =>
                            toggleTerritoryValue(gs.HasRancho, obj.t, obj.setBool)),
                        getBool = delegate { return gs.HasRancho.Contains(territory); }
                    },
                    new(HasFort)
                    {
                        callback = new((gsfo obj) =>
                            toggleTerritoryValue(gs.HasFort, obj.t, obj.setBool)),
                        getBool = delegate { return gs.HasFort.Contains(territory); }
                    }   
                };
                
                foreach (gsfo obj in territoryObjects)
                {
                    obj.t = territory;
                    obj.ve = elem(obj.tag, foldout);
                    obj.parent = (Foldout)foldout;
                    gsfObjects.Add(obj);
                }
            }
            
            void toggleTerritoryValue(List<Territory> list, Territory t, bool b)
            {
                if (b && !list.Contains(t)) list.Add(t);
                if (!b) list.Remove(t); // "else" would remove whenever list.Contains(t)
            }

            // add userData to AddPerson buttons
            // can find the buttons by style
            //List<Button> personButtons = gsfObjects.Where(obj => obj.tag == AddPersonToPassage).Select(obj => (Button)obj.ve).ToList();            
            // Man, Woman, Child, Elder
            for (int i = 0; i < personStyles.Count(); i++)
            {
                string style = personStyles[i];
                gsfo obj = new(AddPersonToPassage)
                {
                    callback = new((gsfo obj) =>
                    {
                        var person = (Person)obj.setValue;
                        gs.PersonsInPassage.Add(person);
                        addButtonToPassageDisplay(person);
                    }),
                    ve = statusPanel.Query(className: style).Where(e => e.ClassListContains($"{AddPersonToPassage}")),
                };

                if (obj.ve == null)
                { Debug.Log($"No Button found for {style}"); }
                else obj.ve.userData = (Person)i;

                gsfObjects.Add(obj);
            }

            assignCallBacks();
        }

        void assignCallBacks()
        {
            foreach (var obj in gsfObjects)
            {
                if (obj.ve is DropdownField dropdown)
                {
                    dropdown.RegisterValueChangedCallback((evt) =>
                    { obj.setValue = dropdown.index; obj.callback(obj); });
                }
                else if (obj.ve is Button button)
                {
                    button.clickable.clickedWithEventInfo += (EventBase evt) =>
                    { obj.setValue = (int)button.userData; obj.callback(obj); };
                }
                else if (obj.ve is INotifyValueChanged<int> veInt)
                {
                    veInt.RegisterValueChangedCallback((evt) =>
                    { obj.setValue = evt.newValue; obj.callback(obj); });
                }
                else if (obj.ve is INotifyValueChanged<bool> veBool)
                {
                    veBool.RegisterValueChangedCallback(evt =>
                    { obj.setBool = evt.newValue; obj.callback(obj); });
                }
            }
        }

        void Refresh()
        {
            foreach (var obj in gsfObjects)
            { 
                if (obj.ve is DropdownField dropdown)
                { dropdown.index = obj.getValue() - 1; }
                // cannot use SetValueWithoutNotify on index, only value
                else if (obj.ve is INotifyValueChanged<int> veInt)
                { veInt.SetValueWithoutNotify(obj.getValue()); }
                else if (obj.ve is INotifyValueChanged<bool> veBool)
                { veBool.SetValueWithoutNotify(obj.getBool()); }
                // eInt.value = obj.getValue(); 
                // will not affect buttons because they are type <string>
            }

            // iterate through IsActive on each family
            foreach (var familyObj in familyActiveCheck)
            {
                if (familyObj.getBool())
                { showChildren(familyObj.parent); }
                else
                {
                    hideChildren(familyObj.parent);
                    familyObj.ve.style.display = DisplayStyle.Flex;
                }
            }

            foreach (var person in elem(PassageDisplay).Children().ToList())
            { person.RemoveFromHierarchy(); }
            foreach (var passagePerson in gs.PersonsInPassage)
            { addButtonToPassageDisplay(passagePerson); }
        }

        void addButtonToPassageDisplay(Person person)
        {
            var info = new ButtonInfo()
            {
                name = $"{person}",
                text = "",
                call = delegate { gs.PersonsInPassage.Remove(person); },
                style = personStyles[(int)person]
                // this style is added to class list on make
                /* person switch
                {
                    Person.Man => "ButtonMan",
                    Person.Woman => "ButtonWoman",
                    Person.Child => "ButtonChild",
                    Person.Elder => "ButtonElder"
                }, */
            };

            var button = info.MakeWithCall();
            // adds call directly to .clicked
            button.AddToClassList("Person");
            // need to also add the style "Person"
            // add callback to remove button when clicked
            button.RegisterCallback<ClickEvent>(evt =>
            {
                var b = evt.target as Button;
                b.RemoveFromHierarchy();
            });

            elem(PassageDisplay).Add(button);
            button.visible = true;
            button.style.display = DisplayStyle.Flex;
        }

        // each of these returns the First ve found
        VisualElement elem(GameStateFunction tag) => findVisualElement($"{tag}", statusPanel);

        VisualElement elem(string s) => findVisualElement($"{s}", statusPanel);

        VisualElement elem(GameStateFunction tag, VisualElement parent) => findVisualElement($"{tag}", parent);

        // TODO: make these utility functions:

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

        VisualElement findVisualElement(string s, VisualElement area)
        {
            VisualElement ve = area.Q(className: s); 
            if (ve != null) return ve;
            else
            {
                Debug.LogError($"No VisualElement found for class {s} in {area}");
                return null;
            }
        }
        
        // this searches from element up:
        /*var list = statusPanel.Query(className: $"{tag}").ToList();
        if (list == null || list.Count == 0)
        {
            Debug.LogError($"No VisualElement found anywhere with class {tag}");
            return null;
        }
        else
        {
            foreach ( var element in list) 
            {
                if (element.GetFirstOfType<Foldout>() == parent)
                { 
                    ve = element;
                    break;
                }
            }
            // ve = list.Where(e => e.parent == parent).FirstOrDefault();
            ve = list.Where(e => e.GetFirstOfType<Foldout>() == parent).FirstOrDefault(); 
            if (ve != null) return ve;
            else
            {
                Debug.LogError($"Class {tag} found, but none found in foldout {parent}");
                return null;
            }
        }*/
    }
}
