using System;
using System.Collections.Generic;
using System.Linq;

namespace NavajoWars
{
    public class PlanningFour : GameStep
    {
        public override string stepName { get => "PlanningFour"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Four");
            int tgAvailable = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            if (gs.CP <= 0 || gs.AP <= 0 || tgAvailable <= 0)
            {
                ui.displayText("Not possible to use CP to buy Trade Goods. Press Next to continue.");
                ui.OnNextClick = actionComplete;
            }
            else
            {
                int toSpend = Math.Min(gs.Families.Where(f => f.HasWoman).Count(), tgAvailable);
                ui.displayText($"You may spend up to {toSpend} AP to purchase Trade Goods. How many?");

                List<ButtonInfo> choices = new();
                for (int i = 0; i < (toSpend + 1); i++)
                {
                    ButtonInfo choiceSpend = new(i.ToString())
                    {
                        data = i,
                        passBack = spend
                    };
                    choices.Add(choiceSpend);
                }
                ui.ShowChoiceButtons(choices);
            }
        }

        void spend(ButtonInfo result) 
        {
            int spent = (int)result.data; 
            gs.AP -= spent;
            gs.TradeGoodsHeld += spent;
            ui.displayText($"Spending {spent} AP leaves {gs.AP} AP.\n{gs.TradeGoodsHeld} Trade Goods Held. Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PlanningFive>().Begin();
        }

        public override void Undo()
        {
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            gm.LoadUndo(this);
            Begin();
        }
    }
}
