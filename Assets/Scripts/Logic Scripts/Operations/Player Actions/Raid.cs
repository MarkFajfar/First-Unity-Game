using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Raid : GameStep
    {
        public override string stepName { get => "Raid"; }

        Family selectedFamily;
        bool fortInTerritory;
        bool horse;

        public override void Begin()
        {
            gm.SaveUndo(this);
            introText();
        }

        void introText()
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            fortInTerritory = selectedFamily.IsWhere.HasFort;
            horse = selectedFamily.HasHorse;
            ui.displayHeadline($"{selectedFamily.Name} Raids\nSanta Fe or Outpost");
            if (fortInTerritory) ui.addHeadline(" or Fort");
            ui.displayText($"Raid costs MPs equal to the {(horse ? "parenthesized " : "")}value of all Areas between {selectedFamily.Name} and Santa Fe (not counting SF), or if {selectedFamily.Name} has not spent any MP, it may spend all MP to long-range Raid SF from any Area. Or, {selectedFamily.Name} may Raid an Outpost {(fortInTerritory ? "or Fort " : "")} if in the same Area or an Area with a lower Value on the same track as the target.{(fortInTerritory ? " Not possible if currently in a Fort." : "")}\n Raid Santa Fe or Outpost{(fortInTerritory ? " or Fort?" : "?")}");
            ButtonInfo sf = new("Santa Fe", raidSF);
            ButtonInfo outpost = new("Outpost", raidOutpost);
            ButtonInfo fort = new("Fort", raidFort);
            List<ButtonInfo> choices = new() { sf, outpost };
            if (fortInTerritory) choices.Add(fort);
            ui.ShowChoiceButtons(choices);
        }

        void raidSF()
        {
            ui.displayText("Draw a cube from the Raid Pool!");
            RaidTable.resolveDraw();
        }

        void raidOutpost()
        {
            ui.displayText($"Draw cubes one at a time from the Raid Pool until the Outpost takes sufficient hits to destroy it, or {selectedFamily.Name} is forced to fight a battle, or result is 'Raid Ends'.");
            RaidTable.resolveDraw();
        }


        void raidFort()
        {
            ui.displayText($"Draw cubes one at a time from the Raid Pool until the Fort takes sufficient hits to destroy it, or {selectedFamily.Name} is forced to fight a battle, or result is 'Raid Ends'.");
            RaidTable.resolveDraw();
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponentInChildren<ChooseAnotherAction>();
            chooseAnotherAction.Begin();
        }

        public override void Undo()
        {
            
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            introText();
            // stuff to do on undo
        }
    }
}
