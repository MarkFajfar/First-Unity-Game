using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Plant : GameStep
    {
        public override string stepName { get => "Plant"; }

        public override void Begin()
        {
            // add
            
            bool drought = gs.HasDrought.Contains(gs.selectedFamily.IsWhere);
            bool rancho = gs.HasRancho.Contains(gs.selectedFamily.IsWhere);
            int missing = numMissing(gs.selectedFamily);
            bool isFirstAction = gs.completedActions.Count() == 0;
            ui.displayHeadline($"{gs.selectedFamily.Name} Plants or Harvests");
            ui.displayText($"{gs.selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ");
            ui.addText((drought || rancho) ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ");
            ui.addText("To harvest, reveal Corn counter and place it in resources. Press Next to continue.");
            ui.OnOpsNextClick += actionComplete;
            // NECESSARY TO RECORD PLANT OR HARVEST?
        }

        void actionComplete()
        {
            ui.OnOpsNextClick -= actionComplete;
            gs.completedActions.Add(this);
            logic.instructFromStep(this, "ChooseAnotherAction");
        }

        public override void Undo()
        {
            
            // stuff to do on undo
            
        }

        int numMissing(GameState.Family family)
        {
            int missing = 0;
            if (!family.HasMan) missing++;
            if (!family.HasWoman) missing++;
            if (!family.HasChild) missing++;
            return missing;
        }
    }
}
