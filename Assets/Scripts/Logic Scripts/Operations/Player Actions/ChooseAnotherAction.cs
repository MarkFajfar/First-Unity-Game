using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class ChooseAnotherAction : GameStep
    {
        public override string stepName { get => "ChooseAnotherAction"; }

        public override void Begin()
        {
            GameStep caller = gs.stepStack.Peek();
            gs.completedActions.Add(caller);
            // or add just before calling this step
            chooseAnotherAction();
        }

        async void chooseAnotherAction()
        {
            ui.addText("If family has MPs remaining, you may select another Action. Otherwise, you may select another family to activate (or choose to end family actions). ");
            bParams same = new("Action for Same Family");
            bParams end = new("Another Family or End");
            List<bParams> choices = new() { same, end };
            ui.DisplayChoiceButtonsEvent(choices);
            (int index, string text) result = await IReceive.GetChoiceAsync(choices);
            if (result.text == same.name)
            {
                ChooseAction chooseAction = GetComponent<ChooseAction>();
                // push to stepStack immediately before calling enxt action
                gs.stepStack.Push(this);
                chooseAction.Begin();
            }
            if (result.text == end.name)
            {
                gs.completedFamilies.Add(gs.selectedFamily);
                gs.completedActions = new List<GameStep>();
                ChooseFamily chooseFamily = GetComponent<ChooseFamily>();
                // push to stepStack immediately before calling enxt action
                gs.stepStack.Push(this);
                chooseFamily.Begin();
            }
        }

        public override void Undo()
        {
            chooseAnotherAction();
        }
    }
}
