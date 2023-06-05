using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningSix : GameStep
    {
        // all cubes in Recovery to Raid Pool Bag, then all cubes in Raided to Recovery
        public override string stepName { get => "PlanningSix"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Six");
            ui.displayText("Reset Cubes"); // add text from glossary
            ui.addText("Press Next to continue.");
            ui.OnNextClick = resetCubes;
        }

        void resetCubes() 
        { 
            ui.OnNextClick -= resetCubes;
            foreach (Cube cube in gs.Raided)
            {
                gs.Recovery.Add(cube);
                gs.Raided.Remove(cube);
            }
            ui.displayText("The Raided Bowl is now empty, and the following cubes are in the Recovery Bowl:\n");
            foreach (Cube cube in gs.Recovery)
            {
                ui.addText($"{cube}, ");
            }
            ui.addText("\nPress Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PlanningSeven>().Begin();
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
