using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PassageSix : GameStep
    {
        public override string stepName { get => "PassageSix"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Six");
            if (gs.SheepHeld > 0 || gs.HorsesHeld > 0)
            {
                ui.displayText($"Your Families occupy {gs.TerritoriesWithFamily} Territories and you have {gs.SheepHeld} Sheep and {gs.HorsesHeld} Horse(s). ");
                if (gs.SheepHeld <= gs.TerritoriesWithFamily && gs.HorsesHeld <= gs.TerritoriesWithFamily)
                {
                    ui.addText("All animals can be fed. ");
                }
                else 
                {
                    if (gs.SheepHeld > gs.TerritoriesWithFamily) ui.addText($"Return {gs.SheepHeld - gs.TerritoriesWithFamily} Sheep to the Out of Play box. ");
                    if (gs.HorsesHeld > gs.TerritoriesWithFamily) ui.addText($"Return {gs.HorsesHeld - gs.TerritoriesWithFamily} Horse(s) to the Out of Play box. ");
                    ui.addText("Open the Status menu and select the animals lost. ");
                }
            }
            else
            {
                ui.displayText("No animals to be fed. ");
            }
            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageSeven>().Begin();
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
