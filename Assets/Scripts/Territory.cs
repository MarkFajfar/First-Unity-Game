using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public enum eTerritory { SantaFe, Splitrock, SanJuan, Zuni, Monument, Hopi, BlackMesa, Canyon, Default }
    
    [Serializable]
    public class Territory
    {
        readonly string [] names = { "Santa Fe", "Splitrock", "San Juan", "Zuni", "Monument", "Hopi", "Black Mesa", "Canyon de Chelly", "Default" };
         
        public string Name { get => name; private set => name = value; }
        [SerializeField] string name;

        public eTerritory Tag { get => tag; private set => tag = value; }
        [SerializeField] eTerritory tag;

        public Territory(int num) 
        {
            Number = num;
            Name = names[num];
            Tag = (eTerritory)num;
            Families = new();
        }

        public int Number { get => number; private set => number = value; }
        [SerializeField] int number;

        public List<string> Families;
        // do not nest Family objects in Territory object because a Territory object is  nested in each Family object

        public int DroughtNum { get => droughtNum; set => droughtNum = Math.Clamp(value, 0, 2); }
        [SerializeField] int droughtNum = 0;

        public bool HasDrought 
        { 
            get 
            {
                if (droughtNum > 0) return true;
                else return false;
            }
            /* set
            {
                // can this be set?
                if (value == true) droughtNum++;
                else droughtNum--;
            } */
        }
        //[SerializeField] bool hasDrought = false;

        public int CornNum { get => cornNum; set => cornNum = Math.Clamp(value, 0, 6); }
        [SerializeField] int cornNum = 0;

        public bool HasCorn
        {
            get
            {
                if (cornNum > 0) return true;
                else return false;
            }
            /* set
            {
                // can this be set?
                if (value == true) droughtNum++;
                else droughtNum--;
            } */
        }
        //[SerializeField] bool hasCorn;

        public bool HasMission { get => hasMission; set => hasMission = value; }
        [SerializeField] bool hasMission;

        public bool HasRancho { get => hasRancho; set => hasRancho = value; }
        [SerializeField] bool hasRancho;

        public bool HasFort { get => hasFort; set => hasFort = value; }
        [SerializeField] bool hasFort;

        public bool HasFamily(string name)
        {
            return Families.Contains(name);
        }
    }
}
