using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NavajoWars.GameStateFunction;
using gsfo = NavajoWars.GameStateFunctionObject;

namespace NavajoWars
{
    public class GameStateEvent : MonoBehaviour
    {
        //PropertyInfo[] Properties;
        //Type gsType = typeof(GameState);

        GameManager gm;
        GameState gs;
        delegate void delegateObj(gsfo obj); 
        delegate int returnInt(gsfo obj);
        delegate bool returnBool(gsfo obj);
        Dictionary<GameStateFunction, delegateObj> delegates;
        Dictionary<GameStateFunction, returnInt> returnInts;
        Dictionary<GameStateFunction, returnBool> returnBools;

        List<IChangeGameState> unsubscribe;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();

            //Properties[0] = gsType.GetProperty("AP");
            //Properties[0].SetValue(null, 3);

            delegates = new()
            {
                {AP, new((gsfo obj) => gs.AP = obj.setValue)},
                {CP, new((gsfo obj) => gs.CP = obj.setValue)},
                {MP, new((gsfo obj) => gs.MP = obj.setValue)},
                {Morale, new((gsfo obj) => gs.Morale = obj.setValue)},
                {EnemyFerocity, new((gsfo obj) => gs.EnemyFerocity = obj.setValue)},

                {Elder0, new((gsfo obj) => gs.ElderDisplay[0] = obj.setValue)},
                {Elder1, new((gsfo obj) => gs.ElderDisplay[1] = obj.setValue)},
                {Elder2, new((gsfo obj) => gs.ElderDisplay[2] = obj.setValue)},
                {Elder3, new((gsfo obj) => gs.ElderDisplay[3] = obj.setValue)},
                {Elder4, new((gsfo obj) => gs.ElderDisplay[4] = obj.setValue)},
                {Elder5, new((gsfo obj) => gs.ElderDisplay[5] = obj.setValue)},
                {Elder6, new((gsfo obj) => gs.ElderDisplay[6] = obj.setValue)},

                {IsActive, new((gsfo obj) => obj.f.IsActive = obj.setBool)},
                {Man, new((gsfo obj) => obj.f.HasMan = obj.setBool)},
                {Woman, new((gsfo obj) => obj.f.HasWoman = obj.setBool)},
                {Child, new((gsfo obj) => obj.f.HasChild = obj.setBool)},
                {Horse, new((gsfo obj) => obj.f.HasHorse = obj.setBool)},
                {IsWhere, new((gsfo obj) => obj.f.IsWhere = (Territory)obj.setValue + 1)},
                {Ferocity, new((gsfo obj) => obj.f.Ferocity = obj.setValue)},

                {HasDrought, new((gsfo obj) => ToggleTerritoryValue(gs.HasDrought, obj.t, obj.setBool))},
                {DroughtNum, new((gsfo obj) => gs.TerritoryDrought[(int)obj.t] = obj.setValue)},
                {HasCorn, new((gsfo obj) => ToggleTerritoryValue(gs.HasCorn, obj.t, obj.setBool))},
                {HasMission, new((gsfo obj) => ToggleTerritoryValue(gs.HasMission, obj.t, obj.setBool))},
                {HasRancho, new((gsfo obj) => ToggleTerritoryValue(gs.HasRancho, obj.t, obj.setBool))},
                {HasFort, new((gsfo obj) => ToggleTerritoryValue(gs.HasFort, obj.t, obj.setBool))},

                {AddPersonToPassage, new((gsfo obj) => gs.PersonsInPassage.Add((Person)obj.setValue))},

                {AddResource, new((gsfo obj) => gs.Resources.Add((Resource)obj.setValue))},

                {AddToRaided, new((gsfo obj) => gs.Raided.Add((Cube)obj.setValue))},
                {AddToRecovery, new((gsfo obj) => gs.Recovery.Add((Cube)obj.setValue))},
                {AddToSubjugation, new((gsfo obj) => gs.Subjugation.Add((Cube)obj.setValue))},

                {ButtonPassage, new((gsfo obj) => gs.PersonsInPassage.Remove((Person)obj.setValue))},
                {CubeButtonToBowl, new((gsfo obj) => obj.bowl.Remove((Cube)obj.setValue))},
                {ButtonResource, new((gsfo obj) => gs.Resources.Remove((Resource)obj.setValue))},

                {Default, new((gsfo obj) => Debug.Log($"{obj.tag} not found in Game State delegateObj"))}
            };

