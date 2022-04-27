using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using gs = NavajoWars.GameState;

namespace NavajoWars
{
    public class Scenario : ScriptableObject
    {
        public string Name;
        public string LongTitle;
        public int AP;
        public int CP;
        public int MP;
        public int Ferocity;
        public int Morale;

        public bool IsActiveFamilyA;
        public bool IsActiveFamilyB;
        public bool IsActiveFamilyC;
        public bool IsActiveFamilyD;
        public bool IsActiveFamilyE;
        public bool IsActiveFamilyF;

        public string[] Setup;
    }
}
