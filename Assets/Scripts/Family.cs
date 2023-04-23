using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    [Serializable]
    public class Family
    {
        public string Name { get => name; set => name = value; }
        [SerializeField] string name;
        public bool IsActive { get => isActive; set => isActive = value; }
        [SerializeField] bool isActive = false;
        public bool HasMan { get => hasMan; set => hasMan = value; }
        [SerializeField] bool hasMan = true;
        public bool HasWoman { get => hasWoman; set => hasWoman = value; }
        [SerializeField] bool hasWoman = true;
        public bool HasChild { get => hasChild; set => hasChild = value; }
        [SerializeField] bool hasChild = true;
        public bool HasHorse { get => hasHorse; set => hasHorse = value; }
        [SerializeField] bool hasHorse = false;
        public Territory IsWhere { get => isWhere; set => isWhere = value; }
        [SerializeField] Territory isWhere = Territory.Splitrock;
        public int Ferocity { get => ferocity; set => ferocity = value; }
        [SerializeField] int ferocity = 0;

        int[] evReference = { 2, 1, 0, -1 };
        public int Evasion
        { get { return evReference[Ferocity]; } }
        //public int Evasion { get => evasion; set => evasion = value; }
        //[SerializeField] int evasion = 0;

        public bool isSelectedOps { get => isselectedops; set => isselectedops = value; }
        [SerializeField] bool isselectedops = false;

        public bool isCompletedOps { get => iscompletedops; set => iscompletedops = value; }
        [SerializeField] bool iscompletedops = false;
    }
}