            returnInts = new()
            {
                {AP, delegate { return gs.AP; } },
                {CP, delegate { return gs.CP; } },
                {MP, delegate { return gs.MP; } },
                {Morale, delegate { return gs.Morale; } },
                {EnemyFerocity, delegate { return gs.EnemyFerocity; } },

                {Elder0, delegate { return gs.ElderDisplay[0]; } },
                {Elder1, delegate { return gs.ElderDisplay[1]; } },
                {Elder2, delegate { return gs.ElderDisplay[2]; } },
                {Elder3, delegate { return gs.ElderDisplay[3]; } },
                {Elder4, delegate { return gs.ElderDisplay[4]; } },
                {Elder5, delegate { return gs.ElderDisplay[5]; } },
                {Elder6, delegate { return gs.ElderDisplay[6]; } },

                {IsWhere, new((gsfo obj) =>  { return (int)obj.f.IsWhere; } ) },
                {Ferocity, new((gsfo obj) =>  { return obj.f.Ferocity; } ) },

                {DroughtNum, new((gsfo obj) =>  { return gs.TerritoryDrought[(int)obj.t]; } ) },

                {Default, new((gsfo obj) => { Debug.Log($"{obj.tag} not found in Game State returnInt"); return 0; } ) }
            };

            returnBools = new()
            {
                {IsActive, new((gsfo obj) =>  { return obj.f.IsActive; } ) },
                {Man, new((gsfo obj) =>  { return obj.f.HasMan; } ) },
                {Woman, new((gsfo obj) =>  { return obj.f.HasWoman; } ) },
                {Child, new((gsfo obj) =>  { return obj.f.HasChild; } ) },
                {Horse, new((gsfo obj) =>  { return obj.f.HasHorse; } ) },

                {HasDrought, new((gsfo obj) => { return gs.HasDrought.Contains(obj.t); } ) },
                {HasCorn, new((gsfo obj) => { return gs.HasCorn.Contains(obj.t); } ) },
                {HasMission, new((gsfo obj) => { return gs.HasMission.Contains(obj.t); } ) },
                {HasRancho, new((gsfo obj) => { return gs.HasRancho.Contains(obj.t); } ) },
                {HasFort, new((gsfo obj) => { return gs.HasFort.Contains(obj.t); } ) },

                {Default, new((gsfo obj) => { Debug.Log($"{obj.tag} not found in Game State returnBool"); return false; } ) }
            };          
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            // requires that GameStateUI script be attached to the UI object in the scene
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

        void onGameStateChanged(object s, gsfo obj) 
        {
            print("Invoking Delegate");
            delegateObj todo = delegates[obj.tag] ?? delegates[Default];
            todo.Invoke(obj);
        }

        public int ReturnInt(gsfo obj)
        {
            returnInt todo = returnInts[obj.tag] ?? returnInts[Default];
            return todo.Invoke(obj);
        }

        public bool ReturnBool(gsfo obj)
        {
            returnBool todo = returnBools[obj.tag] ?? returnBools[Default];
            return todo.Invoke(obj);
        }

        public static void ToggleTerritoryValue(List<Territory> list, Territory t, bool b)
        {
            if (b && !list.Contains(t)) list.Add(t);
            if (!b) list.Remove(t); // "else" would remove whenever list.Contains(t)
        }
    }
}
