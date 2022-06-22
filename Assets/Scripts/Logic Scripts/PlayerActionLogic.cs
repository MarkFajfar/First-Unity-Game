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
        List<string> selectedF = new();
        int numFamEligible;
        bool isAnyAction;
        bool isFirstAction;
        public enum Actions
        { clickedTakeActions, chooseFamily, undoChooseFamily, chooseAction, undoChooseAction, chooseAnotherAction, undoChooseAnotherAction, clickedMove, resolveMove, undoMove, newTerritorySelected, clickedFindWater, removeDrought, undoRemoveDrought, clickedPlantorHarvestCorn, clickedTribalCouncil, clickedRaid, clickedTradeatFort, undoTradeatFort, doneActions }
        List<Actions> actionList;
        Actions currentAction;
        Actions nextAction;
        Actions backAction;
        Territory priorLocation;
        int priorTradeGoods;
        int priorFerocity;

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

        void currentActionFunc()
        {
            print("current Action is " + currentAction.ToString());
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(currentAction.ToString());
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
            numFamEligible = 1 + Math.Min(numFamilies, numElders + Math.Max(numFamInCan, 1));
            isFirstAction = true;
            isAnyAction = false;
            chooseFamily();
        }
        
        public async void chooseFamily()
        {
            print("chooseFamily");
            actionList.Add(Actions.chooseFamily);
            // choose family to activate
            // ADD POPUP FOR ACTION COSTS???
            ui.headline.text = "Player Actions";
            numFamEligible--;
            currentAction = Actions.chooseFamily;
            nextAction = Actions.doneActions;
            backAction = Actions.undoChooseFamily;
            if (numFamEligible > 0)
            {
                ui.message.text = $"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP";
                bool isFamMissing = gs.Families.Where(f => !selectedF.Contains(f.Name) && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    ui.message.text += ", except:";
                    foreach (var family in gs.Families.Where(f => !selectedF.Contains(f.Name)))
                    {
                        int missing = numMissing(family);
                        if (missing > 0)
                        { ui.message.text += $"\n{family.Name} has {6 - missing} MP"; }
                    }
                }
                ui.message.text += ".\nChoose a family to activate, or press Next to end family actions.";
                List<string> listFamilyNames = gs.Families.Where(f => !selectedF.Contains(f.Name)).Select(f => f.Name).ToList();
                (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(listFamilyNames);
                string choiceText = result.choiceText.Replace("clicked", "");
                numFamEligible--;
                selectedF.Add(choiceText);
                selectedFamily = gs.Families.First(f => f.Name == choiceText);
                isFirstAction = true;
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
            choice.CloseChoices();
            if (!isAnyAction) 
            {
                numFamEligible++;
                ui.PlayerOperation(); 
            }
            else
            {
                Actions penultAction = actionList[actionList.Count() - 2];
                callActionFunc(penultAction);
            };
        }

        public void chooseAction()
        {
            actionList.Add(Actions.chooseAction);
            currentAction = Actions.chooseAction;
            backAction = Actions.undoChooseAction;
            nextAction = Actions.chooseAction;
            // next button just cycles back, need to choose action or press back
            // choose from list of valid actions 
            // ui.OnOpsNextClick -= ui.PlayerOpsDone;
            ui.showBackNext();
            ui.message.text = $"Choose an action for {selectedFamily.Name}.";
            List<string> validActions = new() { "Move", "Plant or Harvest Corn" };
            if (isFirstAction) validActions.Add("Tribal Council");
            if (selectedFamily.HasMan && gs.MP > 0) validActions.Add("Raid");
            if (gs.HasDrought.Contains(selectedFamily.IsWhere)) validActions.Add("Find Water");
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.CP > 0 && (gs.TradeGoodsMax - gs.TradeGoodsHeld) > 0) validActions.Add("Trade at Fort");
            choice.DisplayChoices(this, validActions);
        }

        public void undoChooseAction()
        {
            ui.message.text = "";
            ui.headline.text = "";
            choice.CloseChoices();
            if (isFirstAction)
            {
                // if family hasn't taken an action, "unselect"
                selectedF.Remove(selectedFamily.Name);
                numFamEligible++;
            }
            chooseFamily();        
        }

        public async void chooseAnotherAction()
        {
            actionList.Add(Actions.chooseAnotherAction);
            nextAction = Actions.chooseAnotherAction;
            backAction = Actions.undoChooseAnotherAction;
            isFirstAction = false;
            isAnyAction = true;
            ui.message.text += "If family has MPs remaining, you may select another Action. Otherwise, you may select another family to activate (or choose to end family actions. ";
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Action for Same Family", "Select Another Family" });
            string choiceText = result.choiceText.Replace("clicked", "");
            if (choiceText == "Action for Same Family") chooseAction();
            else chooseFamily(); 
        }

        public void undoChooseAnotherAction()
        {
            print("undoChooseAnotherAction");
            ui.message.text = "";
            ui.headline.text = "";
            choice.CloseChoices();
            Actions penultAction = actionList[actionList.Count() - 2];
            callActionFunc(penultAction);
        }

        public void clickedMove()
        {
            //display text info about move and wait for next or back click
            //ui.OnOpsBackClick -= clickedMove; 
            //ui.OnOpsBackClick += StepFour;
            choice.CloseChoices(); // if cominb back from resolveMove
            bool horse = selectedFamily.HasHorse;
            bool fortInGame = gs.HasFort.Count() > 0;
            bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
            int missing = numMissing(selectedFamily);
            ui.headline.text = $"Move {selectedFamily.Name}"; 
            ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Move costs MPs equal to the destination Area's {(horse ? "parenthesized " : "")}value. If there is {(fortInGame ? $"a Fort in the destination Territory, or " : "")}an Outpost in an Area < = the destination Area, the MP cost is +1. ";
            string chellyMsg = isFirstAction ? "Move to or from Canyon de Chelly requires all MPs. " : "No move to or from Canyon because not family's first action. ";
            string fortMsg = (fortInGame && selectedFamily.Ferocity > 1) ? "A family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. " : ""; 
            if (fortInTerritory) fortMsg += isFirstAction ? "If currently in the same Area as a Fort, move only on first action and cost is increased by die roll." : "Move from same Area as a Fort only in first action.";            
            ui.message.text += chellyMsg + fortMsg;
            backAction = Actions.chooseAction; 
            nextAction = Actions.resolveMove;
            ui.showBackNext();
        }

        public async void resolveMove()
        {
            //ui.OnOpsNextClick -= resolveMove;
            //ui.OnOpsBackClick -= StepFour;
            actionList.Add(Actions.resolveMove); // will come back here from choose Action
            backAction = Actions.clickedMove;
            nextAction = Actions.resolveMove;
            // next button just cycles back, need to choose action or press back
            priorLocation = selectedFamily.IsWhere;
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
                isAnyAction = true;
                isFirstAction = false;
                nextAction = Actions.chooseAnotherAction;
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
                isAnyAction = true;
                isFirstAction = false;
                backAction = Actions.undoMove;
                nextAction = Actions.chooseFamily;
            }            
        }

        public void newTerritorySelected()
        {
            actionList.Add(Actions.undoMove); // will go back there from choose Action
            ui.message.text = "";
            //ui.OnOpsNextClick -= newTerritorySelected;
            selectedFamily.IsWhere = (Territory)choice.locations.value + 1;
            choice.CloseLocations();
            isAnyAction = true;
            isFirstAction = false;
            chooseAnotherAction();
        }
        
        public void undoMove()
        {
            selectedFamily.IsWhere = priorLocation;
            // WHAT ABOUT ANY AND FIRST ACTION?? use a counter?
            resolveMove();
        }
        
        public void clickedFindWater()
        {
            actionList.Add(Actions.clickedFindWater);
            currentAction = Actions.clickedFindWater;
            backAction = Actions.undoChooseAction;
            bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere);
            bool horse = selectedFamily.HasHorse;
            int missing = numMissing(selectedFamily);
            ui.headline.text = $"{selectedFamily.Name} Finds a New Water Hole";
            if (!drought)
            {
                ui.message.text = "There is no drought in this Family's Territory. Press Next to choose a different action.";
                nextAction = Actions.chooseAnotherAction;
            }
            else
            {
                ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Remove one Drought counter at cost of MPs equal to 9 minus value of current Area";
                ui.message.text += horse ? ", or parenthesized value if lower. " : ". ";
                ui.message.text += "Press Back if insufficient MP, or Next to remove Drought.";
                nextAction = Actions.removeDrought;
            }
        }

        public void removeDrought()
        {
            actionList.Add(Actions.undoRemoveDrought); // will go back there from choose Action
            currentAction = Actions.removeDrought;
            gs.HasDrought.Remove(selectedFamily.IsWhere);
            isAnyAction = true;
            isFirstAction = false;
            chooseAnotherAction();
        }

        public void undoRemoveDrought()
        {
            gs.HasDrought.Add(selectedFamily.IsWhere);
            // WHAT ABOUT ANY AND FIRST ACTION?? same issue as resolveMove
            clickedFindWater();
        }

        public void clickedPlantorHarvestCorn()
        {
            bool drought = gs.HasDrought.Contains(selectedFamily.IsWhere); 
            bool rancho = gs.HasRancho.Contains(selectedFamily.IsWhere);
            int missing = numMissing(selectedFamily);
            ui.headline.text = $"{selectedFamily.Name} Plants or Harvests";
            ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Action costs MPs equal to 4 plus value of current Area. Reminder: only 1 Corn counter per Area, and if Family leaves an Area containing Corn, it is immediately returned to draw cup. To plant, draw 1 Corn counter and place it face down in Family's Area. ";
            ui.message.text += drought || rancho ? "Harvest requires die roll > number of drought and rancho counters in Territory. " : "Harvest does not require a die roll. ";
            ui.message.text += "To harvest, reveal Corn counter and place it in resources. Press Next to continue.";
            // NECESSARY TO RECORD PLANT OR HARVEST?
            isAnyAction = true;
            isFirstAction = false;
            nextAction = Actions.chooseAnotherAction;
        }

        public async void clickedTribalCouncil()
        {
            nextAction = Actions.clickedTribalCouncil;
            // next action recycles back if clicked inadvertently
            ui.headline.text = $"{selectedFamily.Name} Calls Tribal Council";
            ui.message.text = $"This must be {selectedFamily.Name}'s only action. If die roll is > = the family's current Area, collect 1 AP. Otherwise there is no effect. Was the die roll successful?";
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(new List<string> { "Successful", "Not Successful" });
            string choiceText = result.choiceText.Replace("clicked", "");
            print("Tribal Council roll: " + choiceText);
            ui.message.text = "";
            if (choiceText == "Successful")
            {
                gs.AP++;
                ui.message.text += "One AP added. ";
            }
            ui.message.text += "Press Next to Continue. ";
            isAnyAction = true;
            isFirstAction = false;
            nextAction = Actions.chooseFamily;
        }

        public void clickedRaid()
        {
            actionList.Add(Actions.clickedRaid);
            currentAction = Actions.clickedRaid;
            backAction = Actions.undoChooseAction;
            nextAction = Actions.clickedRaid; 
            ui.headline.text = $"{selectedFamily.Name} Raids New Mexico";
            ui.message.text = "Placeholder for raid. ";
            ui.message.text += "Press Next to Continue. ";
            isAnyAction = true;
            isFirstAction = false;
            nextAction = Actions.chooseFamily;
        }

        public async void clickedTradeatFort()
        {
            actionList.Add(Actions.clickedTradeatFort);
            currentAction = Actions.clickedTradeatFort;
            backAction = Actions.undoChooseAction;
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
                actionList.Add(Actions.undoTradeatFort); // back action will go there
                backAction = Actions.undoTradeatFort;
                priorTradeGoods = gs.TradeGoodsHeld;
                priorTradeGoods = selectedFamily.Ferocity;
                gs.TradeGoodsHeld = gs.TradeGoodsMax;
                selectedFamily.Ferocity = 0;
                gs.CP--;
                ui.message.text += $"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. ";
                isAnyAction = true;
                isFirstAction = false;
                nextAction = Actions.chooseFamily;
            }
            if (choiceText == "Do Not Trade")
            {
                nextAction = Actions.chooseFamily;
            }
            ui.message.text += "Press Next to Continue. ";
        }

        public void undoTradeatFort()
        {
            gs.TradeGoodsHeld = priorTradeGoods;
            selectedFamily.Ferocity = priorFerocity;
            gs.CP++;
            ui.message.text = $"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. ";
            ui.message.text += "Press Next to Continue. ";
            nextAction = Actions.chooseFamily;
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