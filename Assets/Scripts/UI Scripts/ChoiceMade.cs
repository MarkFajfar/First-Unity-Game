using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class ChoiceMade : EventArgs
    {
        public ChoiceMade(int choiceIndex, string choiceText)
        {
            ChoiceText = choiceText;
            ChoiceIndex = choiceIndex;
        }
        public string ChoiceText { get; set; }
        public int ChoiceIndex { get; set; }

        /*public delegate void ChoiceMadeEventHandler(object sender, ChoiceMade args);
        public static event ChoiceMadeEventHandler ChoiceMadeEvent;

        public void OnChoiceMade(ChoiceMade e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeEventHandler raiseEvent = ChoiceMadeEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }*/
    }

    public class ChoiceMadeObject : EventArgs
    {
        public ChoiceMadeObject(GameStep gamestep)
        {
            cGameStep = gamestep;
        }
        public GameStep cGameStep { get; set; }
    
        /*public delegate void ChoiceMadeObjectEventHandler(object sender, ChoiceMadeObject args);
        public static event ChoiceMadeObjectEventHandler ChoiceMadeObjectEvent;

        public void OnChoiceMadeObject(ChoiceMadeObject e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeObjectEventHandler raiseEvent = ChoiceMadeObjectEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }*/
    }
}
