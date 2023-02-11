using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class TribalCouncil : GameStep
    {
        public override string stepName { get => "TribalCouncil"; }

        GameState.Family selectedFamily;

        public override void Begin()
        {
            gm.SaveUndo(this);
            askDieRoll();
        }

        async void askDieRoll()
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();

            ui.displayHeadline($"{selectedFamily.Name} Calls Tribal Council");
            ui.displayText($"This must be {selectedFamily.Name}'s only action. If die roll is > = the family's current Area, collect 1 AP. Otherwise there is no effect. Was the die roll successful?");

            bParams yes = new("Yes Successful");
            bParams no = new("Not Successful");
            List<bParams> choices = new() { yes, no };
            // use async because logic to apply after choice made
            ui.MakeChoiceButtonsAsync(choices);
            bParams result = await IReceive.GetChoiceAsync();
            ui.displayText("");
            if (result.name == yes.name)
            {
                gs.AP++;
                ui.addText("One AP added. ");
            }
            ui.addText("Press Next to Continue. ");
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponent<ChooseAnotherAction>();
            chooseAnotherAction.Begin();
        }

        public override void Undo()
        {
            // reset complete marker??
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            gm.LoadUndo(this);
            askDieRoll();
            // stuff to do on undo
        }
    }
}
