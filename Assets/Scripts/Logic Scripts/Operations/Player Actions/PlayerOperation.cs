using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PlayerOperation : GameStep
    {
        public override string stepName { get => "PlayerOperation"; }

        ChooseFamily chooseFamily;
        PlanningOne planningOne;
        // add for Passage;
        
        public override async void Begin()
        {
            chooseFamily = GetComponent<ChooseFamily>();
            planningOne = GetComponent<PlanningOne>();
            // no need for save because no changes made?
            ui.showBackNext();
            ui.displayHeadline("Select a Player Operation");
            // ui.displayText(""); // leave any text from prior step
            // add gamestep objects to buttons to call directly 
            ButtonInfo planning = new("Planning", planningOne);
            ButtonInfo actions = new("Take Actions", chooseFamily);
            ButtonInfo passage = new("Passage of Time");
            List<ButtonInfo> choices = new() { planning, actions, passage };
            // do not add to stack to come back here until after choice is made
            // so going directly to GameStep does not work?
            //gs.stepStack.Push(this);
            //ui.MakeChoiceButtons(choices);
            ui.MakeChoiceButtonsAsync(choices);
            ButtonInfo result = await IReceive.GetChoiceAsync();
            gs.stepStack.Push(this);
            result.gameStep.Begin();
        }

        public override void Undo()
        {
            // same whether or not called from Undo?
            Begin();
        }
    }
}
