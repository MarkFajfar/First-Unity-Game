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
            ui.OnNextClick += resetCubes;
        }

        void resetCubes() 
        { 
            ui.OnNextClick -= resetCubes;
            gs.Recovery.Black = gs.Raided.Black;
            gs.Raided.Black = 0;
            gs.Recovery.White = gs.Raided.White;
            gs.Raided.White = 0;
            gs.Recovery.Brown = gs.Raided.Brown;
            gs.Raided.Brown = 0;
            gs.Recovery.Green = gs.Raided.Green;
            gs.Raided.Green = 0;
            gs.Recovery.Blue = gs.Raided.Blue;
            gs.Raided.Blue = 0;
            gs.Recovery.Yellow = gs.Raided.Yellow;
            gs.Raided.Yellow = 0;
            gs.Recovery.Red = gs.Raided.Red;
            gs.Raided.Red = 0;
            ui.displayText($"The Raided Bowl is now empty, and the following cubes are in the Recovery Bowl:\nBlack: {gs.Recovery.Black}\nWhite: {gs.Recovery.White}\nBrown: {gs.Recovery.Brown}\nGreen: {gs.Recovery.Green}\nBlue: {gs.Recovery.Blue}\nYellow: {gs.Recovery.Yellow}\nRed: {gs.Recovery.Red}\nPress Next to continue.");
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PlanningSeven>().Begin();
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
