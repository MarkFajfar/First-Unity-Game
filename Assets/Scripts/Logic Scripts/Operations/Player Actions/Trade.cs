using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Trade : GameStep
    {
        public override string stepName { get => "Trade"; }

        Family selectedFamily;

        public override void Begin()
        {
            gm.SaveUndo(this);
            askTrade();
        }

        void askTrade()
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            int tradeGoodsAvail = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            ui.displayHeadline($"{selectedFamily.Name} Trades at Fort");
            ui.displayText($"This action costs all remaining MP and 1 CP, will reduce {selectedFamily.Name}'s ferocity to zero, and {selectedFamily.Name} must be in the Fort. {(tradeGoodsAvail == 1 ? "One trade good is" : $"{tradeGoodsAvail} trade goods are")} available. Do you wish to trade?");
            ButtonInfo yes = new("Yes, Trade", yesTrade);
            ButtonInfo no = new("Do Not Trade", noTrade);
            List<ButtonInfo> choices = new() { yes, no };
            // use async because logic to apply after choice made
            ui.ShowChoiceButtons(choices);        
            //HAS TO BE LAST ACTION
        }

        void yesTrade() 
        {
            gs.TradeGoodsHeld = gs.TradeGoodsMax;
            selectedFamily.Ferocity = 0;
            gs.CP--;
            ui.addText($"You have {gs.CP} CP and {gs.TradeGoodsHeld} trade goods. Press Next to Continue. ");
            ui.OnNextClick += actionComplete;
        }

        void noTrade()
        {
            // go back, same as undo ?
            gm.stepStack.Pop().Undo();
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponentInChildren<ChooseAnotherAction>();
            chooseAnotherAction.Begin();
        }

        public override void Undo()
        {
            // reset complete marker??
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            gm.LoadUndo(this);
            askTrade();
        }
    }
}
