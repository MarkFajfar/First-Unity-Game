using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class PassageOneA1 : GameStep
    {
        public override string stepName { get => "PassageOneA1"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            List<Person> childrenInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Child);
            List<Family> familiesWithChildren = gs.Families.FindAll(f => f.HasChild);
            // following are not used?
            //int numManInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man).Count();
            //int numManInFamilies = gs.Families.Where(f => f.HasMan).Count();
            //int numWomanInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Woman).Count();
            //int numWomanInFamilies = gs.Families.Where(f => f.HasWoman).Count();

            ui.displayHeadline("Passage of Time\nStep One (A)");
            ui.OnNextClick = actionComplete;
            if (childrenInPassage.Count() + familiesWithChildren.Count() > 0)
            {
                ui.displayText($"Each Child in a Family or the Passage of Time Box may be converted into an Adult or Elder. Select below and place new counters into Passage of Time Box. Or, press Next to continue to the next part of Step One (A).");
                displayChildren(childrenInPassage, familiesWithChildren);
            }
            else 
            {
                ui.displayText("No children eligible to convert; press Next to continue to the next part of Step One (A).");
            }
        }

        void displayChildren(List<Person> childrenInPassage, List<Family> familiesWithChildren)
        {
            int countPassage = 1; 
            List<FoldoutInfo> foldoutsToShow = new();

            foreach (Person child in childrenInPassage)
            {
                List<ButtonInfo> buttons = new();
                ButtonInfo man = new ("PassageMan", "ButtonMan", countPassage, actionOnChildButton);
                ButtonInfo woman = new ("PassageWoman", "ButtonWoman", countPassage, actionOnChildButton);
                man.person = Person.Man;
                woman.person = Person.Woman;
                buttons.Add(man);
                buttons.Add(woman);
                //foreach (ButtonInfo button in buttons) 
                //{ button.text = ""; } // not necessary done in constructor when name but no text is specified
                FoldoutInfo passageFoldout = new (
                        $"Passage {countPassage}",
                        "FoldoutChild",
                        buttons);
                foldoutsToShow.Add(passageFoldout);
                countPassage++;
            }

            foreach (Family family in familiesWithChildren)
            {
                List<ButtonInfo> buttons = new();
                ButtonInfo man = new("FamilyMan", "ButtonMan", actionOnChildButton);
                ButtonInfo woman = new("FamilyWoman", "ButtonWoman", actionOnChildButton);
                man.person = Person.Man;
                woman.person = Person.Woman;
                buttons.Add(man);
                buttons.Add(woman);
                FoldoutInfo familyFoldout = new (
                        $"Child in {family.Name}",
                        "FoldoutChild",
                        buttons);
                familyFoldout.family = family;
                foldoutsToShow.Add(familyFoldout);
            }
            ui.ShowChoiceFoldouts(foldoutsToShow);
            ui.OnNextClick = actionComplete;
        }

        void actionOnChildButton(ButtonInfo button)
        {
            FoldoutInfo foldout = (FoldoutInfo)button.parentData;

            if (foldout.family.Name != Info.Default) 
            {
                foldout.family.HasChild = false;
                ui.displayText($"Remove one Child from {foldout.family.Name} ");
            }
            else 
            {
                gs.PersonsInPassage.Remove(Person.Child);
                ui.displayText($"Remove one Child from the Passage of Time Box ");
            }

            ui.hideFoldout(foldout); 
            gs.PersonsInPassage.Add(button.person);
            ui.addText($"and place a new {button.person} into the Passage of Time Box.");
            ui.addText("\nIf done with children, press Next to continue to the next part of Step One (A).");
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageOneA2>().Begin();
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
