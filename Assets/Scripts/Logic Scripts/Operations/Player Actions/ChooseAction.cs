using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class ChooseAction : GameStep
    {
        public override string stepName { get => "ChooseAction"; }

        public Move move;
        public Plant plant;
        public FindWater findWater;
        public Raid raid;
        public Trade trade;
        public TribalCouncil tribalCouncil;

        public ChooseFamily chooseFamily;

        GameState.Family selectedFamily;

        void getReferences()
        {
            // assign in Inspector
            move = gameObject.GetComponent<Move>();
            plant = gameObject.GetComponent<Plant>();
            raid = gameObject.GetComponent<Raid>();
            findWater = gameObject.GetComponent<FindWater>();
            trade = gameObject.GetComponent<Trade>();
            tribalCouncil = gameObject.GetComponent<TribalCouncil>();
        }

        /*void WeirdActionFunction(List<bParams> choices) { }

        void WeirdEventFunction(object sender, bParamsEventArgs choices) { }*/

        public override async void Begin()
        {
            /*chooseFamily.tCreateButtons += WeirdActionFunction;
            chooseFamily.CreateButtonsEvent += WeirdEventFunction;*/
            
            //getReferences();

            // no save because only choosing from list of possible actions

            // if coming down from choose family, then reset list of completed actions
            if (gs.stepStack.Peek().stepName == "ChooseFamily") 
            { 
                gs.completedActions = 0;
                foreach (GameStep step in logic.steps)
                { if (OperationsLogic.PlayerActionStepNames.Contains(step.stepName)) step.isCompleted = false; }
                // TODO: where are game steps stored?
            }
            // make this a generic method??

            ui.Initialize();

            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            ui.displayText($"Choose an action for {selectedFamily.Name}");
            if (gs.completedActions > 0)
            {
                ui.addText(", or press Back to choose a new family.");
                // ui.OnNextClick += chooseAnotherFamily; 
                // subscribing here is a problem because if not clicking Next it does not unsubscribe
            }
            else
            {
                ui.addText(".");
                ui.unsubNext(); // clear next button, just in case
            }

            //var validActions = new Dictionary<string, GameStep>() { { "Move", move }, { "Plant or Harvest Corn", plant } };      
            ButtonInfo bMove = new("Move", move);
            ButtonInfo bPlant = new("Plant or Harvest Corn", plant);
            ButtonInfo bRaid = new("Raid", raid);
            ButtonInfo bFindWater = new("Find Water", findWater);
            ButtonInfo bTrade = new("Trade at Fort", trade);
            ButtonInfo bTribalCouncil = new("Tribal Council", tribalCouncil);
            List<ButtonInfo> validActions = new() { bMove, bPlant };
            if (gs.completedActions == 0) validActions.Add(bTribalCouncil);
            if (selectedFamily.HasMan && gs.MP > 0) validActions.Add(bRaid);
            if (gs.HasDrought.Contains(selectedFamily.IsWhere)) validActions.Add(bFindWater);
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.CP > 0 && (gs.TradeGoodsMax - gs.TradeGoodsHeld) > 0) validActions.Add(bTrade);
            ui.MakeChoiceButtonsAsync(validActions);
            ButtonInfo result = await IReceive.GetChoiceAsync();
            gs.stepStack.Push(this);
            result.gameStep.Begin();
        }

        void chooseAnotherFamily()
        {
            ui.OnNextClick -= chooseAnotherFamily;
            // is it necessary to cancel task?
            gs.stepStack.Push(this);
            //ChooseFamily chooseFamily = GetComponent<ChooseFamily>();
            chooseFamily.Undo();
        }

        public override void Undo()
        {
            Begin();
        }
    }
}