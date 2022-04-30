using UnityEngine;

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

        [TextArea (0,10)]
        public string[] Setup;
    }
}
