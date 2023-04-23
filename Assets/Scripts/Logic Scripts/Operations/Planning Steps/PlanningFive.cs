using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class PlanningFive : GameStep
    {
        public override string stepName { get => "PlanningFive"; }

        bool isWarInstr;
        bool isDipInstr;

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Planning\nStep Five");
            isWarInstr = true;
            isDipInstr = true;
            // check if any needed Instruction is on the Active Column, change bool
            if (gs.AP <= 0 || (gs.MP <= 0 && gs.TradeGoodsHeld <= 0) || (!isWarInstr && !isDipInstr))
            {
                ui.displayText("Not possible to conduct Tribal Warfare or Tribal Diplomacy. They Require at least 1 AP and either 1 MP or 1 Trade Good. Also the Ute or Comanche Instruction must be in the Active Column. Press Next to go to next step.");
                ui.OnNextClick += actionComplete;
            }
            else
            {
                askDipWar();
            }
        }

        async void askDipWar()
        {
            ui.hideBackNext();
            foreach (var family in gs.Families.Where(f => f.HasMan))
            {
                if (gs.AP > 0 && (gs.MP > 0 || gs.TradeGoodsHeld > 0))
                {
                    bool canWar = gs.MP > 0 && isWarInstr;
                    bool canDip = gs.TradeGoodsHeld > 0 && isDipInstr;
                    string askWar = canWar ? "spend 1 MP to conduct Tribal Warfare" : "";
                    string askDip = canDip ? "spend 1 Trade Good to conduct Tribal Diplomacy" : "";
                    string or = (canWar && canDip) ? ", or " : "";
                    ui.displayText($"For {family.Name}, " + askWar + or + askDip + "?");

                    ButtonInfo choiceWar = new("Tribal Warfare");
                    ButtonInfo choiceDip = new("Tribal Diplomacy");
                    ButtonInfo no = new("Neither");
                    List<ButtonInfo> choices = new() { no };
                    if (canWar) choices.Add(choiceWar);
                    if (canDip) choices.Add(choiceDip); 
                    ui.MakeChoiceButtonsAsync(choices);
                    ButtonInfo result = await IReceive.GetChoiceAsyncParams();                    
                    // note - efficient to use async when next step is very simple; also, this is a loop
                    if (result == choiceWar)
                    { gs.AP--; gs.MP--; }
                    if (result == choiceDip)
                    { gs.AP--; gs.TradeGoodsHeld--; }
                    ui.displayText("");
                }
                else
                {
                    ui.displayText($"Insufficient resources available for {family.Name}.\n");
                    break;
                }
            }
            ui.addText($"There are now {gs.AP} AP, {gs.MP} MP, and {gs.TradeGoodsHeld} Trade Goods. Press Next to Continue.");
            ui.showBackNext();
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PlanningSix>().Begin();
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
