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
        GameState.Family selectedFamily;
        List<string> completedFamilies; 
        
        int numFamEligible;

        public enum Actions
        { clickedTakeActions, chooseFamily, undoChooseFamily, chooseAction, undoChooseAction, chooseAnotherAction, undoChooseAnotherAction, completeAction, completeFamily, clickedMove, resolveMove, undoMove, newTerritorySelected, clickedFindWater, removeDrought, undoRemoveDrought, clickedPlantorHarvestCorn, clickedTribalCouncil, undoTribalCouncil, clickedRaid, clickedTradeatFort, undoTradeatFort, doneActions }
        List<Actions> actionList;
        Actions selectedAction;
        Actions nextAction;
        Actions backAction;
        List<Actions> completedActions;

        Territory priorLocation;
        int priorTradeGoods;
        int priorFerocity;
        bool isTribalCouncilSuccess;

        // METHODS CALLED BELOW MUST BE PUBLIC??
        void nextActionFunc()
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

        void callActionFunc(Actions action)
        {
            print("called Action is " + action.ToString());
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(action.ToString());
            chosenMethod?.Invoke(this, null);
        }

        void Start()
        {
            print("Player Action Logic Start");
            actionList = new List<Actions>();
        }

        public void clickedTakeActions()
        {
            // initialize values then go to chooseFamily
            print("Take Actions");
            actionList.Add(Actions.clickedTakeActions);
            ui.Initialize();
            ui.showBackNext();
            ui.OnOpsNextClick += nextActionFunc;
            ui.OnOpsBackClick += backActionFunc;
            //ui.next.clicked += nextActionFunc;
            //ui.back.clicked += backActionFunc;
            int numFamilies = gs.Families.Count();
            int numElders = gs.ElderDisplay.Sum();
            int numFamInCan = gs.Families.Where(f => f.IsWhere == Territory.Canyon).Count();
            numFamEligible = Math.Min(numFamilies, numElders + Math.Max(numFamInCan, 1));
            completedFamilies = new();
            chooseFamily();
        }
        
        public async void chooseFamily()
        {
            print("chooseFamily");
            actionList.Add(Actions.chooseFamily);
            // choose family to activate
            // ADD POPUP FOR ACTION COSTS???
            ui.headline.text = "Player Actions";
            nextAction = Actions.doneActions;
            backAction = Actions.undoChooseFamily;
            if (numFamEligible > 0)
            {
                ui.message.text = $"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP";
                bool isFamMissing = gs.Families.Where(f => !completedFamilies.Contains(f.Name) && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    ui.message.text += ", except:";
                    foreach (var family in gs.Families.Where(f => !completedFamilies.Contains(f.Name)))
                    {
                        int missing = numMissing(family);
                        if (missing > 0)
                        { ui.message.text += $"\n{family.Name} has {6 - missing} MP"; }
                    }
                }
                ui.message.text += ".\nChoose a family to activate, or press Next to end family actions.";
                List<string> listFamilyNames = gs.Families.Where(f => !completedFamilies.Contains(f.Name)).Select(f => f.Name).ToList();
                (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(listFamilyNames);

                string choiceText = result.choiceText.Replace("clicked", "");
                selectedFamily = gs.Families.First(f => f.Name == choiceText);
                completedActions = new();
                chooseAction();
            }
            else
            { ui.message.text = "No more families may be activated. Click Next to continue."; }
        }

        public void undoChooseFamily()
        {
            print("undoChooseFamily");
            ui.message.text = "";
            ui.headline.text = "";
            choice.CloseChoiceButtons();
            if (completedActions.Count() == 0)
            {
                ui.PlayerOperation();
            }
            else
            {
                completedFamilies.Remove(selectedFamily.Name);
                numFamEligible++;
                Actions penultAction = actionList[actionList.Count() - 2];
                callActionFunc(penultAction);
            };
            // can get to chooseFamily only initially, from completeFamily or from undoChooseAction
            // if initially or undoChooseAction, then count=0
            // so else is only if from completeFamily - so "undocompleteFamily"
        }

        public void chooseAction()
        {
            actionList.Add(Actions.chooseAction);
            backAction = Actions.undoChooseAction;
            nextAction = Actions.chooseAction;
            // next button just cycles back, need to choose action or press back
            // choose from list of valid actions 
            ui.showBackNext();
            ui.message.text = $"Choose an action for {selectedFamily.Name}. ";
            List<string> validActions = new() { "Move", "Plant or Harvest Corn" };
            if (completedActions.Count() == 0) validActions.Add("Tribal Council");
            if (selectedFamily.HasMan && gs.MP > 0) validActions.Add("Raid");
            if (gs.HasDrought.Contains(selectedFamily.IsWhere)) validActions.Add("Find Water");
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.CP > 0 && (gs.TradeGoodsMax - gs.TradeGoodsHeld) > 0) validActions.Add("Trade at Fort");
            choice.DisplayChoiceButtons(this, validActions);
        }

        public void undoChooseAction()
        {
            ui.message.text = "";
            ui.headline.text = "";
            choice.CloseChoiceButtons();
            if (completedActions.Count() == 0) chooseFamily();
            else chooseAnotherAction();
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
            actionList.Add(Actions.chooseAnotherAction);
            nextAction = Actions.chooseAnotherAction;
            backAction = Actions.undoChooseAnotherAction;
            ui.message.text += "If family has MPs remaining, you may select another Action. Otherwise, you may select another family to activate (or choose to end family actions). ";
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Action for Same Family", "Another Family or End" });
            string choiceText = result.choiceText.Replace("clicked", "");
            if (choiceText == "Action for Same Family") chooseAction();
            else completeFamily(); 
        }

        public void undoChooseAnotherAction()
        {
            print("undoChooseAnotherAction");
            ui.message.text = "";
            ui.headline.text = "";
            choice.CloseChoiceButtons();
            completedActions.RemoveAt(completedActions.Count() - 1);
            Actions penultAction = actionList[actionList.Count() - 2];
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
            selectedAction = Actions.clickedMove;
            actionList.Add(Actions.clickedMove); // include, so return here
            choice.CloseChoiceButtons(); // if coming back from resolveMove
            bool horse = selectedFamily.HasHorse;
            bool fortInGame = gs.HasFort.Count() > 0;
            bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
            bool isFirstAction = completedActions.Count() == 0;
            int missing = numMissing(selectedFamily);
            ui.headline.text = $"Move {selectedFamily.Name}"; 
            ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Move costs MPs equal to the destination Area's {(horse ? "parenthesized " : "")}value. If there is {(fortInGame ? $"a Fort in the destination Territory, or " : "")}an Outpost in an Area < = the destination Area, the MP cost is +1. ";
            string chellyMsg = isFirstAction ? "Move to or from Canyon de Chelly requires all MPs. " : "No move to or from Canyon because not family's first action. ";
            string fortMsg = (fortInGame && selectedFamily.Ferocity > 1) ? "A family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. " : ""; 
            if (fortInTerritory) fortMsg += isFirstAction ? "If currently in the same Area as a Fort, move only on first action and cost is increased by die roll." : "Move from same Area as a Fort only in first action.";            
            ui.message.text += chellyMsg + fortMsg;
            backAction = Actions.chooseAction; 
            nextAction = Actions.resolveMove;
        }

        public async void resolveMove()
        {
            actionList.Add(Actions.resolveMove); 
            backAction = Actions.clickedMove;
            nextAction = Actions.resolveMove;
            // next button just cycles, need to choose action or press back
            priorLocation = selectedFamily.IsWhere;
            bool isFirstAction = completedActions.Count() == 0;
            ui.message.text = $"Is {selectedFamily.Name} moving to a new Territory?";
            List<string> moveChoices = new() { "No Same Territory", "Yes New Territory" };
            if (isFirstAction) moveChoices.Add("Yes to Canyon de Chelly");
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(moveChoices);

            string choiceText = result.choiceText.Replace("clicked", "");
            print("resolveMove: " + choiceText);
            backAction = Actions.resolveMove;
            if (choiceText == "No Same Territory")
            {
                ui.message.text = "Move completed. Press Next to continue. ";
                bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
                if (selectedFamily.Ferocity > 1 && fortInTerritory) ui.message.text += "Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband.";
                nextAction = Actions.completeAction;
            }
            if (choiceText == "Yes New Territory")
            {
                ui.message.text = $"Select {selectedFamily.Name}'s new Territory and click Next to proceed.";
                choice.DisplayLocations();
                nextAction = Actions.newTerritorySelected;
            }
            if (choiceText == "Yes to Canyon de Chelly")
            {
                ui.message.text = "Move to Canyon de Chelly completed. Click Next to proceed.";
                selectedFamily.IsWhere = Territory.Canyon;
                completedActions.Add(selectedAction);
                backAction = Actions.undoMove;
                nextAction = Actions.completeFamily;                
            }            
        }

        public void newTerritorySelected()
        {
            actionList.Add(Actions.undoMove); // will go back there from choose Action
            ui.message.text = "";
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
            actionList.Add(Actions.clickedFindWater);
            selectedAction = Actions.clickedFindWater;
            backAction = Actions.chooseAction;
            // bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere); //action not available if no drought
            bool horse = selectedFamily.HasHorse;
            int missing = numMissing(selectedFamily);
            bool isFirstAction = completedActions.Count() == 0;
            ui.headline.text = $"{selectedFamily.Name} Finds a New Water Hole";
            /*if (!drought)
            {
                ui.message.text = "There is no drought in this Family's Territory. Press Next to choose a different action.";
                nextAction = Actions.chooseAnotherAction;
            }
            else
            {*/
                ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Remove one Drought counter at cost of MPs equal to 9 minus value of current Area";
                ui.message.text += horse ? ", or parenthesized value if lower. " : ". ";
                ui.message.text += "Press Back if insufficient MP, or Next to remove Drought.";
                nextAction = Actions.removeDrought;
            //}
        }

        public void removeDrought()
        {
            actionList.Add(Actions.undoRemoveDrought); // will go back there from choose Action
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
            actionList.Add(Actions.clickedPlantorHarvestCorn);
            selectedAction = Actions.clickedPlantorHarvestCorn;
            bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere); 
            bool rancho = gs.HasRancho.Contains(selectedFamily.IsWhere);
            int missing = numMissing(selectedFamily);
            bool isFirstAction = completedActions.Count() == 0;
            ui.headline.text = $"{selectedFamily.Name} Plants or Harvests";
            ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ";
            ui.message.text += drought || rancho ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ";
            ui.message.text += "To harvest, reveal Corn counter and place it in resources. Press Next to continue.";
            // NECESSARY TO RECORD PLANT OR HARVEST?
            backAction = Actions.chooseAction;
            nextAction = Actions.completeAction;
        }

        public async void clickedTribalCouncil()
        {
            actionList.Add(Actions.clickedTribalCouncil);
            selectedAction = Actions.clickedTribalCouncil;
            backAction = Actions.chooseAction; 
            nextAction = Actions.clickedTribalCouncil;
            // next action recycles back if clicked inadvertently
            ui.headline.text = $"{selectedFamily.Name} Calls Tribal Council";
            ui.message.text = $"This must be {selectedFamily.Name}'s only action. If die roll is > = the family's current Area, collect 1 AP. Otherwise there is no effect. Was the die roll successful?";
            isTribalCouncilSuccess = false;
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Successful", "Not Successful" });
            string choiceText = result.choiceText.Replace("clicked", "");
            print("Tribal Council roll: " + choiceText);
            ui.message.text = "";
            if (choiceText == "Successful")
            {
                gs.AP++;
                ui.message.text += "One AP added. ";
                isTribalCouncilSuccess = true;
            }
            ui.message.text += "Press Next to Continue. ";
            completedActions.Add(selectedAction);
            actionList.Add(Actions.undoTribalCouncil);
            backAction = Actions.undoTribalCouncil;
            nextAction = Actions.completeFamily;
        }

        public void undoTribalCouncil()
        {
            if (isTribalCouncilSuccess) gs.AP--;
            completedActions.RemoveAt(completedActions.Count() - 1);
            clickedTribalCouncil();
        }

        public void clickedRaid()
        {
            actionList.Add(Actions.clickedRaid);
            selectedAction = Actions.clickedRaid;
            backAction = Actions.chooseAction;
            nextAction = Actions.clickedRaid; 
            ui.headline.text = $"{selectedFamily.Name} Raids New Mexico";
            ui.message.text = "Placeholder for raid. ";
            ui.message.text += "Press Next to Continue.";
            completedActions.Add(selectedAction);
            nextAction = Actions.completeAction;
        }

        public async void clickedTradeatFort()
        {
            actionList.Add(Actions.clickedTradeatFort);
            selectedAction = Actions.clickedTradeatFort;
            backAction = Actions.chooseAction;
            nextAction = Actions.clickedTradeatFort;
            int tradeGoodsAvail = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            ui.headline.text = $"{selectedFamily.Name} Trades at Fort";
            ui.message.text = $"This action costs all remaining MP and 1 CP, will reduce {selectedFamily.Name}'s ferocity to zero, and {selectedFamily.Name} must be in the Fort. {(tradeGoodsAvail == 1 ? "One trade good is" : $"{tradeGoodsAvail} trade goods are")} available. Do you wish to trade?";
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Yes Trade", "Do Not Trade" });
            string choiceText = result.choiceText.Replace("clicked", "");
            print("Trade response: " + choiceText);
            ui.message.text = "";
            if (choiceText == "Yes Trade")
            {
                completedActions.Add(selectedAction); 
                actionList.Add(Actions.undoTradeatFort); // back from completeFamily will go there
                backAction = Actions.undoTradeatFort;
                nextAction = Actions.completeFamily;
                priorTradeGoods = gs.TradeGoodsHeld;
                priorFerocity = selectedFamily.Ferocity;
                gs.TradeGoodsHeld = gs.TradeGoodsMax;
                selectedFamily.Ferocity = 0;
                gs.CP--;
                ui.message.text += $"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. ";
            }
            else
            {
                backAction = Actions.clickedTradeatFort;
                nextAction = Actions.chooseAction;
            }
            ui.message.text += "Press Next to Continue. ";
        }

        public void undoTradeatFort()
        {
            completedActions.RemoveAt(completedActions.Count() - 1); 
            gs.TradeGoodsHeld = priorTradeGoods;
            selectedFamily.Ferocity = priorFerocity;
            gs.CP++;
            ui.message.text = $"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. {selectedFamily.Name}'s ferocity is {selectedFamily.Ferocity}";
            ui.message.text += "Press Next to Continue. ";
            backAction = Actions.clickedTradeatFort;
            nextAction = Actions.chooseAction;
        }

        int numMissing(GameState.Family family)
        {
            int missing = 0;
            if (!family.HasMan) missing++;
            if (!family.HasWoman) missing++;
            if (!family.HasChild) missing++;
            return missing;
        }

        public void doneActions()
        {
            ui.OnOpsNextClick -= nextActionFunc;
            ui.OnOpsBackClick -= backActionFunc;
            ui.PlayerOpsDone();
        }
    }
}