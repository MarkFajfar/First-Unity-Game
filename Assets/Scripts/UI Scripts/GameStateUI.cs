using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static NavajoWars.GameStateTag;
using gso = NavajoWars.GameStateObject;

namespace NavajoWars
{    public class GameStateUI : MonoBehaviour, IChangeGameState
    {
        GameManager gm;
        GameState gs;
        GameStateEvent gse;

        public event EventHandler<GameStateObject> OnGameStateChanged;

        List<GameStateObject> gsObjects; 
        List<GameStateObject> familyActiveCheck;
        // personStyles may not be correct
        //string[] personStyles = { $"{Man}", $"{Woman}", $"{Child}", $"{Elder}" };
        //string[] resourceStyles = { $"{Horse}",  $"{Sheep}",  $"{Corn}",  $"{TradeGood}",  $"{Firearm}" };
        //string[] cubeStyles = { $"{Black}", $"{White}", $"{Brown}", $"{Red}", $"{Yellow}", $"{Green}", $"{Blue}" };
        List<List<Cube>> Bowls; 
        VisualElement statusPanel;
        Button status;

        // string FoldoutClassName = Foldout.toggleUssClassName;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            gse = gmobj.GetComponent<GameStateEvent>(); 

            gsObjects = new();
            familyActiveCheck = new();
            Bowls = new List<List<Cube>> { gs.Raided, gs.Recovery, gs.Subjugation };
        }

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            statusPanel = root.Q<VisualElement>("StatusPanel");
            status = root.Q<Button>("Status");
            status.clicked += Refresh;

            OnGameStateChanged += onChange;

