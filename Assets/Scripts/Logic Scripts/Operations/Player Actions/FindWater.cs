using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class FindWater : GameStep
    {
        public override string stepName { get => "FindWater"; }

        Family selectedFamily;

        bool testIsEligible(Family f) => f is { HasMan: true, HasWoman: false, Ferocity : 1 or 2 };

        public override void Begin()
        {
            ui.displayHeadline($"{selectedFamily.Name} Finds a New Water Hole");
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            if (!gs.HasDrought.Contains(selectedFamily.IsWhere)) 
            {
                ui.displayText("No Drought where this Family is located. Press Back to choose a different action.");
                ui.OnNextClick = actionComplete;
            }
            else
            {
                bool horse = selectedFamily.HasHorse;
                int missing = OperationsLogic.numMissing(selectedFamily);
                bool isFirstAction = gs.completedActions == 0;
                ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Remove one Drought counter at cost of MPs equal to 9 minus {(horse ? "parenthesized " : "")}value of current Area");
                //ui.addText(horse ? ", or parenthesized value if lower. " : ". ");
                ui.addText("Press Back if insufficient MP, or Next to remove Drought.");
                ui.OnNextClick = removeDrought;
            }
        }

        void removeDrought()
        {
            gm.SaveUndo(this);
            ui.OnNextClick -= removeDrought;
            var territory = selectedFamily.IsWhere;
            int i = (int)territory;
            if (gs.TerritoryDrought[i] > 0) gs.TerritoryDrought[i]--;
            if (gs.TerritoryDrought[i] < 1) gs.HasDrought.Remove(territory);

            ui.displayText(gs.TerritoryDrought[i] switch
            {
                0 => $"Drought in {territory} has been completely removed!",
                1 => $"One Drought counter remains in {territory}.",
                _ => "Check number of Drought counters."
            });

            ui.addText("\nPress Next to continue.");

            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
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
