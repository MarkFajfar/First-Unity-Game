using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
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

        public delegate void ChoiceMadeEventHandler(object sender, ChoiceMade args);
        public static event ChoiceMadeEventHandler ChoiceMadeEvent;

        public void OnChoiceMade(ChoiceMade e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeEventHandler raiseEvent = ChoiceMadeEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }
    }
    public class ChoiceMadeString : EventArgs
    {
        public ChoiceMadeString(string choiceText)
        {
            ChoiceText = choiceText;
        }
        public string ChoiceText { get; set; }

        public delegate void ChoiceMadeStringEventHandler(object sender, ChoiceMadeString args);
        public static event ChoiceMadeStringEventHandler ChoiceMadeStringEvent;

        public void OnChoiceMadeString(ChoiceMadeString e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeStringEventHandler raiseEvent = ChoiceMadeStringEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }
    }

    public class ChoiceMadeParams : EventArgs
    {
        public ChoiceMadeParams(ButtonInfo clickedParams)
        {
            cParams = clickedParams;
        }
        public ButtonInfo cParams { get; set; }

        public delegate void ChoiceMadeParamsEventHandler(object sender, ChoiceMadeParams args);
        public static event ChoiceMadeParamsEventHandler ChoiceMadeParamsEvent;

        public void OnChoiceMadeParams(ChoiceMadeParams e)
        {
            ChoiceMadeParamsEvent?.Invoke(this, e);
            //ChoiceMadeParamsEventHandler raiseEvent = ChoiceMadeParamsEvent;
            //if (raiseEvent != null) raiseEvent(this, e);
        }

        public static void unsubChoiceMadeParams()
        {
            if (ChoiceMadeParamsEvent != null)
            {
                //foreach (UIScript.ClickBack subscriber in OnOpsBackClick.GetInvocationList().Cast<UIScript.ClickBack>())
                foreach (ChoiceMadeParamsEventHandler subscriber in ChoiceMadeParamsEvent.GetInvocationList().Cast<ChoiceMadeParamsEventHandler>())
                { ChoiceMadeParamsEvent -= subscriber; }
            }
        }
    }

    public class ChoiceMadeGameStep : EventArgs
    {
        public ChoiceMadeGameStep(GameStep gamestep)
        {
            cGameStep = gamestep;
        }
        public GameStep cGameStep { get; set; }

        public delegate void ChoiceMadeGameStepEventHandler(object sender, ChoiceMadeGameStep args);
        public static event ChoiceMadeGameStepEventHandler ChoiceMadeGameStepEvent;

        public void OnChoiceMadeGameStep(ChoiceMadeGameStep e)
        {
            ChoiceMadeGameStepEventHandler raiseEvent = ChoiceMadeGameStepEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        public static void unsubChoiceMadeGameStep()
        {
            if (ChoiceMadeGameStepEvent != null)
            {
                //foreach (UIScript.ClickBack subscriber in OnOpsBackClick.GetInvocationList().Cast<UIScript.ClickBack>())
                foreach (ChoiceMadeGameStepEventHandler subscriber in ChoiceMadeGameStepEvent.GetInvocationList().Cast<ChoiceMadeGameStepEventHandler>())
                { ChoiceMadeGameStepEvent -= subscriber; }
            }
        }
    }
}
