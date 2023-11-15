using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Plant : GameStep
    {
        public override string stepName { get => "Plant"; }

        Family selectedFamily;
        Territory territory;

        public override void Begin()
        {
            // add?
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            territory = selectedFamily.IsWhere;
            bool drought = gs.HasDrought.Contains(territory);
            bool rancho = gs.HasRancho.Contains(territory);
            int missing = OperationsLogic.numMissing(selectedFamily);
            ui.displayHeadline($"{selectedFamily.Name} Plants OR Harvests");
            ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(gs.completedActions == 0 ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ");
            ui.addText((drought || rancho) ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ");
            ui.addText("To harvest, reveal Corn counter and place it in Resources box. Will you plant OR harvest? (Choose One)");
            ui.ShowChoiceButtons(new()
                {
                    new("Plant", plant),
                    new("Harvest", harvest),
                    new("Neither", neither)
                });
            // TODO: RECORD PLANT OR HARVEST
            // could have multiple entries in HasCorn if planting more than once in a Territory
        }

        void plant()
        {
            ui.displayText($"Corn planted in {territory}. Press Next to continue.");
            gs.HasCorn.Add(territory);
            ui.OnNextClick = actionComplete;
        }

        void harvest()
        {
            ui.displayText($"Corn harvested from {territory}.");
            gs.HasCorn.Remove(territory);
            if (gs.HasCorn.Contains(territory)) 
            { 
                int num = gs.HasCorn.Where(t => t == territory).Count();
                ui.addText($" There {Are(num)} corn remaining in {territory}.");
            }
            ui.addText(" Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        void neither()
        {
            ui.displayText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        string Are(int i) => i == 1 ? "is one" : $"are {i}";

        protected override void actionComplete()
        {
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponentInChildren<ChooseAnotherAction>();
            chooseAnotherAction.Begin();
        }

        public override void Undo()
        {
            // reset complete marker??
            if (isCompleted) 
            { 
                isCompleted = false; 
                gs.completedActions --; 
            }
            Begin();
            // stuff to do on undo
        }
    }
}
