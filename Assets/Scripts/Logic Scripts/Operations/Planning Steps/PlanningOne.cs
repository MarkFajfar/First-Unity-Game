using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningOne : GameStep
    {
        public override string stepName { get => "PlanningOne"; }

        public override void Begin()
        {
            // reset completed families only if coming from choose player operation 
            GameStep caller = null;
            if (gm.stepStack.Count > 0) caller = gm.stepStack.Peek();
            if (caller != null && caller.stepName == "PlayerOperation")
            {
                gs.completedFamilies = 0;
                gs.completedActions = 0;
                foreach (var family in gs.Families)
                {
                    family.isSelectedOps = false;
                    family.isCompletedOps = false;
                }
                clearCompleted?.Invoke();
            }
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep One");
            ui.displayText($"Place a {gs.CurrentCard.ThisCardPerson} in the Passage of Time Box, then press Next to continue.");
            gs.PersonsInPassage.Add(gs.CurrentCard.ThisCardPerson);
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GameStep planningTwo = GetComponentInChildren<PlanningTwo>();
            planningTwo.Begin();
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
