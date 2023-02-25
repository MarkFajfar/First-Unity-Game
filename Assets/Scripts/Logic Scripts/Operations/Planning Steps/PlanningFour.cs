using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningFour : GameStep
    {
        public override string stepName { get => "PlanningFour"; }

        public async override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Four");
            int tgAvailable = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            if (gs.CP <= 0 || gs.AP <= 0 || tgAvailable <= 0)
            {
                ui.displayText("Not possible to use CP to buy Trade Goods. Press Next to continue.");
                ui.OnNextClick += actionComplete;
            }
            else
            {
                int toSpend = Math.Min(gs.Families.Where(f => f.HasWoman).Count(), tgAvailable);
                ui.hideBackNext();
                ui.displayText($"You may spend up to {toSpend} AP to purchase Trade Goods. How many?");

                List<ButtonInfo> choices = new();
                for (int i = 0; i < (toSpend + 1); i++)
                {
                    ButtonInfo choiceSpend = new(i.ToString(), i);
                    choices.Add(choiceSpend);
                }
                ui.MakeChoiceButtonsAsync(choices);
                ButtonInfo result = await IReceive.GetChoiceAsync();

                gs.AP -= result.tabIndex;
                gs.TradeGoodsHeld += result.tabIndex;
                ui.displayText($"Spending {result.tabIndex} AP leaves {gs.AP} AP.\n{gs.TradeGoodsHeld} Trade Goods Held. Press Next to continue.");
                ui.showBackNext();
                ui.OnNextClick += actionComplete;
            }
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            //GameStep planningFive = GetComponent<PlanningFive>();
            //planningFive.Begin();
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
            Begin();
            // stuff to do on undo
        }
    }
}
