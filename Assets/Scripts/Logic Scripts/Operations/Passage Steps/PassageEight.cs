using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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

            if (gs.ListTerrDrought.Count() < 1)
            {
                ui.displayText("No Territories have Drought. ");
                ui.addText("Press Next to continue.");
                ui.OnNextClick = actionComplete;
            }
            else droughtCheck();
        }

        void droughtCheck()
        {
            ui.displayText("Roll a die to see if a Territory recovers from Drought. Press the die roll result."); 
            ui.ShowChoiceButtons( new(){
                new("One", droughtResolve) {territory = gs.Territories[1]},
                new("Two", droughtResolve) {territory = gs.Territories[2]},
                new("Three", droughtResolve) {territory = gs.Territories[3]},
                new("Four", droughtResolve) {territory = gs.Territories[4]},
                new("Five", droughtResolve) {territory = gs.Territories[5]},
                new("Six", droughtResolve) {territory = gs.Territories[6]}
            } );
        }

        void droughtResolve(ButtonInfo info)
        {
            ui.displayText($"Dice roll is {info.text} for {info.territory}. ");
            // create array of three Territories to check in sequence; break when drought removed
            Territory lowerAdjacent = gs.Territories[math.max(1, info.territory.Number - 1)];
            Territory higherAdjacent = gs.Territories[math.min(6, info.territory.Number + 1)];
            Territory[] terrToCheck = {info.territory, lowerAdjacent, higherAdjacent};

            bool droughtRemoved = false;

            foreach (var terr in terrToCheck)
            {
                if (terr.HasDrought)
                {
                    ui.addText($"Remove one Drought counter from {terr}. ");
                    terr.DroughtNum --;
                    // if (gs.TerritoryDrought[(int)terr] < 1) 
                    //   { gs.HasDrought.Remove(terr); }
                    droughtRemoved = true;
                    break;
                }
            }

            if (!droughtRemoved) ui.addText("No Drought to be removed because none in an adjacent Territory. ");

            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
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
