using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PassageOneA1 : GameStep
    {
        public override string stepName { get => "PassageOneA1"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            List<Person> childrenInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Child).ToList();
            List<GameState.Family> familiesWithChildren = gs.Families.Where(f => f.HasChild).ToList();
            // following are not used?
            //int numManInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man).Count();
            //int numManInFamilies = gs.Families.Where(f => f.HasMan).Count();
            //int numWomanInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Woman).Count();
            //int numWomanInFamilies = gs.Families.Where(f => f.HasWoman).Count();

            ui.displayHeadline("Passage of Time\nStep One (A)");
            ui.makeTestButton(); 
            /*if (childrenInPassage.Count() + familiesWithChildren.Count() > 0)
            {
                ui.displayText($"Each Child in a Family or the Passage of Time Box may be converted into an Adult or Elder. Select below and place new counters into Passage of Time Box.");
                displayChildren(childrenInPassage, familiesWithChildren);
            }
            else 
            {
                ui.displayText("No children eligible to convert; press Next to continue to the next part of Step One (A).");
                ui.OnNextClick = actionComplete;
            }*/
        }

        void displayChildren(List<Person> childrenInPassage, List<GameState.Family> familiesWithChildren)
        {
            int countPassage = 1; 
            List<FoldoutInfo> foldoutsToMake = new();

            foreach (Person person in childrenInPassage)
            {
                List<ButtonInfo> buttons = new();
                ButtonInfo man = new ("PassageMan", "ButtonMan", countPassage, actionOnChildButton);
                ButtonInfo woman = new ("PassageWoman", "ButtonWoman", countPassage, actionOnChildButton);
                man.buttonData = Person.Man;
                woman.buttonData = Person.Woman;
                buttons.Add(man);
                buttons.Add(woman);
                //foreach (ButtonInfo button in buttons) 
                //{ button.text = ""; } // not necessary done in constructor when name but no text is specified
                FoldoutInfo passageFoldout = new (
                        $"Passage {countPassage}",
                        "FoldoutChild",
                        buttons);
                passageFoldout.foldoutData = "Passage";
                foldoutsToMake.Add(passageFoldout);
                countPassage++;
            }

            foreach (GameState.Family family in familiesWithChildren)
            {
                List<ButtonInfo> buttons = new();
                ButtonInfo man = new("FamilyMan", "ButtonMan", actionOnChildButton);
                ButtonInfo woman = new("FamilyWoman", "ButtonWoman", actionOnChildButton);
                man.buttonData = Person.Man;
                woman.buttonData = Person.Woman;
                buttons.Add(man);
                buttons.Add(woman);
                FoldoutInfo familyFoldout = new (
                        $"Child in {family.Name}",
                        "FoldoutChild",
                        buttons);
                familyFoldout.foldoutData = family;
                foldoutsToMake.Add(familyFoldout);
            }
            ui.ShowChoiceFoldouts(foldoutsToMake);
            ui.OnNextClick = actionComplete;
        }

        /*// this will not work because it doesn't show which child clicked
        List<ButtonInfo> buttonsForChildFoldout() 
        {
            List<ButtonInfo> buttons = new List<ButtonInfo>();
            man = new ButtonInfo("Man", "ButtonMan", actionOnChildButton);
            woman = new ButtonInfo("Woman", "ButtonWoman", actionOnChildButton);
            buttons.Add(man);
            buttons.Add(woman);
            return buttons;
        }*/

        void actionOnChildButton(ButtonInfo button)
        {
            print($"{button.name} clicked");
            Person added = (Person)button.buttonData;
            FoldoutInfo foldout = (FoldoutInfo)button.parentData;

            if (foldout.foldoutData is string)  // if (foldout.foldoutData as string == "Passage")
            {
                gs.PersonsInPassage.Remove(Person.Child);              
                ui.displayText($"Remove one Child from the Passage of Time Box ");
            }
            else
            // if (button.name.StartsWith("Family")); or reverse if - Family
            {
                //string familyName = button.name[..8];
                //FoldoutInfo info = (FoldoutInfo)button.parentData;
                GameState.Family family = foldout.foldoutData as GameState.Family;
                    //gs.Families.Where(f => f.Name == familyName).First();
                if (family != null) family.HasChild = false;
                ui.displayText($"Remove one Child from {family.Name} ");
            }

            gs.PersonsInPassage.Add(added);
            ui.addText($"and place a new {added} into the Passage of Time Box.");
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
