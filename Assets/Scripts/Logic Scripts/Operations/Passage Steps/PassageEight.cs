using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PassageEight : GameStep
    {
        public override string stepName { get => "PassageEight"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Eight");

            if (gs.HasDrought.Count() < 1)
            {
                ui.displayText("No Territories have Drought. ");
                ui.addText("Press Next to continue.");
                ui.OnNextClick = actionComplete;
            }
            else droughtCheck();
        }

        void droughtCheck()
        {
            
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageNine >().Begin();
        }

        public override void Undo()
        {
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            gm.LoadUndo(this);
            Begin();
        }
    }
}
