using System.Collections.Generic;
using System.Linq;

namespace NavajoWars
{
    public class PassageTwo : GameStep
    {
        public override string stepName { get => "PassageTwo"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Two");
            ui.displayText("Return all Animal counters in the Family boxes and the Passage of Time box to the Resources box.\nPress Next to continue.");


            
            ui.OnNextClick = actionComplete;




        }
        
        protected override void actionComplete()
        {
            base.actionComplete();
            //GetComponentInChildren<PassageThree>().Begin();
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