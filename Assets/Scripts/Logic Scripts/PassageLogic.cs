using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PassageLogic : MonoBehaviour, IReceive
    {
        GameManager gm;
        GameState gs;
        OperationsUIScript ui;
        ChoiceUIScript choice;
        ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
        List<Action> passageSteps;
        List<bool> stepDone;


        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            ui = gameObject.GetComponent<OperationsUIScript>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
        }
        void Start()
        {
            passageSteps = new();
            passageSteps.Add(clickedPassage);
            passageSteps.Add(StepOne);
            stepDone = new() { false, false, false, false, false, false };
        }

        public void clickedPassage()
        {
            ui.InitializePlanning();
            stepDone[0] = true;
            StepOne();
        }

        public void doStep(int passageStepNum) => passageSteps[passageStepNum]();

        void StepOne()
        {
        }
    }
}
