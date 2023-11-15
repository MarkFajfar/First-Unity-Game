using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PassageTen : GameStep
    {
        public override string stepName { get => "PassageFour"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Three");

            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            //GetComponentInChildren<PassageFour>().Begin();
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
