using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class Move : GameStep
    {
        public override string stepName { get => "Move"; }

        GameState.Family selectedFamily;
        //Territory priorLocation;
        bool horse;
        bool fortInGame;
        bool fortInTerritory;
        bool isFirstAction;
        int missing;
        bool newTerritorySelected;

        public override void Begin()
        {
            gm.SaveUndo(this); 
            introText();
        }

        void introText() 
        {
            selectedFamily = gs.Families.Where(f => f.isSelectedOps).First();
            //priorLocation = selectedFamily.IsWhere;
            horse = selectedFamily.HasHorse;
            fortInGame = gs.HasFort.Count > 0;
            fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
            isFirstAction = gs.completedActions == 0;
            missing = OperationsLogic.numMissing(selectedFamily);
            newTerritorySelected = false; 
            
            ui.displayHeadline($"Move {selectedFamily.Name}");
            ui.displayText($"{selectedFamily.Name} has {6 - missing} MP{(isFirstAction ? "" : ", minus points already spent")}. Move costs MPs equal to the destination Area's {(horse ? "parenthesized " : "")}value. If there is {(fortInGame ? $"a Fort in the destination Territory, or " : "")}an Outpost in an Area < = the destination Area, the MP cost is +1. ");
            string chellyMsg = isFirstAction ? "Move to or from Canyon de Chelly requires all MPs. " : "No move to or from Canyon because not family's first action. ";
            string fortMsg = (fortInGame && selectedFamily.Ferocity > 1) ? "A family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband. " : "";
            if (fortInTerritory) fortMsg += isFirstAction ? "If currently in the same Area as a Fort, move only on first action and cost is increased by die roll. " : "Move from same Area as a Fort only in first action. ";
            ui.addText(chellyMsg + fortMsg + "Press Next to continue.");
            ui.OnNextClick += resolveMove;
        }

        void resolveMove()
        {
            ui.OnNextClick -= resolveMove;
            // after first screen cleared, set back button to come back to this step
            gs.stepStack.Push(this);
            ui.displayText($"Is {selectedFamily.Name} moving to a new Territory?");
            bParams no = new("No Same Territory", noSame);
            bParams yesN = new("Yes New Territory", yesNew);
            bParams yesC = new("Yes to Canyon de Chelly", yesChelly);
            List<bParams> choices = new() { no, yesN };
            if (isFirstAction) choices.Add(yesC);
            ui.MakeChoiceButtons(choices);
        }

        void noSame() 
        {
            ui.displayText("Move completed. Press Next to continue. ");
            bool fortInTerritory = gs.HasFort.Contains(selectedFamily.IsWhere);
            if (selectedFamily.Ferocity > 1 && fortInTerritory) ui.addText("Reminder: a Family with Ferocity > 1 that ends it activation in the same Area as a Fort must disband.\nPress Next to continue.");
            ui.OnNextClick += actionComplete;
        }

        void yesNew() 
        {
            ui.displayText($"Select {selectedFamily.Name}'s new Territory and click Next to continue.");
            ui.DisplayLocations();
            newTerritorySelected = true;
            ui.OnNextClick += actionComplete;
        }

        void yesChelly()
        {
            ui.displayText("Move to Canyon de Chelly completed. Press Next to continue.");
            selectedFamily.IsWhere = Territory.Canyon;
            ui.OnNextClick += actionComplete;
        }

        protected override void actionComplete()
        {
            if (newTerritorySelected) 
            {
                newTerritorySelected = false;
                selectedFamily.IsWhere = ui.ReturnLocation();
                ui.CloseLocations();
            }
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponent<ChooseAnotherAction>();
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
            introText();
            // stuff to do on undo
        }
    }
}