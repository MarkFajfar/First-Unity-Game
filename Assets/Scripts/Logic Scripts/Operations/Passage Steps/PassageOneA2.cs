using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
            displayAdults();
        }

        void displayAdults()
        {
            List<ToggleInfo> togglesToMake = new();

            List<Person> adultsInPassage = gs.PersonsInPassage.FindAll(p => (p == Person.Man || p == Person.Woman)).ToList();

            foreach (Person person in adultsInPassage)
            {
                PersonFamily data = new()
                { 
                    person = person,
                    family = null
                };

                string label = person.ToString() + " in Passage";

                ToggleInfo info = makeToggleInfo(label, data);

                togglesToMake.Add(info);
            }

            foreach (GameState.Family family in gs.Families)
            {
                if (family.HasMan) 
                {
                    PersonFamily man = new()
                    {
                        person = Person.Man,
                        family = family
                    };

                    string label = $"Man in {family.Name}";

                    ToggleInfo info = makeToggleInfo(label, man);

                    togglesToMake.Add(info);
                }

                if (family.HasWoman)
                {
                    PersonFamily woman = new()
                    {
                        person = Person.Woman,
                        family = family
                    };

                    string label = $"Woman in {family.Name}";

                    ToggleInfo info = makeToggleInfo(label, woman);

                    togglesToMake.Add(info);
                }
            }
            ui.ShowChoiceToggles(togglesToMake);
            ui.OnNextClick = displaySummary;
        }

        ToggleInfo makeToggleInfo(string label, object data)
        {
            ToggleInfo info = new()
            {
                label = label,
                name = label.Replace(" ", ""),
                toggleData = data,
                passBack = actionOnToggle,
            };
            return info;
        }

        List<PersonFamily> changes = new();
        // keep a list of changes made when toggled? or just describe final status

        void actionOnToggle(ToggleInfo toggle, bool ticked)
        {
            if (toggle.toggleData is PersonFamily)
            { 
                var data = toggle.toggleData as PersonFamily;

                changes.Add(data); // ?
                
                //data.family != null ? data.family.HasMan = !value : ;
                if (data.family != null)
                {
                    GameState.Family affectedFamily = gs.Families.Where(f => f.Name == data.family.Name).First();
                    if (data.person == Person.Man) affectedFamily.HasMan = !ticked;
                    if (data.person == Person.Woman) affectedFamily.HasWoman = !ticked;
                }
                else
                {
                    if (ticked) 
                    {
                        Person affectedPerson = gs.PersonsInPassage.Where(p => p == data.person).First();
                        gs.PersonsInPassage.Remove(affectedPerson);
                    }
                    else 
                    {
                        gs.PersonsInPassage.Add(data.person);
                    }                    
                }
                if (ticked) gs.PersonsInPassage.Add(Person.Elder);
                if (!ticked) gs.PersonsInPassage.Remove(Person.Elder);
            }
        }

        void displaySummary()
        {
            ui.OnNextClick -= displaySummary;
            ui.ClearChoicePanel();
            ui.displayText($"There are now ... Press Next to continue.");
            ui.addText("\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text\n Example of long text");
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            // GetComponentInChildren<PassageTwo>().Begin();
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