            initializeObjects();
        }

        void OnDisable()
        {
            OnGameStateChanged -= onChange;
        }

        void initializeObjects()
        {
            GameStateTag[] toplevelTags = 
            { 
                AP, CP, MP, EnemyAP, Morale, EnemyFerocity, 
                Elder0, Elder1, Elder2, Elder3, Elder4, Elder5, Elder6 
            };

            // create a gso for each tag and add to gsObjects
            foreach (var tagToAdd in toplevelTags)
            {
                gsObjects.Add(new()
                {
                    tag = tagToAdd,
                    ve = element(tagToAdd)
                });
            }

            GameStateTag[] familyTags =
            {
                IsActive, Man, Woman, Child, Horse, IsWhere, Ferocity
            }; 

            foreach (Family family in gs.AllFamilies)
            {
                Foldout foldout = (Foldout)element(family.Name.Replace(" ", ""));
                foldout.value = false; // start closed
                foreach (var tagToAdd in familyTags)
                {
                    gsObjects.Add(new()
                    {
                        tag = tagToAdd,
                        f = family,
                        ve = element(tagToAdd, foldout),
                        parent = foldout
                    });
                }
            }

            // make list of IsActive obj to toggle close/open
            familyActiveCheck = gsObjects.Where(obj => obj.tag == IsActive).ToList();

            GameStateTag[] territoryTags =
            {
                DroughtNum, CornNum, HasMission, HasRancho, HasFort
            };

            foreach (Territory territory in gs.Territories)
            {
                if (territory.Tag == eTerritory.SantaFe || territory.Tag == eTerritory.Default) continue;
            
                Foldout foldout = (Foldout)element($"{territory.Tag}");
                foldout.value = false; // start closed
                foreach (var tagToAdd in territoryTags)
                {
                    gsObjects.Add(new()
                    {
                        tag = tagToAdd,
                        t = territory,
                        ve = element(tagToAdd, foldout),
                        parent = foldout
                    });
                }
            }

            // following is for the Person, Resource and Cube buttons 
            // that are always present, to add a new item to bowl
            // the buttons that are added to the bowl (and can be removed)
            // are created in the addButton functions
            foreach (Person person in Enum.GetValues(typeof(Person)))
            {
                if (person == Person.Default) continue;

                gso obj = new()
                {
                    tag = AddPersonToPassage,
                    p = person,
                    ve = statusPanel.Query(className: $"{person}").Where(e => e.ClassListContains($"{AddPersonToPassage}")),
                };

                if (obj.ve != null) obj.ve.userData = person; 
                // put person (resource, cube) enum in userData so that 
                // any userData can be converted to int in callback

                gsObjects.Add(obj);
            }

            foreach (Resource resource in Enum.GetValues(typeof(Resource)))
            {
                if (resource == Resource.Default) continue;

                gso obj = new()
                {
                    tag = AddResource,
                    r = resource,
                    ve = statusPanel.Query(className: $"{resource}").Where(e => e.ClassListContains($"{AddResource}")),
                };

                if (obj.ve != null) obj.ve.userData = resource; 

                gsObjects.Add(obj);
            }

            foreach (var bowlToAdd in Bowls)
            {
                // tag denotes bowl, userData in each ve denotes cube
                GameStateTag tag = Default;
                if (bowlToAdd == gs.Raided) tag = AddToRaided;
                if (bowlToAdd == gs.Recovery) tag = AddToRecovery;
                if (bowlToAdd == gs.Subjugation) tag = AddToSubjugation;

                foreach (Cube cube in Enum.GetValues(typeof(Cube)))
                {
                    if (cube == Cube.Default) continue;

                    gso obj = new(tag)
                    {
                        cube = cube,
                        ve = statusPanel.Query(className: $"{cube}").Where(e => e.ClassListContains($"{tag}")),
                    };

                    if (obj.ve != null) obj.ve.userData = cube; 

                    gsObjects.Add(obj);
                }
            }

            assignCallBacks();
        }
            
        /// <summary>
        /// so that when clicked: set the value in the object, 
        /// and then pass the object to GameStateEvent via OnGameStateChanged
        /// </summary>
        void assignCallBacks()
        {
            foreach (var obj in gsObjects)
            {
                if (obj.ve is DropdownField dropdown)
                {
                    // dropdown value is a string so use int in index
                    dropdown.RegisterValueChangedCallback((evt) =>
                    { obj.setInt = dropdown.index; OnGameStateChanged?.Invoke(this, obj); });
                    //{ obj.setInt = dropdown.index; obj.callback(obj); });
                }
                else if (obj.ve is Button button)
                {
                    // assumes that userData is an int (or enum converted to int) to be used for setInt
                    button.clickable.clickedWithEventInfo += (EventBase evt) =>
                    { obj.setInt = (int)button.userData; OnGameStateChanged?.Invoke(this, obj); };
                }
                else if (obj.ve is INotifyValueChanged<int> veInt)
                {
                    veInt.RegisterValueChangedCallback((evt) =>
                    { obj.setInt = evt.newValue; OnGameStateChanged?.Invoke(this, obj); });
                }
                else if (obj.ve is INotifyValueChanged<bool> veBool)
                {
                    veBool.RegisterValueChangedCallback((evt) =>
                    { obj.setBool = evt.newValue; OnGameStateChanged?.Invoke(this, obj); });
                }
            }
        }

        void Refresh()
        {
            Label population = (Label)element(Population);
            population.text = $"Population {gs.Population}";
            Label arability = (Label)element(Arability);
            arability.text = $"Arability {gs.Arability}";

            foreach (gso obj in gsObjects)
            { 
                if (obj.ve is DropdownField dropdown)
                { dropdown.index = gse.ReturnInt(obj) - 1; }
                // cannot use SetValueWithoutNotify on index, only value
                else if (obj.ve is INotifyValueChanged<int> veInt)
                { veInt.SetValueWithoutNotify(gse.ReturnInt(obj)); }
                else if (obj.ve is INotifyValueChanged<bool> veBool)
                { veBool.SetValueWithoutNotify(gse.ReturnBool(obj)); }
                // eInt.value = obj.getValue(); 
                // will not affect buttons because they are type <string>
            }

            // iterate through IsActive on each family
            foreach (var familyObj in familyActiveCheck)
            {
                if (gse.ReturnBool(familyObj)) familyObj.parent.ShowChildren(); //(familyObj.getBool())
                //{ showChildren(familyObj.parent); }
                else
                {
                    // hideChildren(familyObj.parent);
                    familyObj.parent.HideChildren();
                    familyObj.ve.style.display = DisplayStyle.Flex;
                }
            }

            foreach (var person in element(PassageDisplay).Children().ToList())
            { person.RemoveFromHierarchy(); }
            foreach (var passagePerson in gs.PersonsInPassage)
            { addButtonToPassageDisplay(passagePerson); }

            foreach (var resource in element(ResourceDisplay).Children().ToList())
            { resource.RemoveFromHierarchy(); }
            foreach (var resource in gs.Resources)
            { addButtonToResourceDisplay(resource); }

            foreach (var cube in element(Raided).Children().ToList())
            { cube.RemoveFromHierarchy(); }
            foreach (var cube in gs.Raided)
            { addCubeButtonToBowl(cube, gs.Raided); }

            foreach (var cube in element(Recovery).Children().ToList())
            { cube.RemoveFromHierarchy(); }
            foreach (var cube in gs.Recovery)
            { addCubeButtonToBowl(cube, gs.Recovery); }

            foreach (var cube in element(Subjugation).Children().ToList())
            { cube.RemoveFromHierarchy(); }
            foreach (var cube in gs.Subjugation)
            { addCubeButtonToBowl(cube, gs.Subjugation); }
        }

        /// <summary>
        /// for any UI action when sending change to GameStateEvent
        /// </summary>
        void onChange(object s, gso obj)
        {
            if (obj.tag == IsActive)
            {
                if (obj.setBool) obj.parent.ShowChildren(); // showChildren(obj.parent);
                else
                {
                    //hideChildren(obj.parent);
                    obj.parent.HideChildren();
                    obj.ve.style.display = DisplayStyle.Flex;
                }
            }
            if (obj.tag == AddPersonToPassage)
            {
                addButtonToPassageDisplay(obj.p);
            }
            if (obj.tag == AddResource)
            {
                addButtonToResourceDisplay(obj.r);
            }
            if (obj.tag == AddToRaided)
            {
                addCubeButtonToBowl(obj.cube, gs.Raided);
            }
            if (obj.tag == AddToRecovery)
            {
                addCubeButtonToBowl(obj.cube, gs.Recovery);
            }
            if (obj.tag == AddToSubjugation)
            {
                addCubeButtonToBowl(obj.cube, gs.Subjugation);
            }
        }
        
        void addCubeButtonToBowl(Cube cube, List<Cube> bowl)
        {
            var button = makeButton(new ButtonInfo()
            {
                name = $"{cube}",
                text = "",
                style = $"{cube}",
                cube = cube,
                gso = new gso(ButtonPassage)
                { setInt = (int)cube, cube = cube, bowl = bowl },
                remove = true
            });
            
            string bowlName = "";
            if (bowl == gs.Raided) bowlName = $"{Raided}";
            if (bowl == gs.Recovery) bowlName = $"{Recovery}";
            if (bowl == gs.Subjugation) bowlName = $"{Subjugation}";
            element(bowlName).Add(button);
            button.visible = true;
            button.style.display = DisplayStyle.Flex;
        }

        void addButtonToPassageDisplay(Person person)
        {
            var button = makeButton(new ButtonInfo()
            {
                name = $"{person}",
                text = "",
                person = person,
                style = $"{person}",
                // this style is added to class list on make
                gso = new gso(ButtonPassage)
                { setInt = (int)person, p = person },
                // gsfo just needs tag and data as int in setInt (not otherwise set?)
                remove = true
                // will be removed when clicked                
            });
            /* person switch
                {
                    Person.Man => "ButtonMan",
                    Person.Woman => "ButtonWoman",
                    Person.Child => "ButtonChild",
                    Person.Elder => "ButtonElder"
                }, */
            // button.AddToClassList("Person");
            // don't need to add style person because Man, Woman etc. made equivalenrt
            element(PassageDisplay).Add(button);
            button.visible = true;
            button.style.display = DisplayStyle.Flex;
        }

        void addButtonToResourceDisplay(Resource resource)
        {
            var button = makeButton(new ButtonInfo()
            {
                name = $"{resource}",
                text = "",
                style = $"{resource}",
                resource = resource,
                gso = new gso(ButtonResource)
                { setInt = (int)resource, r = resource},
                remove = true
            });
            button.AddToClassList("Resource");
            // need to also add the style "Resource"
            
            element(ResourceDisplay).Add(button);
            button.visible = true;
            button.style.display = DisplayStyle.Flex;
        }

        Button makeButton(ButtonInfo info)
        {
            Button button = info.Make();
            button.RegisterCallback<ClickEvent>((evt) =>
            {
                var clickedButton = evt.target as Button;
                var clickedInfo = (ButtonInfo)clickedButton.userData;
                OnGameStateChanged?.Invoke(this, clickedInfo.gso); 
            } );
            return button;
        }

        // each of these returns the First ve found
        VisualElement element(GameStateTag tag) => statusPanel.ByStringInArea($"{tag}"); // findVisualElement($"{tag}", statusPanel);

        VisualElement element(string s) => statusPanel.ByStringInArea(s); //findVisualElement($"{s}", statusPanel);

        VisualElement element(GameStateTag tag, VisualElement parent) => parent.ByStringInArea($"{tag}"); //findVisualElement($"{tag}", parent);

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
            }*/

        /*      void initializeFunctions()
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
                },
                new(Elder0)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[0] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[0]; }
                },
                new(Elder1)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[1] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[1]; }
                },
                new(Elder2)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[2] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[2]; }
                },
                new(Elder3)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[3] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[3]; }
                },
                new(Elder4)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[4] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[4]; }
                },
                new(Elder5)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[5] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[5]; }
                },
                new(Elder6)
                {
                    callback = new((gsfo obj) => gs.ElderDisplay[6] = obj.setValue),
                    getValue = delegate { return gs.ElderDisplay[6]; }
                }
            };

            foreach (gsfo obj in toplevelObjects)
            {
                obj.ve = element(obj.tag);
                gsfObjects.Add(obj);
            }  

            foreach (Family family in gs.AllFamilies)
            {
                Foldout foldout = (Foldout)element(family.Name.Replace(" ", ""));
                foldout.value = false; // start closed

                gsfo[] familyObjects =
                {
                    /* new(IsActive)
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
                    obj.ve = element(obj.tag, foldout);
                    obj.parent = foldout;
                    gsfObjects.Add(obj);
                }
            }

            // make list of IsActive obj to toggle close/open
            familyActiveCheck = gsfObjects.Where(obj => obj.tag == IsActive).ToList();

            for (int i = 1; i < (int)Territory.Default; i++)
            {
                Territory territory = (Territory)i;
                Foldout foldout = (Foldout)element($"{territory}");
                foldout.value = false; // start closed

                gsfo[] territoryObjects =
                {
                    new(HasDrought)
                    {
                        callback = new((gsfo obj) =>
                            toggleTerritoryValue(gs.HasDrought, obj.t, obj.setBool)),
                        getBool = delegate { return gs.HasDrought.Contains(territory); }
                    }, 
                    new(DroughtNum)
                    {
                        // cast the territory as an int
                        callback = new((gsfo obj) => gs.TerritoryDrought[(int)obj.t] = obj.setValue),
                        getValue = delegate { return gs.TerritoryDrought[(int)territory]; }
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
                    obj.ve = element(obj.tag, foldout);
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
                        // when button pressed use int from setValue as lookup in Person enum
                        var person = (Person)obj.setValue;
                        gs.PersonsInPassage.Add(person);
                        addButtonToPassageDisplay(person);
                    }),
                    ve = statusPanel.Query(className: style).Where(e => e.ClassListContains($"{AddPersonToPassage}")),                    
                };

                if (obj.ve == null)
                { Debug.Log($"No Add Person Button found for {style}"); }
                else obj.ve.userData = (Person)i;

                gsfObjects.Add(obj);
            }

            // Resources box: Horse, Sheep, Corn, TradeGood, Firearm
            for (int i = 0; i < resourceStyles.Count(); i++)
            {
                string style = resourceStyles[i];
                gsfo obj = new(AddResource)
                {
                    callback = new((gsfo obj) =>
                    {
                        // when button pressed use int from setValue as lookup in Resource enum
                        var resource = (Resource)obj.setValue;
                        gs.Resources.Add(resource);
                        addButtonToResourceDisplay(resource);
                    }),
                    ve = statusPanel.Query(className: style).Where(e => e.ClassListContains($"{AddResource}")),
                };

                if (obj.ve == null)
                { Debug.Log($"No Add Resource Button found for {style}"); }
                else obj.ve.userData = (Resource)i;

                gsfObjects.Add(obj);
            }
            
            // Cubes: Black, White, Brown, Red, Yellow, Green, Blue
            foreach (var bowl in Bowls)
            {
                GameStateFunction tag = Default;
                if (bowl == gs.Raided) tag = AddToRaided;
                if (bowl == gs.Recovery) tag = AddToRecovery;
                if (bowl == gs.Subjugation) tag = AddToSubjugation;

                for (int i = 0; i < cubeStyles.Count(); i++)
                {
                    string style = cubeStyles[i];
                    gsfo obj = new(tag)
                    {
                        callback = new((gsfo obj) =>
                        {
                            // when button pressed use int from setValue as lookup in Person enum
                            var cube = (Cube)obj.setValue;
                            bowl.Add(cube);
                            addCubeButtonToBowl(cube, bowl);
                        }),
                        ve = statusPanel.Query(className: style).Where(e => e.ClassListContains($"{tag}")),
                    };

                    if (obj.ve == null)
                    { Debug.Log($"No Add Cube Button found for {style}"); }
                    else obj.ve.userData = (Cube)i;

                    gsfObjects.Add(obj);
                }
            }
            assignCallBacks();
        } */