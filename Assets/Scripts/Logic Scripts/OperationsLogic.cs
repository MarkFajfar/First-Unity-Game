using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class OperationsLogic : MonoBehaviour, IReceive
    {
        protected GameManager gm;
        protected GameState gs;
        protected OperationsUIScript ui;
        protected ChoiceUIScript choice;
        protected ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
        protected List<Action> Steps;
        protected List<bool> stepDone;

        protected void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            ui = gameObject.GetComponent<OperationsUIScript>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
        }

        protected void doStep(int StepNum) => Steps[StepNum]();

    }
}
