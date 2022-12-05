using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{    
    public interface IChoose 
    {
        public delegate void ChoiceMadeEventHandler(object sender, ChoiceMade args);
        public event ChoiceMadeEventHandler ChoiceMadeEvent;

        public delegate void ChoiceMadeObjectEventHandler(object sender, ChoiceMadeObject args);
        public event ChoiceMadeObjectEventHandler ChoiceMadeObjectEvent;
    }
}
