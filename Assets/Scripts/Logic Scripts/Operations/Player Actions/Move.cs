using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using CustomExtensions;

namespace NavajoWars
{
    public class Move : GameStep
    {
        public override string stepName { get => "Move"; }

        Family selectedFamily;
        //Territory priorLocation;
        bool horse;
        bool fortInGame;
        bool fortInTerritory;
        bool isFirstAction;
        int missing;

        public override void Begin()
        {
            gm.SaveUndo(this); 
            introText();
        }

        void introText() 
        {
            selectedFamily = gs.SelectedFamily;
            //priorLocation = selectedFamily.IsWhere;
            horse = selectedFamily.HasHorse;
            fortInGame = gs.ListTerrFort.Count > 0;
            fortInTerritory = selectedFamily.IsWhere.HasFort; // gs.HasFort.Contains(selectedFamily.IsWhere);
            isFirstAction = gs.completedActions == 0;
            missing = OperationsLogic.numMissing(selectedFamily);
            
            ui.ClearChoicePanel();
            ui.displayHeadline($"Move {selectedFamily.Name}");
            ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Move costs MPs equal to the destination Area's {(horse ? "parenthesized " : "")}value. If there is {(fortInGame ? $"a Fort in the destination Territory, or " : "")}an Outpost in an Area < = the destination Area, the MP cost is +1. ");
            string chellyMsg = isFirstAction ? "Move to or from Canyon de Chelly requires all MPs. " : "No move to or from Canyon de Chelly because not family's first action. ";
            string fortMsg = (fortInGame && selectedFamily.Ferocity > 1) ? "A family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. " : "";
            if (fortInTerritory) fortMsg += isFirstAction ? "If currently in the same Area as a Fort, move only on first action and cost is increased by die roll. " : "Move from same Area as a Fort only in first action. ";
            ui.addText(chellyMsg + fortMsg + "Press Next to continue.");
            ui.OnNextClick = resolveMove;
        }

        void resolveMove()
        {
            ui.OnNextClick -= resolveMove;
            // after first screen cleared, set back button to come back to this step
            gm.stepStack.Push(this);
            ui.displayText($"Is {selectedFamily.Name} moving to a new Territory?");
            ButtonInfo no = new("No Same Territory", noSame);
            ButtonInfo yesN = new("Yes New Territory", yesNew);
            ButtonInfo yesC = new("Yes to Canyon de Chelly", yesChelly);
            List<ButtonInfo> choices = new() { no, yesN };
            if (isFirstAction) choices.Add(yesC);
            ui.ShowChoiceButtons(choices);
        }

        void noSame() 
        {
            ui.displayText("Move completed. ");
            if (selectedFamily.IsWhere.HasCorn) 
                ui.addText("Remove corn counter if Family moving from that Area. ");
            if (selectedFamily.Ferocity > 1 && selectedFamily.IsWhere.HasFort) 
                ui.addText("Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. ");
            ui.addText("\nPress Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        void yesNew() 
        {
            ui.displayText($"Select {selectedFamily.Name}'s new Territory and press Next to continue. ");
            if (selectedFamily.IsWhere.HasCorn) ui.addText("Remove corn counter if Family moving from that Area.");
            ui.DisplayLocations();
            ui.OnNextClick = newTerritorySelected;
        }

        void newTerritorySelected()
        {
            selectedFamily.MoveTo(ui.ReturnLocation());
            ui.CloseLocations();
            ui.displayText($"{selectedFamily.Name} moved to {selectedFamily.IsWhere.Name}. ");
            if (selectedFamily.Ferocity > 1 && selectedFamily.IsWhere.HasFort) 
            ui.addText("Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. ");
            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        void yesChelly()
        {
            ui.displayText("Move to Canyon de Chelly completed. ");
            if (selectedFamily.IsWhere.HasCorn) ui.addText("Remove corn counter if Family moving from that Area.");
            ui.addText("Press Next to continue.");
            //var canyon = eTerritory.Canyon;
            selectedFamily.MoveTo(eTerritory.Canyon.ByTag());
            //(gs.Territories.Where(t => t.Tag == eTerritory.Canyon).First());
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<ChooseAnotherAction>().Begin();
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
            introText();
            // stuff to do on undo
        }
    }
}