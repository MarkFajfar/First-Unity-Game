using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace NavajoWars
{
    public class PassageTwo : GameStep
    {
        public override string stepName { get => "PassageTwo"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Two");
            int animalCount = gs.AnimalsInPassage.Count;
            int horseCount = gs.Families.Where(f => f.HasHorse).Count();
            if (animalCount > 0 || horseCount >0)
            {
                ui.displayText("Return");
                if (animalCount > 0 ) ui.addText($" {animalCount} animal(s) from the Passage of Time box{(horseCount > 0 ? " and" : "")}");
                if (horseCount > 0) ui.addText($" {horseCount} horse(s) from Family boxes");
                ui.addText(" to the Resources box.\nPress Next to continue.");
                foreach (Resource animal in gs.AnimalsInPassage)
                {
                    gs.AnimalsInPassage.Remove(animal);
                    gs.Resources.Add(animal);
                }
                for (int i = 0; i < horseCount; i++)
                {
                    gs.Resources.Add(Resource.Horse);
                    var family = gs.Families.Where(f => f.HasHorse).First();
                    family.HasHorse = false;
                }
            }
            else
            {
                ui.displayText("No animals to be returned to the Resources box.\nPress Next to continue.");
            }
            ui.OnNextClick = actionComplete;
        }
        
        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageThree>().Begin();
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