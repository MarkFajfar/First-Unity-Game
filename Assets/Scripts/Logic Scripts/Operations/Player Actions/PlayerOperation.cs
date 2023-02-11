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
        
        public override void Begin()
        {
            chooseFamily = GetComponent<ChooseFamily>();
            planningOne = GetComponent<PlanningOne>();
            // no need for save because no changes made?
            ui.showBackNext();
            ui.displayHeadline("Select a Player Operation");
            ui.displayText("");
            // add gamestep objects to buttons to call directly 
            bParams planning = new("Planning", planningOne);
            bParams actions = new("Take Actions", chooseFamily);
            bParams passage = new("Passage of Time");
            List<bParams> choices = new() { planning, actions, passage };
            // add to stack to come back here
            gs.stepStack.Push(this);
            ui.MakeChoiceButtons(choices);
            //ui.MakeChoiceButtonsAsync(choices);
            //bParams result = await IReceive.GetChoiceAsync();
            //result.gameStep.Begin();
            // not async, so button click goes directly to that gameStep
        }

        public override void Undo()
        {
            // same whether or not called from Undo?
            Begin();
        }
    }
}
