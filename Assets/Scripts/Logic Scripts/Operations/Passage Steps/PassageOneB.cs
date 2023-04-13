using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

namespace NavajoWars
{
    public class PassageOneB : GameStep
    {
        public override string stepName { get => "PassageOneA2"; }

        public override void Begin()
        {
            foreach (Person p in gs.PersonsInPassage.Where(p => p == Person.Default)) gs.PersonsInPassage.Remove(p);

            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep One (B)");

            if (gs.PersonsInPassage.Count == 0)
            {
                ui.displayText("No persons in Passage of Time Box. Press Next to skip to Step One (D).");
                ui.OnNextClick = actionComplete;
            }
            else
            {
                int numEldersInPassage = gs.PersonsInPassage.FindAll
                    (p => p == Person.Elder).Count();
                int numOthersInPassage = gs.PersonsInPassage.FindAll
                    (p => p == Person.Man || p == Person.Woman || p == Person.Child).Count();
                ui.displayText("");
                if (numOthersInPassage > 0)
                {
                    ui.addText($"Each Child and Adult in the Passage of Time Box may be moved to an in-play Family{(gs.CP > 0 ? " or to start a new Family" : "")}. ");
                    if (gs.CP > 0) ui.addHeadline(" and (C)");
                    displayPassageOthers(numEldersInPassage);
                }
                else 
                {
                    ui.addText("Each Elder may be moved to the left-most box of the Elder Display.");
                    displayPassageElders();
                }                
            }
        }

        private void displayPassageOthers(int elders)
        {
            List<Person> othersInPassage = gs.PersonsInPassage.Where(p => p == Person.Man || p == Person.Woman || p == Person.Child).ToList();
            int countPassage = 1;
            List<FoldoutInfo> foldoutsToShow = new();

            foreach (Person person in othersInPassage)
            {
                List<ButtonInfo> buttons = new();

                List<Family> families = person switch
                {
                    Person.Child => gs.Families.Where(f => !f.HasChild).ToList(),
                    Person.Woman => gs.Families.Where(f => !f.HasWoman).ToList(),
                    Person.Man => gs.Families.Where(f => !f.HasMan).ToList(),
                    _ => gs.Families
                };
                
                foreach (var family in families)
                { buttons.Add(makeFamilybutton(countPassage, family)); }
                
                if (gs.CP > 0) 
                {
                    foreach (var family in gs.AllFamilies)
                    { 
                        if (!family.IsActive) buttons.Add(makeFamilybutton(countPassage, family)); 
                    }
                    //foreach (var inactive in gs.Families.Where(f => !f.IsActive && f.Name != Info.Default).ToList())
                    // inactive families are not in gs.Families
                }
                FoldoutInfo f = new(
                        $"Passage {countPassage}, {person}",
                        "FoldoutChild",
                        buttons);
                foldoutsToShow.Add(f);
                countPassage++;
            }

            ButtonInfo makeFamilybutton(int count, Family family) 
            {
                ButtonInfo b = new()
                {
                    text = $"Move to {family.Name}",
                    name = family.Name.Replace(" ", "") + count,
                    style = "TBD",
                    //TO DO: make style for text button in foldout
                    tabIndex = count,
                    passBack = actionOnFoldoutButton
                };
                return b;
            }

            ui.ShowChoiceFoldouts(foldoutsToShow);
            ui.OnNextClick = actionComplete;
            if (elders > 0) ui.OnNextClick = displayPassageElders;
        }

        void displayPassageElders()
        {
            List<ToggleInfo> togglesToMake = new();
            int elders = gs.PersonsInPassage.FindAll(p => p == Person.Elder).Count();
            for (int i = 0; i < elders; i++) 
            {
                string label = $"Elder {i} in Passage";
                ToggleInfo info = new(label, new() { Name = Info.Default} , Person.Elder, actionOnToggle);
                togglesToMake.Add(info);
            }
            ui.ShowChoiceToggles(togglesToMake);
            ui.OnNextClick = actionComplete;
        }

        void actionOnFoldoutButton(ButtonInfo info)
        {
            
        }

        void actionOnToggle(ToggleInfo info, bool ticked)
        {
            if (ticked)
            {
                //tbd
            }
            else 
            { 
            
            }
        }

        void removeDefaultPersons()
        {
            foreach (Person p in gs.PersonsInPassage.Where(p => p == Person.Default)) gs.PersonsInPassage.Remove(p);
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            //GetComponentInChildren<PassageOneD>().Begin();
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
