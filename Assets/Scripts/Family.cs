using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    [Serializable]
    public class Family
    {
        public string Name { get => name; set => name = value; }
        [SerializeField] string name;
        public bool IsActive 
        { 
            get => isActive; 
            set 
            {                 
                isActive = value;
                if (value == false)
                {
                    HasMan = false;
                    HasWoman = false;
                    HasChild = false;
                    HasHorse = false;
                    IsWhere = null;
                    Ferocity = 0;
                } 
            }
        }
        [SerializeField] bool isActive = false;
        public bool HasMan { get => hasMan; set => hasMan = value; }
        [SerializeField] bool hasMan = false;
        public bool HasWoman { get => hasWoman; set => hasWoman = value; }
        [SerializeField] bool hasWoman = false;
        public bool HasChild { get => hasChild; set => hasChild = value; }
        [SerializeField] bool hasChild = false;
        public bool HasHorse { get => hasHorse; set => hasHorse = value; }
        [SerializeField] bool hasHorse = false;

        public Territory IsWhere { get => isWhere; set => isWhere = value; }
        [SerializeField] Territory isWhere;

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

        /// <summary>
        /// Sets family's .IsWhere to the Territory and adds family's Name to its list; removes family's Name from prior Territory's list
        /// </summary>
        public void MoveTo(Territory territory)
        {
            if (IsWhere != null) IsWhere.Families.Remove(Name);
            IsWhere = territory;
            IsWhere.Families.Add(Name);
        }
    }
}
