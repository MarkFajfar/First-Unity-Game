using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PlayerOperation : GameStep
    {
        public override string stepName { get => "PlayerOperation"; }
        
        public override void Begin()
        {
            // need save for undo to reverse changes in next step
            gm.SaveUndo(this);
            ui.showBackNext();
            ui.displayHeadline("Select a Player Operation");
            ui.displayText(""); // leave any text from prior step?
            // add gamestep objects to buttons to call directly 
            ButtonInfo planning = new("Planning", GetComponentInChildren<PlanningOne>(), nextStep);
            ButtonInfo actions = new("Take Actions", GetComponentInChildren<ChooseFamily>(), nextStep);
            ButtonInfo passage = new("Passage of Time", GetComponentInChildren<PassageOneA1>(), nextStep);
            List<ButtonInfo> choices = new() { planning, actions, passage };
            // do not add to stack to come back here until after choice is made
            // so going directly to GameStep does not work
            ui.ShowChoiceButtons(choices);  
        }

        void nextStep(ButtonInfo info) 
        {
            gm.stepStack.Push(this);
            info.gameStep.Begin();
        }

        public override void Undo()
        {
            gm.LoadUndo(this);
            Begin();
        }
    }
}
