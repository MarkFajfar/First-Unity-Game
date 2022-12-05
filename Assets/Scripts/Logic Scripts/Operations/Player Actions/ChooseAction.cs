using JetBrains.Annotations;
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

        void getReferences()
        {
            move = gameObject.GetComponent<Move>();
            plant = gameObject.GetComponent<Plant>();
            raid = gameObject.GetComponent<Raid>();
            findWater = gameObject.GetComponent<FindWater>();
            trade = gameObject.GetComponent<Trade>();
            tribalCouncil = gameObject.GetComponent<TribalCouncil>();
        }

        public override async void Begin()
        {
            getReferences();

            // no save because only choosing from list of possible actions

            // if coming down from choose family, then reset list of completed actions
            GameStep caller = gs.stepStack.Peek();
            if (caller.stepName == "ChooseFamily") gs.completedActions = new();

            ui.Initialize();

            ui.displayText($"Choose an action for {gs.selectedFamily.Name}");
            if (gs.completedActions.Count > 0)
            {
                ui.addText(", or press Next to choose a new family.");
                ui.OnOpsNextClick += chooseAnotherFamily;
            }
            else
            {
                ui.addText(".");
                ui.unsubNext(); // clear next button, just in case
            }

            //var validActions = new Dictionary<string, GameStep>() { { "Move", move }, { "Plant or Harvest Corn", plant } };      
            bParams bMove = new("Move", move);
            bParams bPlant = new("Plant or Harvest Corn", plant);
            bParams bRaid = new("Raid", raid);
            bParams bFindWater = new("Find Water", findWater);
            bParams bTrade = new("Trade at Fort", trade);
            bParams bTribalCouncil = new("Tribal Council", tribalCouncil);
            List<bParams> validActions = new() { bMove, bPlant };
            if (gs.completedActions.Count == 0) validActions.Add(bTribalCouncil);
            if (gs.selectedFamily.HasMan && gs.MP > 0) validActions.Add(bRaid);
            if (gs.HasDrought.Contains(gs.selectedFamily.IsWhere)) validActions.Add(bFindWater);
            if (gs.HasFort.Contains(gs.selectedFamily.IsWhere) && gs.CP > 0 && (gs.TradeGoodsMax - gs.TradeGoodsHeld) > 0) validActions.Add(bTrade);
            ui.DisplayChoiceButtonsEvent(validActions);
            GameStep result = await IReceive.GetChoiceAsyncObject(validActions);
            gs.stepStack.Push(this);
            result.Begin();
        }

        void chooseAnotherFamily()
        {
            ui.OnOpsNextClick -= chooseAnotherFamily;
            // is it necessary to cancel task?
            gs.stepStack.Push(this);
            ChooseFamily chooseFamily = GetComponent<ChooseFamily>();
            chooseFamily.Undo();
        }

        public override void Undo()
        {
            Begin();
        }
    }
}
