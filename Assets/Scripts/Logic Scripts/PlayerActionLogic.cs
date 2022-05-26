using System;
using System.Linq;
using System.Collections.Generic;

namespace NavajoWars
{
    public class PlayerActionLogic : OperationsLogic
    {
        
        void Start()
        {
            Steps = new();
            Steps.Add(clickedTakeActions);
            Steps.Add(StepOne);
            Steps.Add(StepTwo);
            stepDone = new() { false, false, false, false, false, false };
        }

        public void clickedTakeActions()
        {
            ui.Initialize();
            ui.OnChangeStep += doStep;
            stepDone[0] = true;
            int numFamilies = gs.Families.Count();
            int numElders = gs.ElderDisplay.Sum();
            int numFamInCan = gs.Families.Where(f => f.IsWhere == Territory.Canyon).Count();
            numFamEligible = 1 + Math.Min(numFamilies, numElders + Math.Max(numFamInCan, 1));
            StepOne();
        }

        List<string> selectedF = new(); 
        GameState.Family selectedFamily;
        int numFamEligible;
        async void StepOne()
        {
            numFamEligible--;
            if (numFamEligible > 0)
            {
                ui.hideBackNext();
                ui.message.text = $"{numFamEligible} {(numFamEligible > 1 ? "families" : "family")} can be activated.\nEach family has 6 MP";
                bool isFamMissing = gs.Families.Where(f => !selectedF.Contains(f.Name) && (!f.HasMan || !f.HasWoman || !f.HasChild)).Any();
                if (isFamMissing)
                {
                    ui.message.text += ", except:";
                    foreach (var family in gs.Families.Where(f => !selectedF.Contains(f.Name)))
                    {
                        int missing = 0;
                        if (!family.HasMan) missing++;
                        if (!family.HasWoman) missing++;
                        if (!family.HasChild) missing++;
                        if (missing > 0)
                        { ui.message.text += $"\n{family.Name} has {6 - missing} MP"; }
                    }
                }
                ui.message.text += ".\n Choose a family to activate.";
                List<string> listFamilyNames = gs.Families.Where(f => !selectedF.Contains(f.Name)).Select(f => f.Name).ToList();
                (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(listFamilyNames);
                string choiceText = result.choiceText.Replace("clicked", "");
                selectedF.Add(choiceText);
                selectedFamily = gs.Families.First(f => f.Name == choiceText);
                StepTwo();
            }
            stepDone[0] = true;
            ui.message.text = "No more families may be activated. Click Next to continue.";
            ui.showBackNext();
            //ui.next.clicked -= ui.nextClicked;
            //ui.next.clicked += go to card draw? or next click to last step?
        }

        void StepTwo()
        {
            // repeat number of MP available?
            // list actions available
            ui.hideBackNext();
            ui.message.text = $"Choose an action for {selectedFamily.Name}.";
            List<string> validActions = new() { "Move", "Plant or Harvest Corn", "Tribal Council" };
            if (selectedFamily.HasMan && gs.MP > 0) validActions.Add("Raid");
            if (gs.HasDrought.Contains(selectedFamily.IsWhere)) validActions.Add("Find Water");
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.TradeGoodsHeld > 0) validActions.Add("Trade at Fort");
            choice.DisplayChoices(this, validActions);
            //(int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(validActions);
            //string choiceText = result.choiceText.Replace("clicked", "");
        }

        async void clickedMove()
        {
            int missing = 0;
            bool horse = selectedFamily.HasHorse;
            bool fort = gs.HasFort.Contains(selectedFamily.IsWhere);
            // test for moving from fort 
            if (!selectedFamily.HasMan) missing++;
            if (!selectedFamily.HasWoman) missing++;
            if (!selectedFamily.HasChild) missing++;
            ui.message.text = $"{selectedFamily.Name} has {6 - missing} MP. Move costs MPs equal to the destination Area's {(horse ? "parenthesized" : "")} value. If there is a Fort in the destination Territory, or an Outpost in an Area < = the destination Area, the MP cost is +1. ";
            if (fort) ui.message.text += "If currently in the same Area as a Fort, move cost increased by die roll. ";
            ui.message.text += "Is the family moving to a new Territory?";
            List<string> moveChoices = new() { "No Same Territory", "Yes New Territory", "Yes Canyon de Chelly" };
            (int choiceIndex, string choiceText) result = await IReceive.GetChoiceAsync(moveChoices);
            string choiceText = result.choiceText.Replace("clicked", "");
            if (choiceText == "NoSameTerritory")
            {
                ui.message.text = "Move completed.";
                if (selectedFamily.Ferocity > 1 && fort) ui.message.text += "Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband.";
                ui.showBackNext();
            }

        }

        void clickedPlantorHarvestCorn()
        {

        }

        void clickedTribalCouncil()
        {

        }

        void clickedRaid()
        {

        }

        void clickedFindWater()
        {

        }

        void clickedTradeatFort()
        {

        }
    }
}