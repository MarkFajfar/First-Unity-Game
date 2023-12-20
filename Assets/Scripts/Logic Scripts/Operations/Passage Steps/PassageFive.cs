using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PassageFive : GameStep
    {
        public override string stepName { get => "PassageFive"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Five");
            int cornCounters = gs.Resources.Where(r => r == Resource.Corn).Count();
            if (gs.Arability >= gs.Population)
            {
                ui.displayText($"Your population of {gs.Population} can be fed by the total arability ({gs.Arability}) of your territories. {(cornCounters > 0 ?"However, return all corn counters in the Resources box to the draw cup. " : "")}Press Next to continue.");
            }
            else
            {
                int deficit = gs.Population - gs.Arability;
                ui.displayText($"Your population is {gs.Population} and your total arability is {gs.Arability}. ");
                if (gs.SheepHeld < 1 && cornCounters < 1)
                {
                    ui.addText($"You must return {deficit} population counters to the Out of Play box. Open the Status menu and select the population lost. ");
                }
                else 
                {
                    ui.addText($"You need {deficit} food. ");
                    if (gs.SheepHeld > 0) ui.addText("Each sheep counter may feed 4 population . ");
                    if (cornCounters > 0) ui.addText("Each corn counter in the Resources box may feed the number shown. ");
                    if (deficit <= (gs.SheepHeld * 4) + cornCounters)
                    {
                        // enough even if each corn counter is only one
                        ui.addText($"You must feed all your population. Open the Status menu and select the resource counters used. Any excess on the counters used is lost. Leave unused sheep counters in the Resources box{(cornCounters > 0 ? ", but return any unused corn counters in the Resources box to the draw cup" : "")}. ");
                    }
                    else
                    {
                        ui.addText($"You must feed as much of your population as you can with your sheep and corn. Open the Status menu and select the resource counters used. Any excess on the counters used is lost. If any population remains unfed, return them to the Out of Play box. Leave unused sheep counters in the Resources box{(cornCounters > 0 ? ", but return any unused corn counters in the Resources box to the draw cup" : "")}. ");
                    }
                }
                ui.addText("Then press Next to continue.");
            }
            gs.Resources.RemoveAll(r => r == Resource.Corn);
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageSix>().Begin();
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
