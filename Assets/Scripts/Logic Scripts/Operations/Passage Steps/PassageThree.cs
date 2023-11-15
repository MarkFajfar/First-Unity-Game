using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PassageThree : GameStep
    {
        public override string stepName { get => "PassageThree"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Three");
            bool sheep = gs.SheepHeld > 0 && gs.SheepHeld < gs.SheepMax;
            bool horses = gs.HorsesHeld > 0 && gs.HorsesHeld < gs.HorsesMax;
            if (sheep) 
            {
                ui.displayText($"Sheep breed; add one to the Resources box so that there are {gs.SheepHeld + 1}.\n");
                gs.Resources.Add(Resource.Sheep);
            }
            if (horses)
            {
                ui.displayText($"Horses breed; add one to the Resources box so that there are {gs.HorsesHeld + 1}.\n");
                gs.Resources.Add(Resource.Horse);
            }
            if (!sheep && !horses) ui.displayText("No animals in the Resources box.\n");
            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageFour>().Begin();
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
