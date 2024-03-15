using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NavajoWars.GameStateTag;
using gso = NavajoWars.GameStateObject;

namespace NavajoWars
{
    public class GameStateEvent : MonoBehaviour
    {
        GameManager gm;
        GameState gs;
        delegate void delegateObj(gso obj); 
        delegate int returnInt(gso obj);
        delegate bool returnBool(gso obj);
        Dictionary<GameStateTag, delegateObj> delegates;
        Dictionary<GameStateTag, returnInt> returnInts;
        Dictionary<GameStateTag, returnBool> returnBools;

        List<IChangeGameState> unsubscribe;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();

            delegates = new()
            {
                {AP, new((gso obj) => gs.AP = obj.setInt)},
                {CP, new((gso obj) => gs.CP = obj.setInt)},
                {MP, new((gso obj) => gs.MP = obj.setInt)},
                {EnemyAP, new((gso obj) => gs.EnemyAP = obj.setInt)},
                {Morale, new((gso obj) => gs.Morale = obj.setInt)},
                {EnemyFerocity, new((gso obj) => gs.EnemyFerocity = obj.setInt)},

                {Elder0, new((gso obj) => gs.ElderDisplay[0] = obj.setInt)},
                {Elder1, new((gso obj) => gs.ElderDisplay[1] = obj.setInt)},
                {Elder2, new((gso obj) => gs.ElderDisplay[2] = obj.setInt)},
                {Elder3, new((gso obj) => gs.ElderDisplay[3] = obj.setInt)},
                {Elder4, new((gso obj) => gs.ElderDisplay[4] = obj.setInt)},
                {Elder5, new((gso obj) => gs.ElderDisplay[5] = obj.setInt)},
                {Elder6, new((gso obj) => gs.ElderDisplay[6] = obj.setInt)},

                {IsActive, new((gso obj) => obj.f.IsActive = obj.setBool)},
                {Man, new((gso obj) => obj.f.HasMan = obj.setBool)},
                {Woman, new((gso obj) => obj.f.HasWoman = obj.setBool)},
                {Child, new((gso obj) => obj.f.HasChild = obj.setBool)},
                {Horse, new((gso obj) => obj.f.HasHorse = obj.setBool)},
                //{IsWhere, new((gso obj) => obj.f.IsWhere = gs.Territories[obj.setInt + 1])}, // (eTerritory)obj.setInt + 1)},
                {IsWhere, new((gso obj) =>  obj.f.MoveTo(((eTerritory)obj.setInt + 1).ByTag()) ) }, //(gs.Territories[obj.setInt + 1]))},
                { Ferocity, new((gso obj) => obj.f.Ferocity = obj.setInt)},

                //{HasDrought, new((gso obj) => obj.t.HasDrought = obj.setBool)},// ToggleTerritoryValue(gs.HasDrought, obj.t, obj.setBool))},
                {DroughtNum, new((gso obj) => obj.t.DroughtNum = obj.setInt)},
                
                //{HasCorn, new((gso obj) => obj.t.HasCorn = obj.setBool)},
                {CornNum, new((gso obj) => obj.t.CornNum = obj.setInt)},
                
                {HasMission, new((gso obj) => obj.t.HasMission = obj.setBool)},
                {HasRancho, new((gso obj) => obj.t.HasRancho = obj.setBool)},
                {HasFort, new((gso obj) => obj.t.HasFort = obj.setBool)},

                {AddPersonToPassage, new((gso obj) => gs.PersonsInPassage.Add((Person)obj.setInt))},

                {AddResource, new((gso obj) => gs.Resources.Add((Resource)obj.setInt))},

                {AddToRaided, new((gso obj) => gs.Raided.Add((Cube)obj.setInt))},
                {AddToRecovery, new((gso obj) => gs.Recovery.Add((Cube)obj.setInt))},
                {AddToSubjugation, new((gso obj) => gs.Subjugation.Add((Cube)obj.setInt))},

                {ButtonPassage, new((gso obj) => gs.PersonsInPassage.Remove((Person)obj.setInt))},
                {CubeButtonToBowl, new((gso obj) => obj.bowl.Remove((Cube)obj.setInt))},
                {ButtonResource, new((gso obj) => gs.Resources.Remove((Resource)obj.setInt))},

                {Default, new((gso obj) => Debug.Log($"{obj.tag} not found in Game State delegateObj"))}
            };

