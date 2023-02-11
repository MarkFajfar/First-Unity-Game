using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningSeven : GameStep
    {
        public override string stepName { get => "PlanningSeven"; }

        public override void Begin()
        {
            
            gm.SaveUndo(this);
            ui.displayHeadline("Planning Complete");
            ui.displayText($"Press Next to continue.");
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            logic.instructFromStep(this, "PlayerOpsDone");
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
