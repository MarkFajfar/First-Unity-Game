using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NavajoWars
{
    public class PassageFour : GameStep
    {
        public override string stepName { get => "PassageFour"; }

        List<Territory> hasFamilyAndCorn = new();

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Four");
            foreach (var t in gs.Territories)
            {
                if (t.HasCorn && t.Families.Count > 0) 
                hasFamilyAndCorn.Add(t);                
            }
            if (hasFamilyAndCorn.Count > 0)
            {
                ui.displayText("If any Family is in an area with a corn counter, do you wish to harvest? (Remember - do not look at counter before deciding!)\n");
                ui.addText($"Note: Your population is {gs.Population} and your total arability is {gs.Arability}. ");
                if (gs.SheepHeld > 0) ui.addText($"Your sheep can feed {gs.SheepHeld * 4} population. "); 
                int deficit = gs.Population - gs.Arability - (gs.SheepHeld * 4);
                ui.addText($"So, you {(deficit > 0 ? $"need {deficit} corn" : "do not need any corn")} to feed your population.");

                ui.ShowChoiceButtons(new()
                {
                    new("Yes", yes),
                    new("No", no)
                });
            }
            else
            {
                ui.displayText("No Area with a Family and a corn counter.\nPress Next to continue.");
                ui.OnNextClick = actionComplete;
            }
        }

        void yes()
        {
            ui.displayText("Place havested counters face up in the Resources box. Press button for each Territory harvested, then press Next to continue.");
            List<ButtonInfo> buttons = new();
            foreach (var territory in hasFamilyAndCorn)
            {
                ButtonInfo button = new($"{territory.Name}", recordHarvest)
                {
                    territory = territory,
                    clearPanel = false, // don't clear panel when clicked
                    remove = true // remove when clicked
                };
                buttons.Add(button);
            }
            ui.ShowChoiceButtons(buttons);
            // or list of bool items to be ticked, then next to complete
            ui.OnNextClick = actionComplete;
        }

        void recordHarvest(ButtonInfo info)
        {
            // TODO: what if more than one corn counter harvested from a Territory?
            ui.addText($"\nCorn harvested from {info.text}; place it in Resources box.");
            info.territory.CornNum --;
            gs.Resources.Add(Resource.Corn);
        }

        void no()
        {
            ui.displayText("No corn harvested.\nPress Next to continue.");
            ui.OnNextClick = actionComplete;
        }
        
        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageFive>().Begin();
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
