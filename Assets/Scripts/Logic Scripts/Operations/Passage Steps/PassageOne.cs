using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PassageOne : GameStep
    {
        public override string stepName { get => "PassageOne"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            List<Person> childrenInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Child).ToList();
            List<GameState.Family> childrenInFamilies = gs.Families.Where(f => f.HasChild).ToList();
            int numManInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man).Count();
            int numManInFamilies = gs.Families.Where(f => f.HasMan).Count();
            int numWomanInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Woman).Count();
            int numWomanInFamilies = gs.Families.Where(f => f.HasWoman).Count();

            ui.displayHeadline("Passage of Time\nStep One");
            if (childrenInPassage.Count() + childrenInFamilies.Count() > 0)
            {
                ui.displayText($"Each Child in a Family or the Passage of Time Box may be converted into an Adult or Elder. Select below and place new counters into Passage of Time Box.");
                displayChildren(childrenInPassage, childrenInFamilies);
            }
        }

        void displayChildren(List<Person> childrenInPassage, List<GameState.Family> childrenInFamilies)
        {
            int countPassage = 0;
            int countFamilies = 0;

            for (int i = 0; i < (childrenInPassage.Count() + childrenInFamilies.Count()); i++)
            {
                // use MakeFamilyFoldouts; use a Dictionary?
                // but foldout has the same buttons man, woman, elder right?

                //ui.foldouts[i].AddToClassList("FoldoutChild");
                //ui.foldouts[i].value = false;
                //ui.foldouts[i].style.display = DisplayStyle.Flex;

                if (countPassage < childrenInPassage.Count())
                {
                    //ui.foldouts[i].text = $"Passage {countPassage+1}";
                    countPassage++;
                    continue; // skips the rest of loop and goes back to beginning
                }

                //ui.foldouts[i].text = childrenInFamilies[countFamilies].Name;
                countFamilies++;
                //add action buttons?
                //ui.foldouts[i].RegisterCallback<ClickEvent>(buttonClicked);
            }
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
