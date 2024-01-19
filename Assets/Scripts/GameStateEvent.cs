using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static NavajoWars.GameStateFunction;
using gsfo = NavajoWars.GameStateUI.GameStateFunctionObject;

namespace NavajoWars
{
    public class GameStateEvent : MonoBehaviour
    {
        //PropertyInfo[] Properties;
        //Type gsType = typeof(GameState);

        GameManager gm;
        GameState gs;
        delegate void delegateObj(gsfo obj); 
        delegate int delegateInt(gsfo obj);
        delegate bool delegateBool(gsfo obj);
        Dictionary<GameStateFunction, delegateObj> delegates;
        Dictionary<GameStateFunction, delegateInt> delegateInts;
        Dictionary<GameStateFunction, delegateBool> delegateBools;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();


            //Bowls = new List<List<Cube>> { gs.Raided, gs.Recovery, gs.Subjugation };

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
            };

            delegateInts = new()
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
                {Ferocity, new((gsfo obj) =>  { return obj.f.Ferocity; } ) }
            };

            delegateBools = new()
            {
                {IsActive, new((gsfo obj) =>  { return obj.f.IsActive; } ) },
                {Man, new((gsfo obj) =>  { return obj.f.HasMan; } ) },
                {Woman, new((gsfo obj) =>  { return obj.f.HasWoman; } ) },
                {Child, new((gsfo obj) =>  { return obj.f.HasChild; } ) },
                {Horse, new((gsfo obj) =>  { return obj.f.HasHorse; } ) },
            };          
        }

        public void OnGameStateChanged(gsfo obj)
        {
            delegates[obj.tag]?.Invoke(obj);
        }

        public int getInt(gsfo obj)
        {
            // add try catch for null check
            return delegateInts[obj.tag].Invoke(obj);
        }

        public bool getBool(gsfo obj)
        {
            return delegateBools[obj.tag].Invoke(obj);
        }
    }
}
