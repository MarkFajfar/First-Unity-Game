using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{ 
    public class ChooseFamily : GameStep
    {
        public override string stepName { get => "ChooseFamily"; }

        string saveState;
        
        int numFamEligible;
        int numFamilies;
        int numElders;
        int numFamInCan;
        ChooseAction chooseAction;

        public override void Begin()
        {
            //if (caller.stepName == "InitialUndo" || caller.stepName == "ChooseFamily") // if coming from choose player operation or from chooseFamily (below) or from player ops done
            // should this be for all cases? only undo point and completed families are different?
            //{ 
                // set first undo target to caller
                // so pressing "back" before first button sends back to prior undo point
                // allows chain of undo ??
                // or set undo or back button in caller?  or set undo after family selected?
                //logic.setUndo(this, caller);
                
                // reset list of completed families only if coming from choose player operation 
            GameStep caller = gs.stepStack.Peek();
            if (caller.stepName == "PlayerOperation") gs.completedFamilies = new();
            // saveState immediately before calling action
            saveState = JsonUtility.ToJson(gs);
            chooseFamily();
            //}
        }

        async void chooseFamily()
        {
            // choose family to activate
            // ADD POPUP FOR ACTION COSTS???
            ui.Initialize();
            ui.displayHeadline("Player Actions");
            numFamilies = gs.Families.Count;
            numElders = gs.ElderDisplay.Sum();
            numFamInCan = gs.Families.Where(f => f.IsWhere == Territory.Canyon).Count();
            numFamEligible = Math.Min(numFamilies, numElders + Math.Max(numFamInCan, 1));
            numFamEligible -= gs.completedFamilies.Count;
            if (numFamEligible > 0)
            {
                ui.displayText($"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP");
                bool isFamMissing = gs.Families.Where(f => !gs.completedFamilies.Contains(f) && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    ui.addText(", except:");
                    foreach (var family in gs.Families.Where(f => !gs.completedFamilies.Contains(f)))
                    {
                        int missing = numMissing(family);
                        if (missing > 0)
                        {
                            ui.addText($"\n{family.Name} has {6 - missing} MP");
                        }
                    }
                }
                ui.addText(".\nChoose a family to activate");
                if (gs.completedFamilies.Count >0) 
                {
                    ui.addText(", or press Next to end Player Actions.\"");
                    ui.OnOpsNextClick += playerOpsDone;
                }
                else
                {
                    ui.addText(".");
                    ui.unsubNext(); // clear next button, just in case
                }
                //create list of eligible families
                //List<string> listFamilyNames = gs.Families.Where(f => !gs.completedFamilies.Contains(f)).Select(f => f.Name).ToList();
                List<GameState.Family> listFamEligible = gs.Families.Where(f => !gs.completedFamilies.Contains(f)).ToList();
                //initialize list of buttons
                List<bParams> bFamEligible = new List<bParams>();
                //for each elibible family, create button using family name and index
                for (int i = 0; i < listFamEligible.Count; i++)
                {
                    bParams bFamilyName = new(listFamEligible[i].Name, i);
                    bFamEligible.Add(bFamilyName);
                }
                /*foreach (GameState.Family family in listFamEligible)
                {
                    bParams bFamilyName = new(family.Name);
                }*/

                ui.DisplayChoiceButtonsEvent(bFamEligible);
                (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(bFamEligible);

                gs.selectedFamily = listFamEligible[result.choiceIndex];
                //gs.selectedFamily = gs.Families.First(f => f.Name == result.choiceText);
                chooseAction = GetComponent<ChooseAction>();
                // push to stepStack immediately before calling enxt action
                gs.stepStack.Push(this);
                chooseAction.Begin();
            }
            else
            {
                ui.OnOpsNextClick += playerOpsDone;
                ui.displayText("No more families may be activated. Click Next to continue.");
            }
        }

        void playerOpsDone() 
        {
            ui.OnOpsNextClick -= playerOpsDone;
            // is it necessary to cancel task?
            // push to stepStack immediately before calling next action
            gs.stepStack.Push(this);
            logic.instructFromStep(this, "PlayerOpsDone");
        }

        int numMissing(GameState.Family family)
        {
            int missing = 0;
            if (!family.HasMan) missing++;
            if (!family.HasWoman) missing++;
            if (!family.HasChild) missing++;
            return missing;
        }

        public override void Undo()
        {
            // stuff to do on undo
            // overwrite saveState immediately before calling action
            JsonUtility.FromJsonOverwrite(saveState, gs);
            chooseFamily();
        }
    }
}
