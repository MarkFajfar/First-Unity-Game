using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavajoWars;

namespace NavajoWars
{

    public enum Territory { SantaFe, Splitrock, SanJuan, Zuni, Monument, Hopi, BlackMesa, Canyon }

    public class GameState : MonoBehaviour
    {

        public class Family
        {
            public string Name { get; set; }
            public bool IsInPlay { get; set; } = false;
            public bool HasMan { get; set; } = true;
            public bool HasWoman { get; set; } = true;
            public bool HasChild { get; set; } = true;

            public Territory IsWhere = Territory.Splitrock;
            public int Ferocity { get; set; } = 0;
        }

    }
}
