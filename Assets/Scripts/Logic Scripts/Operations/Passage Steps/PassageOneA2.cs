using System.Collections.Generic;
using System.Linq;

namespace NavajoWars
{
    public class PassageOneA2 : GameStep
    {
        public override string stepName { get => "PassageOneA2"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            
            ui.displayHeadline("Passage of Time\nStep One (A)");
            // there will always be at least one adult!
            ui.displayText($"Each Adult in a Family or the Passage of Time Box may be converted into an Elder. Select below and and place new Elder counters into Passage of Time Box. Press Next to continue when done.");
            ui.OnNextClick = displaySummary;
            displayAdults();
        }

        void displayAdults()
        {
            List<ToggleInfo> togglesToMake = new();

            List<Person> adultsInPassage = gs.PersonsInPassage.FindAll(p => (p == Person.Man || p == Person.Woman));

            foreach (Person person in adultsInPassage)
            {
                string label = person.ToString() + " in Passage";
                ToggleInfo info = new(label, new() { Name = Info.Default}, person, actionOnToggle);
                togglesToMake.Add(info);
            }

            foreach (Family family in gs.Families)
            {
                if (family.HasMan) 
                {
                    string label = $"Man in {family.Name}";
                    ToggleInfo info = new(label, family, Person.Man, actionOnToggle);
                    togglesToMake.Add(info);
                }

                if (family.HasWoman)
                {
                    string label = $"Woman in {family.Name}";
                    ToggleInfo info = new(label, family, Person.Woman, actionOnToggle);
                    togglesToMake.Add(info);
                }
            }
            ui.ShowChoiceToggles(togglesToMake);
        }

        void actionOnToggle(ToggleInfo info, bool ticked)
        {
            if (info.family.Name != Info.Default)
            {
                Family affectedFamily = gs.Families.Find(f => f.Name == info.family.Name);
                if (info.person == Person.Man) affectedFamily.HasMan = !ticked;
                if (info.person == Person.Woman) affectedFamily.HasWoman = !ticked;
            }
            else
            {
                if (ticked) 
                {
                    Person affectedPerson = gs.PersonsInPassage.Find(p => p == info.person);
                    gs.PersonsInPassage.Remove(affectedPerson);
                }
                else 
                {
                    gs.PersonsInPassage.Add(info.person);
                }                    
            }
            if (ticked) gs.PersonsInPassage.Add(Person.Elder);
            else gs.PersonsInPassage.Remove(Person.Elder);
        }

        void displaySummary()
        {
            ui.OnNextClick -= displaySummary;
            ui.ClearChoicePanel();
            ui.displayText($"There are now ... Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageOneB>().Begin();
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
