using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class PassageLogic : OperationsLogic
    {
        void Start()
        {
            print("PassageLogic Start");
            Steps = new();
            Steps.Add(clickedPassageofTime);
            Steps.Add(StepOne);
            stepDone = new() { false, false, false, false, false, false };
        }

        public void clickedPassageofTime()
        {
            print("Clicked Passage of Time");
            ui.Initialize();
            ui.OnOpsNextClick += nextStep;
            ui.OnOpsBackClick += backStep;
            stepDone[0] = true;
            step = 0;
            StepOne();
        }

        void StepOne()
        {
            List<Person> childrenInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Child).ToList();
            List<GameState.Family> childrenInFamilies = gs.Families.Where(f => f.HasChild).ToList();
            int numManInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man).Count();
            int numManInFamilies = gs.Families.Where(f => f.HasMan).Count();
            int numWomanInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Woman).Count();
            int numWomanInFamilies = gs.Families.Where(f => f.HasWoman).Count();

            ui.headline.text = "Passage of Time\nStep One";
            if (stepDone[1])
            { return; }  
            // add what to do if already done
            if (childrenInPassage.Count() + childrenInFamilies.Count() > 0)
            {
                ui.message.text = $"Each Child in a Family or the Passage of Time Box may be converted into an Adult or Elder. Select below and place new counters into Passage of Time Box.";
                displayChildren(childrenInPassage, childrenInFamilies);
            }
        }

        // TODO:  move this display function to ChoiceUIScript 
        void displayChildren(List<Person> childrenInPassage, List<GameState.Family> childrenInFamilies)
        {
            ui.choicePanel.visible = true;
            
            int countPassage = 0;
            int countFamilies = 0;

            for (int i = 0; i < (childrenInPassage.Count() + childrenInFamilies.Count()); i++)
            {
                ui.foldouts[i].AddToClassList("FoldoutChild");
                ui.foldouts[i].value = false;
                ui.foldouts[i].style.display = DisplayStyle.Flex;
                
                if (countPassage < childrenInPassage.Count())
                {
                    ui.foldouts[i].text = $"Passage {countPassage+1}";
                    countPassage++;
                    continue;
                }

                ui.foldouts[i].text = childrenInFamilies[countFamilies].Name;
                countFamilies++;
                //add action buttons?
                //ui.foldouts[i].RegisterCallback<ClickEvent>(buttonClicked);
            }

            stepDone[1] = true;
            gm.SaveGame();
            print("step 1 done");
        }

        void StepSeven()
        {
            ui.OnOpsNextClick -= nextStep;
            ui.OnOpsBackClick -= backStep;
            ui.PlayerOpsDone();
        }
    }
}
