using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class FindWater : GameStep
    {
        public override string stepName { get => "FindWater"; }

        GameState.Family selectedFamily;

        public override void Begin()
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            bool horse = selectedFamily.HasHorse;
            int missing = OperationsLogic.numMissing(selectedFamily);
            bool isFirstAction = gs.completedActions == 0;
            ui.displayHeadline($"{selectedFamily.Name} Finds a New Water Hole");
            ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Remove one Drought counter at cost of MPs equal to 9 minus value of current Area");
            ui.addText(horse ? ", or parenthesized value if lower. " : ". ");
            ui.addText("Press Back if insufficient MP, or Next to remove Drought.");
            ui.OnNextClick += removeDrought;
        }

        void removeDrought()
        {
            gm.SaveUndo(this);
            ui.OnNextClick -= removeDrought;
            gs.HasDrought.Remove(selectedFamily.IsWhere);
            actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponentInChildren<ChooseAnotherAction>();
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
            Begin();
            // stuff to do on undo
        }
    }
}
