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

        int numFamEligible;
        int numFamilies;
        int numElders;
        int numFamInCan;
        ChooseAction chooseAction;

        /*public event EventHandler<bParamsEventArgs> CreateButtonsEvent;

        protected virtual void CreateButtons(bParamsEventArgs e)
        {
            EventHandler<bParamsEventArgs> raiseEvent = CreateButtonsEvent;
            raiseEvent?.Invoke(this, e);
        }*/

        public override void Begin()
        {
            // reset completed families only if coming from choose player operation 
            if (gs.stepStack.Count > 0 && gs.stepStack.Peek().stepName == "PlayerOperation") 
            {
                gs.completedFamilies = 0;
                gs.completedActions = 0;
                foreach (var family in gs.Families)
                {
                    family.isSelectedOps = false;
                    family.isCompletedOps = false;
                }
                clearCompleted?.Invoke();
            }
            // saveState immediately before calling action
            gm.SaveUndo(this);
            chooseFamily();
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
            numFamEligible -= gs.completedFamilies;
            if (numFamEligible > 0)
            {
                ui.displayText($"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP");
                bool isFamMissing = gs.Families.Where(f => !f.isCompletedOps && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    ui.addText(", except:");
                    foreach (var family in gs.Families.Where(f => !f.isCompletedOps))
                    {
                        int missing = OperationsLogic.numMissing(family);
                        if (missing > 0)
                        {
                            ui.addText($"\n{family.Name} has {6 - missing} MP");
                        }
                    }
                }
                ui.addText(".\nChoose a family to activate");
                if (gs.completedFamilies > 0) 
                {
                    ui.addText(", or press Next to end Player Actions.\"");
                    ui.OnNextClick += playerOpsDone;
                }
                else
                {
                    ui.addText(".");
                    ui.unsubNext(); // clear next button, just in case
                }
                //create list of eligible families
                //List<string> listFamilyNames = gs.Families.Where(f => !gs.completedFamilies.Contains(f)).Select(f => f.Name).ToList();
                List<GameState.Family> listFamEligible = gs.Families.Where(f => !f.isCompletedOps).ToList();
                //initialize list of buttons
                List<ButtonInfo> bFamEligible = new List<ButtonInfo>();
                //for each eligible family, create button using family name and index
                for (int i = 0; i < listFamEligible.Count; i++)
                {
                    ButtonInfo bFamilyName = new(listFamEligible[i].Name, i);
                    bFamEligible.Add(bFamilyName);
                }
                /*// TEST CREATE BUTTON EVENT
                CreateButtons(new bParamsEventArgs(bFamEligible));
                calltCB(bFamEligible);*/
                // use async because logic to apply to result
                ui.MakeChoiceButtonsAsync(bFamEligible);
                ButtonInfo result = await IReceive.GetChoiceAsync();

                listFamEligible[result.tabIndex].isSelectedOps = true;
                
                chooseAction = GetComponent<ChooseAction>();
                // push to stepStack immediately before calling next action
                gs.stepStack.Push(this);
                chooseAction.Begin();
            }
            else
            {
                ui.OnNextClick += playerOpsDone;
                ui.displayText("No more families may be activated. Press Next to continue.");
            }
        }

        void playerOpsDone() 
        {
            ui.OnNextClick -= playerOpsDone;
            // is it necessary to cancel task?
            // push to stepStack immediately before calling next action
            gs.stepStack.Push(this);
            logic.instructFromStep(this, "PlayerOpsDone");
        }

        public override void Undo()
        {
            // stuff to do on undo
            // overwrite saveState immediately before calling action
            //JsonUtility.FromJsonOverwrite(saveState, gs);
            gm.LoadUndo(this);
            chooseFamily();
        }
    }
}