            returnInts = new()
            {
                {AP, delegate { return gs.AP; } },
                {CP, delegate { return gs.CP; } },
                {MP, delegate { return gs.MP; } },
                {EnemyAP, delegate {return gs.EnemyAP; } },
                {Morale, delegate { return gs.Morale; } },
                {EnemyFerocity, delegate { return gs.EnemyFerocity; } },

                {Elder0, delegate { return gs.ElderDisplay[0]; } },
                {Elder1, delegate { return gs.ElderDisplay[1]; } },
                {Elder2, delegate { return gs.ElderDisplay[2]; } },
                {Elder3, delegate { return gs.ElderDisplay[3]; } },
                {Elder4, delegate { return gs.ElderDisplay[4]; } },
                {Elder5, delegate { return gs.ElderDisplay[5]; } },
                {Elder6, delegate { return gs.ElderDisplay[6]; } },

                {IsWhere, new((gso obj) =>  { return (int)obj.f.IsWhere.Number; } ) },
                {Ferocity, new((gso obj) =>  { return obj.f.Ferocity; } ) },

                {DroughtNum, new((gso obj) =>  { return obj.t.DroughtNum; } ) },
                {CornNum, new((gso obj) =>  { return obj.t.CornNum; } )},

                {Default, new((gso obj) => { Debug.Log($"{obj.tag} not found in Game State returnInt"); return 0; } ) }
            };

            returnBools = new()
            {
                {IsActive, new((gso obj) =>  { return obj.f.IsActive; } ) },
                {Man, new((gso obj) =>  { return obj.f.HasMan; } ) },
                {Woman, new((gso obj) =>  { return obj.f.HasWoman; } ) },
                {Child, new((gso obj) =>  { return obj.f.HasChild; } ) },
                {Horse, new((gso obj) =>  { return obj.f.HasHorse; } ) },

                {HasDrought, new((gso obj) => { return obj.t.HasDrought; } ) },// gs.HasDrought.Contains(obj.t); } ) },
                {HasCorn, new((gso obj) => { return obj.t.HasCorn; } ) },
                {HasMission, new((gso obj) => { return obj.t.HasMission; } ) },
                {HasRancho, new((gso obj) => { return obj.t.HasRancho; } ) },
                {HasFort, new((gso obj) => { return obj.t.HasFort; } ) },

                {Default, new((gso obj) => { Debug.Log($"{obj.tag} not found in Game State returnBool"); return false; } ) }
            };          
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            //requires that GameStateUI script be attached to the UI object in the scene
            //if (scene.name == "MainMenu") return;
            unsubscribe = new();
            var uiobj = GameObject.FindWithTag("UI");
            foreach (var script in uiobj.GetComponentsInChildren<IChangeGameState>())
            {
                print($"subsribing to {script}");
                script.OnGameStateChanged += onGameStateChanged;
                unsubscribe.Add(script);
            }
        }

        void OnSceneUnload(Scene scene)
        {
            foreach (var script in unsubscribe)
            {
                script.OnGameStateChanged -= onGameStateChanged;
            }
            unsubscribe = null;
        }

        void onGameStateChanged(object s, gso obj) 
        {
            delegateObj todo = delegates[obj.tag] ?? delegates[Default];
            todo.Invoke(obj);
        }

        public int ReturnInt(gso obj)
        {
            returnInt todo = returnInts[obj.tag] ?? returnInts[Default];
            return todo.Invoke(obj);
        }

        public bool ReturnBool(gso obj)
        {
            returnBool todo = returnBools[obj.tag] ?? returnBools[Default];
            return todo.Invoke(obj);
        }

        public static void ToggleTerritoryValue(List<eTerritory> list, eTerritory t, bool b)
        {
            if (b && !list.Contains(t)) list.Add(t);
            if (!b) list.Remove(t); // "else" would remove whenever list.Contains(t)
        }
    }
}
