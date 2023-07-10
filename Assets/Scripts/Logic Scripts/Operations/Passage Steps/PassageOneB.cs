using System.Collections.Generic;
using System.Linq;

namespace NavajoWars
{
    public class PassageOneB : GameStep
    {
        public override string stepName { get => "PassageOneB"; }

        public override void Begin()
        {
            foreach (Person p in gs.PersonsInPassage.Where(p => p == Person.Default)) gs.PersonsInPassage.Remove(p);

            elderMoveCount = 0; // necessary here?

            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep One (B) - (E)");

            if (gs.PersonsInPassage.Count == 0)
            {
                ui.displayText("No persons in Passage of Time Box. Press Next to skip to Step One (D), Adjust Culture Points.");
                ui.OnNextClick = placeFamiliesAndDisplaySummary;
            }
            else
            {
                int numEldersInPassage = gs.PersonsInPassage.Where
                    (p => p == Person.Elder).Count();
                int numOthersInPassage = gs.PersonsInPassage.Where
                    (p => p == Person.Man || p == Person.Woman || p == Person.Child).Count();
                ui.displayText("");
                if (numOthersInPassage > 0)
                {
                    ui.addText($"Each Child and Adult in the Passage of Time Box may be moved to an in-play Family{(gs.CP > 0 ? " or to start a new Family" : "")}. ");
                    displayPassageOthers(numEldersInPassage);
                }
                else 
                {
                    ui.addText("Each Elder may be moved to the left-most box of the Elder Display.");
                    displayPassageElders();
                }                
            }
        }

        List<Family> activatedFamilies = new ();

        void displayPassageOthers(int elders)
        {
            List<Person> othersInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man || p == Person.Woman || p == Person.Child);
            int countPassage = 1;
            List<FoldoutInfo> foldoutsToShow = new();

            foreach (Person person in othersInPassage)
            {
                List<ButtonInfo> buttons = new();

                List<Family> eligibleFamilies = person switch
                {
                    Person.Child => gs.Families.FindAll(f => !f.HasChild),
                    Person.Woman => gs.Families.FindAll(f => !f.HasWoman),
                    Person.Man => gs.Families.FindAll(f => !f.HasMan),
                    _ => gs.Families
                };
                
                foreach (var family in eligibleFamilies)
                { buttons.Add(makeFamilybutton(countPassage, family, person)); }
                
                if (gs.CP > 0) 
                {
                    foreach (var family in gs.AllFamilies)
                    { 
                        if (!family.IsActive) buttons.Add(makeFamilybutton(countPassage, family, person)); 
                    }
                    //foreach (var inactive in gs.Families.Where(f => !f.IsActive && f.Name != Info.Default).ToList())
                    // inactive families are not in gs.Families
                }
                FoldoutInfo foldoutToShow = new(
                        $"Passage {countPassage}, {person}",
                        "FoldoutChild",
                        buttons);
                foldoutsToShow.Add(foldoutToShow);
                countPassage++;
            }

            //TODO: make this a generic method?
            ButtonInfo makeFamilybutton(int count, Family family, Person person) 
            {
                ButtonInfo familyButton = new()
                {
                    text = $"Move to {family.Name}",
                    name = family.Name.Replace(" ", "") + count,
                    style = "TBD",
                    //TODO: make style for text button in foldout
                    tabIndex = count,
                    family = family,
                    person = person,
                    passBack = actionOnFoldoutButton
                };
                return familyButton;
            }

            ui.ShowChoiceFoldouts(foldoutsToShow);
            ui.OnNextClick = placeFamiliesAndDisplaySummary;
            if (elders > 0) ui.OnNextClick = displayPassageElders;
        }

        void displayPassageElders()
        {
            List<ToggleInfo> togglesToMake = new();
            int elders = gs.PersonsInPassage.Where(p => p == Person.Elder)
                                            .Count();
            for (int i = 0; i < elders; i++) 
            {
                string label = $"Move Elder {i + 1}";
                ToggleInfo info = new(label, new() { Name = Info.Default} , Person.Elder, actionOnToggle);
                togglesToMake.Add(info);
            }
            ui.ShowChoiceToggles(togglesToMake);
            ui.OnNextClick = placeFamiliesAndDisplaySummary;
        }

        void actionOnFoldoutButton(ButtonInfo info)
        {
            if (!info.family.IsActive)
            { 
                info.family.IsActive = true;
                gs.Families.Add(info.family);
                activatedFamilies.Add(info.family);
            }

            if (info.person == Person.Child) 
            { 
                info.family.HasChild = true;
                gs.PersonsInPassage.Remove(Person.Child);
            }
            if (info.person == Person.Woman)
            {
                info.family.HasWoman = true;
                gs.PersonsInPassage.Remove(Person.Woman);
            }
            if (info.person == Person.Man)
            {
                info.family.HasMan = true;
                gs.PersonsInPassage.Remove(Person.Man);
            }
        }

        int elderMoveCount;
        // does this need to be reset to zero at some point? when and how? in Begin?

        void actionOnToggle(ToggleInfo info, bool ticked)
        {
            if (ticked)
            {
                gs.PersonsInPassage.Remove(Person.Elder);
                gs.ElderDisplay[0]++;
                elderMoveCount++;
            }
            else 
            {
                gs.PersonsInPassage.Add(Person.Elder);
                gs.ElderDisplay[0]--;
                elderMoveCount--;
            }
        }

        void placeFamiliesAndDisplaySummary() 
        {
            ui.OnNextClick -= placeFamiliesAndDisplaySummary;
            ui.ClearChoicePanel();
            ui.displayText("");

            if (activatedFamilies.Count > 0) 
            {
                int i = 0;
                while (i < activatedFamilies.Count)
                {
                    ui.addText($"{activatedFamilies[i].Name}");
                    if (activatedFamilies.Count > 1 && i != activatedFamilies.Count - 1) ui.addText(",");
                    ui.addText(" ");
                    activatedFamilies[i].IsWhere = Territory.Canyon;
                    activatedFamilies[i].Ferocity = 0;
                    i++;
                }
                string s = activatedFamilies.Count switch
                {
                    1 => "is now active. Place it",
                    > 1 => "are now active. Place them",
                    _ => "invalid - go back"
                };
                //if only two cases could use short form:
                //ui.addText($"{(activatedFamilies.Count == 1 ? "is now active. Place it" : "are now active. Place them")} in any Area ..." )
                ui.addText(s + " in any Area in the Canyon de Chelly with a Ferocity of 0.\n");
            }
            
            int totalMissing = 0;
            foreach (Family family in gs.Families) 
            { totalMissing += OperationsLogic.numMissing(family); } 
            if (totalMissing > 0)
            {
                ui.addText($"{totalMissing} CP deducted for empty spaces in active families.\n"); 
                gs.CP -= totalMissing;
            }
            
            if (elderMoveCount > 0)
            {
                ui.addText($"{elderMoveCount} CP added for Elders moved to the Elder Display.\n");
                gs.CP += elderMoveCount;
            }

            if (gs.PersonsInPassage.Count() > 0)
            {
                ui.addText($"Discard {gs.PersonsInPassage.Count()} counters from the Passage of Time Box to the Out of Play Box.\n");
                foreach (var person in gs.PersonsInPassage)
                { gs.PersonsInPassage.Remove(person); }
            }

            ui.displayText($"Passage of Time Step One complete. Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            //GetComponentInChildren<PassageTwo>().Begin();
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
