using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningTwo : GameStep
    {
        public override string stepName { get => "PlanningTwo"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Two");
            ui.displayText("Advance each Elder one box to the right. Elders in the right-most box stay put.");
            ui.addText($"Collect {gs.ElderDisplay.Sum()} AP for Elders, so now there are {gs.AP + gs.ElderDisplay.Sum()} AP."); 
                gs.AP += gs.ElderDisplay.Sum();
                for (int i = 6; i > 0; i--)
                {
                    gs.ElderDisplay[i] += gs.ElderDisplay[i - 1];
                    gs.ElderDisplay[i - 1] = 0;
                }
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GameStep planningThree = GetComponent<PlanningThree>();
            planningThree.Begin();
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
