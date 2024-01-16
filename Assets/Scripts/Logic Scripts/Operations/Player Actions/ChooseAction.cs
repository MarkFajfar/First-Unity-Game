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

        Family selectedFamily;

        void getReferences()
        {
            // assign in Inspector
            move = gameObject.GetComponentInChildren<Move>();
            plant = gameObject.GetComponentInChildren<Plant>();
            raid = gameObject.GetComponentInChildren<Raid>();
            findWater = gameObject.GetComponentInChildren<FindWater>();
            trade = gameObject.GetComponentInChildren<Trade>();
            tribalCouncil = gameObject.GetComponentInChildren<TribalCouncil>();
        }

        /**/

        public override async void Begin()
        {
            gm.SaveUndo(this);
            // if coming down from choose family, then reset list of completed actions
            if (gm.stepStack.Peek().stepName == "ChooseFamily")
            {
                gs.completedActions = 0;

                List<GameStep> steps = new() { move, plant, raid, findWater, trade, tribalCouncil };
                foreach (GameStep step in steps) step.isCompleted = false;
            }
            
            ui.Initialize();

            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            ui.displayText($"Choose an action for {selectedFamily.Name}");
            if (gs.completedActions > 0)
            {
                ui.addText(", or press Next to choose a new family.");
                ui.OnNextClick = chooseAnotherFamily; 
                // unsubscribe below if another button pressed
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
            if (gs.HasFort.Contains(selectedFamily.IsWhere) && gs.CP > 0 && gs.TradeGoodsMax > gs.TradeGoodsHeld) validActions.Add(bTrade);
            ui.MakeChoiceButtonsAsync(validActions);
            ButtonInfo result = await ui.GetChoiceAsyncParams(); // IReceive.GetChoiceAsyncParams();
            // TODO: could return ButtonInfo and call that nextAction, which would unsubNext and push to stack before calling the GameStep in the ButtonInfo
            // would need to create ButtonInfo with both GameStep and name of nextAction
            ui.unsubNext();
            gm.stepStack.Push(this);
            result.gameStep.Begin();
        }

        void chooseAnotherFamily()
        {
            ui.OnNextClick -= chooseAnotherFamily;
            // is it necessary to cancel task?
            gm.stepStack.Push(this);
            //ChooseFamily chooseFamily = GetComponentInChildren<ChooseFamily>();
            chooseFamily.Undo();
        }

        public override void Undo()
        {
            gm.LoadUndo(this);
            Begin();
        }
    }
}
