using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace NavajoWars
{
    public class PlayerActionLogic : OperationsLogic
    {
        Family selectedFamily;
        List<string> completedFamilies; 
        
        int numFamEligible;

        public enum oldGameSteps
        { clickedTakeActions, chooseFamily, undoChooseFamily, chooseAction, undoChooseAction, chooseAnotherAction, undoChooseAnotherAction, completeAction, completeFamily, clickedMove, resolveMove, undoMove, newTerritorySelected, clickedFindWater, removeDrought, undoRemoveDrought, clickedPlantorHarvestCorn, clickedTribalCouncil, undoTribalCouncil, clickedRaid, clickedTradeatFort, undoTradeatFort, doneActions }
        List<oldGameSteps> actionList;
        oldGameSteps selectedAction;
        oldGameSteps nextAction;
        oldGameSteps backAction;
        List<oldGameSteps> completedActions;

        Territory priorLocation;
        int priorTradeGoods;
        int priorFerocity;
        bool isTribalCouncilSuccess;

        // METHODS CALLED BELOW MUST BE PUBLIC??
        /*void nextActionFunc()
        {
            print("next Action is " + nextAction.ToString());
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(nextAction.ToString());
            chosenMethod?.Invoke(this, null);
        }

        void backActionFunc()
        {
            print("back Action is " + backAction.ToString());
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(backAction.ToString());
            chosenMethod?.Invoke(this, null);
        }

        void callActionFunc(oldGameSteps action)
        {
            print("called Action is " + action.ToString());
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(action.ToString());
            chosenMethod?.Invoke(this, null);
        }

        void Start()
        {
            print("Player Action Logic Start");
            actionList = new List<oldGameSteps>();
        }

        public void clickedTakeActions()
        {
            // initialize values then go to chooseFamily
            print("Take Actions");
            *//*actionList.Add(oldGameSteps.clickedTakeActions);
            ui.Initialize();
            ui.showBackNext();
            ui.OnOpsNextClick += nextActionFunc;
            ui.OnOpsBackClick += backActionFunc;*//*
            //ui.next.clicked += nextActionFunc;
            //ui.back.clicked += backActionFunc;
            int numFamilies = gs.Families.Count();
            int numElders = gs.ElderDisplay.Sum();
            int numFamInCan = gs.Families.Where(f => f.IsWhere == Territory.Canyon).Count();
            numFamEligible = Math.Min(numFamilies, numElders + Math.Max(numFamInCan, 1));
            completedFamilies = new();
            //chooseFamily();
        }
        
        *//*public async void chooseFamily()
        {
            print("chooseFamily");
            actionList.Add(oldGameSteps.chooseFamily);
            // choose family to activate
            // ADD POPUP FOR ACTION COSTS???
            //ui.displayHeadline("Player Actions";
            nextAction = oldGameSteps.doneActions;
            backAction = oldGameSteps.undoChooseFamily;
            if (numFamEligible > 0)
            {
                //ui.displayText($"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP";
                bool isFamMissing = gs.Families.Where(f => !completedFamilies.Contains(f.Name) && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    //ui.addText(", except:";
                    foreach (var family in gs.Families.Where(f => !completedFamilies.Contains(f.Name)))
                    {
                        int missing = numMissing(family);
                        if (missing > 0)
                        { 
                            //ui.addText($"\n{family.Name} has {6 - missing} MP";
                        }
                    }
                }
                //ui.addText(".\nChoose a family to activate, or press Next to end family actions.";
                List<string> listFamilyNames = gs.Families.Where(f => !completedFamilies.Contains(f.Name)).Select(f => f.Name).ToList();
                (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(listFamilyNames);

                string choiceText = result.choiceText.Replace("clicked", "");
                selectedFamily = gs.Families.First(f => f.Name == choiceText);
                completedActions = new();
                chooseAction();
            }
            else
            { 
                //ui.displayText("No more families may be activated. Press Next to continue."; 
            }
        }
*/

        /*public void chooseAction()
        {
            actionList.Add(oldGameSteps.chooseAction);
            backAction = oldGameSteps.undoChooseAction;
            nextAction = oldGameSteps.chooseAction;
            // next button just cycles back, need to choose action or press back
            // choose from list of valid actions 
            //ui.showBackNext();
            //ui.displayText($"Choose an action for {selectedFamily.Name}. ";
            List<string> validActions = new() { "Move", "Plant or Harvest Corn" };
            if (completedActions.Count() == 0) validActions.Add("Tribal Council");
            if (selectedFamily.HasMan && gs.MP > 0) validActions.Add("Raid");
            if (gs.HasDrought.Contains(selectedFamily.IsWhere)) validActions.Add("Find Water");
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.CP > 0 && (gs.TradeGoodsMax - gs.TradeGoodsHeld) > 0) validActions.Add("Trade at Fort");
            choice.DisplayChoiceButtons(this, validActions);
        }
*//*
        public void undoChooseAction()
        {
            //ui.displayText("";
            //ui.displayHeadline("";
            choice.CloseChoiceButtons();
            //if (completedActions.Count() == 0) chooseFamily();
            //else chooseAnotherAction();
            // can get to chooseAction only from chooseFamily, or from chooseAnotherAction
            // if from chooseFamily, then count=0
            // so else is only if from chooseAnotherAction        
        }

        public void completeAction()
        {
            // actionList.Add(Actions.completeAction); undo should not come back here
            completedActions.Add(selectedAction);
            chooseAnotherAction();
        }

        public async void chooseAnotherAction()
        {
            actionList.Add(oldGameSteps.chooseAnotherAction);
            nextAction = oldGameSteps.chooseAnotherAction;
            backAction = oldGameSteps.undoChooseAnotherAction;
            //ui.addText("If family has MPs remaining, you may select another Action. Otherwise, you may select another family to activate (or choose to end family actions). ";
            //ui.DisplayChoiceButtonsEvent(List);
            //(int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Action for Same Family", "Another Family or End" });
            //string choiceText = result.choiceText.Replace("clicked", "");
            //if (choiceText == "Action for Same Family") chooseAction();
            //else completeFamily(); 
        }

        public void undoChooseAnotherAction()
        {
            print("undoChooseAnotherAction");
            //ui.displayText("";
            //ui.displayHeadline("";
            choice.CloseChoiceButtons();
            completedActions.RemoveAt(completedActions.Count() - 1);
            oldGameSteps penultAction = actionList[actionList.Count() - 2];
            callActionFunc(penultAction);
        }

        public void completeFamily()
        {
            // actionList.Add(Actions.completeFamily);
            // completedActions.Add(selectedAction); // only if coming direct from action; include there
            completedFamilies.Add(selectedFamily.Name);
            numFamEligible--;
            chooseFamily();
        }

        public void clickedMove()
        {
            //display text info about move and wait for next or back click
            selectedAction = oldGameSteps.clickedMove;
            actionList.Add(oldGameSteps.clickedMove); // include, so return here
            choice.CloseChoiceButtons(); // if coming back from resolveMove
            bool horse = selectedFamily.HasHorse;
            bool fortInGame = gs.HasFort.Count() > 0;
            bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
            bool isFirstAction = completedActions.Count() == 0;
            int missing = numMissing(selectedFamily);
            //ui.displayHeadline($"Move {selectedFamily.Name}"; 
            //ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Move costs MPs equal to the destination Area's {(horse ? "parenthesized " : "")}value. If there is {(fortInGame ? $"a Fort in the destination Territory, or " : "")}an Outpost in an Area < = the destination Area, the MP cost is +1. ";
            string chellyMsg = isFirstAction ? "Move to or from Canyon de Chelly requires all MPs. " : "No move to or from Canyon because not family's first action. ";
            string fortMsg = (fortInGame && selectedFamily.Ferocity > 1) ? "A family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. " : ""; 
            if (fortInTerritory) fortMsg += isFirstAction ? "If currently in the same Area as a Fort, move only on first action and cost is increased by die roll." : "Move from same Area as a Fort only in first action.";            
            //ui.addText(chellyMsg + fortMsg;
            backAction = oldGameSteps.chooseAction; 
            nextAction = oldGameSteps.resolveMove;
        }

        public async void resolveMove()
        {
            actionList.Add(oldGameSteps.resolveMove); 
            backAction = oldGameSteps.clickedMove;
            nextAction = oldGameSteps.resolveMove;
            // next button just cycles, need to choose action or press back
            priorLocation = selectedFamily.IsWhere;
            bool isFirstAction = completedActions.Count() == 0;
            //ui.displayText($"Is {selectedFamily.Name} moving to a new Territory?";
            List<string> moveChoices = new() { "No Same Territory", "Yes New Territory" };
            if (isFirstAction) moveChoices.Add("Yes to Canyon de Chelly");
            ui.DisplayChoiceButtonsEvent(moveChoices);
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(moveChoices);

            string choiceText = result.choiceText.Replace("clicked", "");
            print("resolveMove: " + choiceText);
            backAction = oldGameSteps.resolveMove;
            if (choiceText == "No Same Territory")
            {
                //ui.displayText("Move completed. Press Next to continue. ";
                bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
                //if (selectedFamily.Ferocity > 1 && fortInTerritory) ui.addText("Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband.";
                nextAction = oldGameSteps.completeAction;
            }
            if (choiceText == "Yes New Territory")
            {
                //ui.displayText($"Select {selectedFamily.Name}'s new Territory and click Next to proceed.";
                choice.DisplayLocations();
                nextAction = oldGameSteps.newTerritorySelected;
            }
            if (choiceText == "Yes to Canyon de Chelly")
            {
                //ui.displayText("Move to Canyon de Chelly completed. Press Next to proceed.";
                selectedFamily.IsWhere = Territory.Canyon;
                completedActions.Add(selectedAction);
                backAction = oldGameSteps.undoMove;
                nextAction = oldGameSteps.completeFamily;                
            }            
        }

        public void newTerritorySelected()
        {
            actionList.Add(oldGameSteps.undoMove); // will go back there from choose Action
            //ui.displayText("";
            selectedFamily.IsWhere = (Territory)choice.locations.value + 1;
            choice.CloseLocations();
            completeAction();
        }
        
        public void undoMove()
        {
            completedActions.RemoveAt(completedActions.Count() - 1);
            if (selectedFamily.IsWhere == Territory.Canyon)
            {
                completedFamilies.Remove(selectedFamily.Name);
                numFamEligible++;
            }
            selectedFamily.IsWhere = priorLocation;
            resolveMove();
        }
        
        public void clickedFindWater()
        {
            actionList.Add(oldGameSteps.clickedFindWater);
            selectedAction = oldGameSteps.clickedFindWater;
            backAction = oldGameSteps.chooseAction;
            // bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere); //action not available if no drought
            bool horse = selectedFamily.HasHorse;
            int missing = numMissing(selectedFamily);
            bool isFirstAction = completedActions.Count() == 0;
            //ui.displayHeadline($"{selectedFamily.Name} Finds a New Water Hole";
            *//*if (!drought)
            {
                ui.displayText("There is no drought in this Family's Territory. Press Next to choose a different action.";
                nextAction = Actions.chooseAnotherAction;
            }
            else
            {*//*
                //ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Remove one Drought counter at cost of MPs equal to 9 minus value of current Area";
                //ui.addText(horse ? ", or parenthesized value if lower. " : ". ";
                //ui.addText("Press Back if insufficient MP, or Next to remove Drought.";
                nextAction = oldGameSteps.removeDrought;
            //}
        }

        public void removeDrought()
        {
            actionList.Add(oldGameSteps.undoRemoveDrought); // will go back there from choose Action
            gs.HasDrought.Remove(selectedFamily.IsWhere);
            completeAction();
        }

        public void undoRemoveDrought()
        {
            completedActions.RemoveAt(completedActions.Count() - 1);
            gs.HasDrought.Add(selectedFamily.IsWhere);
            clickedFindWater();
        }

        public void clickedPlantorHarvestCorn()
        {
            actionList.Add(oldGameSteps.clickedPlantorHarvestCorn);
            selectedAction = oldGameSteps.clickedPlantorHarvestCorn;
            bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere); 
            bool rancho = gs.HasRancho.Contains(selectedFamily.IsWhere);
            int missing = numMissing(selectedFamily);
            bool isFirstAction = completedActions.Count() == 0;
            //ui.displayHeadline($"{selectedFamily.Name} Plants or Harvests";
            //ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ";
            //ui.addText(drought || rancho ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ";
            //ui.addText("To harvest, reveal Corn counter and place it in resources. Press Next to continue.";
            // NECESSARY TO RECORD PLANT OR HARVEST?
            backAction = oldGameSteps.chooseAction;
            nextAction = oldGameSteps.completeAction;
        }

        public async void clickedTribalCouncil()
        {
            actionList.Add(oldGameSteps.clickedTribalCouncil);
            selectedAction = oldGameSteps.clickedTribalCouncil;
            backAction = oldGameSteps.chooseAction; 
            nextAction = oldGameSteps.clickedTribalCouncil;
            // next action recycles back if clicked inadvertently
            //ui.displayHeadline($"{selectedFamily.Name} Calls Tribal Council";
            //ui.displayText($"This must be {selectedFamily.Name}'s only action. If die roll is > = the family's current Area, collect 1 AP. Otherwise there is no effect. Was the die roll successful?";
            isTribalCouncilSuccess = false;            
            ui.DisplayChoiceButtonsEvent(List);
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Successful", "Not Successful" });
            string choiceText = result.choiceText.Replace("clicked", "");
            print("Tribal Council roll: " + choiceText);
            //ui.displayText("";
            if (choiceText == "Successful")
            {
                gs.AP++;
                //ui.addText("One AP added. ";
                isTribalCouncilSuccess = true;
            }
            //ui.addText("Press Next to Continue. ";
            completedActions.Add(selectedAction);
            actionList.Add(oldGameSteps.undoTribalCouncil);
            backAction = oldGameSteps.undoTribalCouncil;
            nextAction = oldGameSteps.completeFamily;
        }

        public void undoTribalCouncil()
        {
            if (isTribalCouncilSuccess) gs.AP--;
            completedActions.RemoveAt(completedActions.Count() - 1);
            clickedTribalCouncil();
        }

        public void clickedRaid()
        {
            actionList.Add(oldGameSteps.clickedRaid);
            selectedAction = oldGameSteps.clickedRaid;
            backAction = oldGameSteps.chooseAction;
            nextAction = oldGameSteps.clickedRaid; 
            //ui.displayHeadline($"{selectedFamily.Name} Raids New Mexico";
            //ui.displayText("Placeholder for raid. ";
            //ui.addText("Press Next to Continue.";
            completedActions.Add(selectedAction);
            nextAction = oldGameSteps.completeAction;
        }

        public async void clickedTradeatFort()
        {
            actionList.Add(oldGameSteps.clickedTradeatFort);
            selectedAction = oldGameSteps.clickedTradeatFort;
            backAction = oldGameSteps.chooseAction;
            nextAction = oldGameSteps.clickedTradeatFort;
            int tradeGoodsAvail = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            //ui.displayHeadline($"{selectedFamily.Name} Trades at Fort";
            //ui.displayText($"This action costs all remaining MP and 1 CP, will reduce {selectedFamily.Name}'s ferocity to zero, and {selectedFamily.Name} must be in the Fort. {(tradeGoodsAvail == 1 ? "One trade good is" : $"{tradeGoodsAvail} trade goods are")} available. Do you wish to trade?";
            ui.DisplayChoiceButtonsEvent(List);
            //(int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Yes Trade", "Do Not Trade" });
            string choiceText = result.choiceText.Replace("clicked", "");
            print("Trade response: " + choiceText);
            //ui.displayText("";
            if (choiceText == "Yes Trade")
            {
                completedActions.Add(selectedAction); 
                actionList.Add(oldGameSteps.undoTradeatFort); // back from completeFamily will go there
                backAction = oldGameSteps.undoTradeatFort;
                nextAction = oldGameSteps.completeFamily;
                priorTradeGoods = gs.TradeGoodsHeld;
                priorFerocity = selectedFamily.Ferocity;
                gs.TradeGoodsHeld = gs.TradeGoodsMax;
                selectedFamily.Ferocity = 0;
                gs.CP--;
                //ui.addText($"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. ";
            }
            else
            {
                backAction = oldGameSteps.clickedTradeatFort;
                nextAction = oldGameSteps.chooseAction;
            }
            //ui.addText("Press Next to Continue. ";
            //HAS TO BE LAST ACTION
        }

        public void undoTradeatFort()
        {
            completedActions.RemoveAt(completedActions.Count() - 1); 
            gs.TradeGoodsHeld = priorTradeGoods;
            selectedFamily.Ferocity = priorFerocity;
            gs.CP++;
            //ui.displayText($"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. {selectedFamily.Name}'s ferocity is {selectedFamily.Ferocity}";
            //ui.addText("Press Next to Continue. ";
            backAction = oldGameSteps.clickedTradeatFort;
            nextAction = oldGameSteps.chooseAction;
        }*/



        public void doneActions()
        {
            //ui.OnOpsNextClick -= nextActionFunc;
            //ui.OnOpsBackClick -= backActionFunc;
            //ui.PlayerOpsDone();
        }
    }
}