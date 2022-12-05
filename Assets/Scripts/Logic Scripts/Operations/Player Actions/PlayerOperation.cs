using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PlayerOperation : GameStep
    {
        public override string stepName { get => "PlayerOperation"; }

        ChooseFamily chooseFamily;
        // add for Planning and Passage;

        public override async void Begin()
        {
            chooseFamily = GetComponent<ChooseFamily>();
            // no need for save because no changes made?
            ui.showBackNext();
            ui.displayHeadline("Select a Player Operation");
            ui.displayText("");
            // add gamestep objects to buttons to call directly 
            bParams planning = new("Planning");
            bParams actions = new("Take Actions", chooseFamily);
            bParams passage = new("Passage of Time");
            List<bParams> choices = new() { planning, actions, passage };
            ui.DisplayChoiceButtonsEvent(choices);
            GameStep result = await IReceive.GetChoiceAsyncObject(choices);
            // after button clicked, change back to come here
            // but will be superseded if there is any step in the stack 
            gs.stepStack.Push(this);
            result.Begin();
        }

        public override void Undo()
        {
            // same whether or not called from Undo?
            Begin();
        }
    }
}
