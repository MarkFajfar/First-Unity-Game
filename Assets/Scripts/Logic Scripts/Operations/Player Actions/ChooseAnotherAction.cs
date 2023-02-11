using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class ChooseAnotherAction : GameStep
    {
        public override string stepName { get => "ChooseAnotherAction"; }
        
        GameState.Family selectedFamily;

        public override void Begin()
        {
            GameStep caller = null;
            if (gs.stepStack.Count > 0) caller = gs.stepStack.Peek();
            if (caller != null && (caller.stepName == "Trade" || caller.stepName == "TribalCouncil"))
            {
                // these have to be last action so go to end
                endFamily();
            }
            else chooseAnotherAction();
        }

        void chooseAnotherAction()
        {
            ui.displayText("If family has MPs remaining, you may select another Action. Otherwise, you may select another family to activate (or choose to end family actions). ");
            bParams same = new("Action for Same Family", sameFamily);
            bParams end = new("Another Family or End", endFamily);
            List<bParams> choices = new() { same, end };
            ui.MakeChoiceButtons(choices);
        }

        void sameFamily() 
        {
            ChooseAction chooseAction = GetComponent<ChooseAction>();
            // push to stepStack immediately before calling next action
            gs.stepStack.Push(this);
            chooseAction.Begin();
        }

        void endFamily()
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            selectedFamily.isCompletedOps = true;
            gs.completedFamilies++;
            gs.completedActions = 0;
            ChooseFamily chooseFamily = GetComponent<ChooseFamily>();
            // push to stepStack immediately before calling next action
            gs.stepStack.Push(this);
            chooseFamily.Begin();
        }

        public override void Undo()
        {
            chooseAnotherAction();
        }
    }
}
