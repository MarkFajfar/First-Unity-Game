using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Plant : GameStep
    {
        public override string stepName { get => "Plant"; }

        Family selectedFamily;

        public override void Begin()
        {
            // add?
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();

            bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere);
            bool rancho = gs.HasRancho.Contains(selectedFamily.IsWhere);
            int missing = OperationsLogic.numMissing(selectedFamily);
            ui.displayHeadline($"{selectedFamily.Name} Plants or Harvests");
            ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(gs.completedActions == 0 ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ");
            ui.addText((drought || rancho) ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ");
            ui.addText("To harvest, reveal Corn counter and place it in resources. Press Next to continue.");
            ui.OnNextClick += actionComplete;
            // NECESSARY TO RECORD PLANT OR HARVEST?
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
                gs.completedActions --; 
            }
            Begin();
            // stuff to do on undo
        }
    }
}
