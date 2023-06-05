using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class BowlStructs 
    {
        [Serializable]
        public struct raided
        {
            public int Black
            { get => black; set => black = Math.Clamp(value, 0, 3); }
            [SerializeField] int black;
            public int White
            { get => white; set => white = Math.Clamp(value, 0, 4); }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = Math.Clamp(value, 0, 4); }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = Math.Clamp(value, 0, 2); }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = Math.Clamp(value, 0, 3); }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = Math.Clamp(value, 0, 3); }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = Math.Clamp(value, 0, 1); }
            [SerializeField] int blue;
        }

        [Serializable]
        public struct recovery
        {
            public int Black
            { get => black; set => black = Math.Clamp(value, 0, 3); }
            [SerializeField] int black;
            public int White
            { get => white; set => white = Math.Clamp(value, 0, 4); }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = Math.Clamp(value, 0, 4); }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = Math.Clamp(value, 0, 2); }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = Math.Clamp(value, 0, 3); }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = Math.Clamp(value, 0, 3); }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = Math.Clamp(value, 0, 1); }
            [SerializeField] int blue;
        }

        [Serializable]
        public struct subjugation
        {
            public int Black
            { get => black; set => black = Math.Clamp(value, 0, 3); }
            [SerializeField] int black;
            public int White
            { get => white; set => white = Math.Clamp(value, 0, 4); }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = Math.Clamp(value, 0, 4); }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = Math.Clamp(value, 0, 2); }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = Math.Clamp(value, 0, 3); }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = Math.Clamp(value, 0, 3); }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = Math.Clamp(value, 0, 1); }
            [SerializeField] int blue;
        }

        public raided RaidedStruct;
        public recovery RecoveryStruct;
        public subjugation SubjugationStruct;


    }
}
