using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningThree : GameStep
    {
        public override string stepName { get => "PlanningThree"; }

        List<string> selectedF;

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Three");
            if (gs.ElderDisplay.Sum() > 0) elderAction();
            else
            {
                ui.displayText("No Elder Actions available. Press Next to continue.");
                ui.OnNextClick = actionComplete;
            }
        }

        //TODO revise to be calls rather than async
        async void elderAction()
        {
            selectedF = new();
            // ui.hideBackNext();
            // async necessary to pause loops and await button click
            // at this point there are no elders in slot 0 (moved right in prev step)
            for (int i = 1; i < gs.ElderDisplay.Length; i++)
            {
                if (gs.ElderDisplay[i] > 0)
                {
                    ui.displayText(""); 
                    int j = 0;
                    while (j < gs.ElderDisplay[i])
                    {
                        ui.addText($"{gs.ElderDisplay[i] - j} Elder Action(s) in Slot {i}. Roll Die.\nFor this roll you need a {gs.ElderTarget[i]} or less.");
                        ButtonInfo yes = new("Roll Succeeded");
                        ButtonInfo no = new("Roll Failed");
                        List<ButtonInfo> choices = new() { yes, no };
                        ui.MakeChoiceButtonsAsync(choices);
                        ButtonInfo result = await IReceive.GetChoiceAsyncParams();
                        ui.displayText(""); 
                        // if no, will increment j and go to next loop
                        if (result == yes)
                        {
                            ui.displayText("Choose Action");
                            ButtonInfo addAP = new("Add 1 AP");
                            ButtonInfo changeFerocity = new("Change Family Ferocity");
                            ButtonInfo addMP = new("Add 1 MP for 1 CP");
                            ButtonInfo addCP = new("Add 1 CP for 1 MP");
                            List<ButtonInfo> elderChoices = new() { addAP, changeFerocity };
                            if (gs.CP > gs.MP) elderChoices.Add(addMP);
                            if (gs.MP > gs.CP) elderChoices.Add(addCP);
                            ui.MakeChoiceButtonsAsync(elderChoices);
                            ButtonInfo elderResult = await IReceive.GetChoiceAsyncParams();
                            if (elderResult == addAP) 
                            {
                                gs.AP++; 
                                ui.displayText($"Adding 1 AP; there are now {gs.AP}\n");
                            }
                            if (elderResult == addMP) 
                            {
                                gs.MP++;
                                gs.CP--;
                                ui.displayText($"Adding 1 MP (to {gs.MP}) and Reducing CP by 1 (to {gs.CP})\n");
                            }
                            if (elderResult == addCP)
                            {
                                gs.MP--;
                                gs.CP++;
                                ui.displayText($"Adding 1 CP (to {gs.CP}) and Reducing MP by 1 (to {gs.MP})\n");
                            }
                            if (elderResult == changeFerocity)
                            {
                                // TODO: fix loop needing to be one long function
                                ChangeFamilyFerocity1();
                                ButtonInfo ferocityResult = await IReceive.GetChoiceAsyncParams();
                                ChangeFamilyFerocity2(ferocityResult);
                            }
                        }
                        j++;
                    }
                }
            }
            ui.displayHeadline("Elder Actions Completed");
            ui.displayText("Press Next to continue.");
            ui.showBackNext();
            ui.OnNextClick += actionComplete;
        }

        void ChangeFamilyFerocity1()
        {
            List<Family> listFerocityFamilies = new();
            ui.displayText("Change one Family +/- 1. Must have a Man. If increased and MP<5, add 1 MP. If decreased and CP<5, add 1 CP. Select Family.");
            listFerocityFamilies = gs.Families.Where(f => f.IsActive && f.HasMan && !selectedF.Contains(f.Name)).ToList();
            List<ButtonInfo> bFamilies = new();
            //for each applicable family, create button using family name and index
            //for (int i = 0; i < listFerocityFamilies.Count; i++)
            foreach (Family family in listFerocityFamilies)
            {
                ButtonInfo bFamilyName = new(family.Name)
                {
                    family = family
                };
                bFamilies.Add(bFamilyName);
            }
            ui.MakeChoiceButtonsAsync(bFamilies);
        }


        async void ChangeFamilyFerocity2(ButtonInfo result)
        {
            // convert back to family name and add to end of list
            selectedF.Add(result.family.Name); //(listFerocityFamilies[result.tabIndex].Name);
            Family selectedFamily = result.family; //gs.Families.First(f => f.Name == result.text);
            ui.displayText($"{selectedFamily.Name} selected. ");
            ButtonInfo increase = new("Increase");
            ButtonInfo decrease = new("Decrease");
            List<ButtonInfo> UpDown = new() { increase, decrease };
            string MPremind = gs.MP < 5 ? "Increase will add 1 MP. " : "Increase will not add MP. ";
            string CPremind = gs.CP < 5 ? "Decrease will add 1 CP. " : "Decrease will not add CP. ";
            if (selectedFamily.Ferocity == 3)
            {
                ui.addText(CPremind);
                UpDown.Remove(increase);
            }
            else if (selectedFamily.Ferocity == 0)
            {
                ui.addText(MPremind);
                UpDown.Remove(decrease);
            }
            else
            {
                ui.addText($"Increase or Decrease Ferocity? " + MPremind + CPremind);
            }
            ui.MakeChoiceButtonsAsync(UpDown);
            ButtonInfo choiceUpDown = await IReceive.GetChoiceAsyncParams();
            if (choiceUpDown == increase)
            {
                selectedFamily.Ferocity++;
                gs.MP += gs.MP < 5 ? 1 : 0;
                ui.displayText($"{selectedFamily.Name} Ferocity is now {selectedFamily.Ferocity} and there are {gs.MP} MP.\n");
            }
            if (choiceUpDown == decrease)
            {
                selectedFamily.Ferocity--;
                gs.CP += gs.CP < 5 ? 1 : 0;
                ui.displayText($"{selectedFamily.Name} Ferocity is now {selectedFamily.Ferocity} and there are {gs.CP} CP.\n");
            }
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PlanningFour>().Begin();
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
