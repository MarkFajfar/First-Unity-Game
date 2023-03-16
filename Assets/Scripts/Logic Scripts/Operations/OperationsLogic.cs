using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class OperationsLogic : LogicScript, IReceive
    {
        public GameObject UIObject;
        public OperationsUIScript ui;
        public PlayerOperation playerOperation;

        protected InitialUndo initialUndo;
        //public GameObject PlayerActionSteps;
        //public GameObject PlanningSteps;
        
        public static string[] PlayerActionStepNames = {"FindWater", "Move", "Plant", "Raid", "Trade", "TribalCouncil"};

        void OnEnable()
        {
            initialUndo = gameObject.AddComponent<InitialUndo>();
        }

        void Start()
        {
            // called when scene "Operations" is loaded, called from gm
            // but what if called from loaded save game? then go to saved currentStep?
            if (gm.stepStack == null) { gm.stepStack = new(); }
            if (gm.stepStack.Count > 0) { gm.stepStack.Pop().Begin(); } 
            else
            {
                // coming from card draw, neither can have been done:
                gs.isPlayerOpsDone = false;
                gs.isEnemyOpsDone = false;
                gs.isPreempt = false;
                gs.canBackToDraw = true;
                ConfirmScreen();
            }
        }

        async void ConfirmScreen()
        {
            ui.OnBackClick -= ConfirmScreen;

            List<ButtonInfo> choices = new();

            ButtonInfo redo = new("Go Back to Card Draw", backtoCardDraw);
            if (gs.canBackToDraw) 
            {
                choices.Add(redo);
                gs.canBackToDraw = false; // do not show again unless coming from card draw or coming back and no actions completed
            }

            if (!gs.isEnemyOpsDone && !gs.isPlayerOpsDone && gs.AP >= gs.CurrentCard.Points[0])
            {
                ui.hideBackNext();
                ui.displayText($"\nPreempt for {gs.CurrentCard.Points[0]} AP?");
                ButtonInfo yes = new("Yes Preempt", yesPreempt); 
                choices.Add(yes);
                ButtonInfo no = new("Do Not Preempt", noEnemyOps);
                choices.Add(no);
                ui.MakeChoiceButtons(choices);
                //ui.MakeChoiceButtonsAsync(choices);
                //ButtonInfo result = await IReceive.GetChoiceAsync();
                // after button clicked, activate back to come here if stack is empty
                // but set back to use stack for initial Undo (because may come back here with steps in stack)
            }
            else
            {
                ui.OnBackClick += ConfirmScreen;
                ui.showBackNext();
                gs.isPreempt = false;

                ButtonInfo draw = new("Next Card");
                if (gs.isEnemyOpsDone && gs.isPlayerOpsDone)
                {
                    ui.displayText("All Operations completed. Click 'Next Card' to draw a new card.");
                    choices.Add(draw);
                }

                ButtonInfo player = new("Player Operations");
                if (gs.isEnemyOpsDone && !gs.isPlayerOpsDone)
                {
                    ui.displayText("Enemy Operations completed. Click 'Player Operations' to continue.");
                    choices.Add(player);
                }

                ButtonInfo enemy = new("Enemy Operations");
                if ((gs.isPlayerOpsDone && !gs.isEnemyOpsDone) || gs.AP < gs.CurrentCard.Points[0])
                {
                    if (gs.AP < gs.CurrentCard.Points[0] && !gs.isPlayerOpsDone)
                    { ui.displayText("Insufficient AP to Preempt.\n"); }
                    else
                    { ui.displayText("Player Operations completed.\n"); }
                    ui.addText("Click 'Enemy Operations' to continue.");
                    choices.Add(enemy);
                } 
                ui.MakeChoiceButtonsAsync(choices);
                ButtonInfo result = await IReceive.GetChoiceAsync();
                // use async where each choice is very short
                if (result.name == redo.name)
                {
                    // no back function, just reenter card name
                    gm.PrevScene();
                }
                if (result.name == draw.name)
                {
                    // no back function, this screen confirms
                    SceneComplete();
                }
                if (result.name == player.name)
                {
                    gm.stepStack.Push(initialUndo);
                    playerOperation.Begin();
                }
                if (result.name == enemy.name)
                {
                    gm.stepStack.Push(initialUndo);
                    EnemyOperation();
                }
            }
        }

        void yesPreempt()
        {
            ui.OnBackClick += ConfirmScreen;
            ui.showBackNext();

            gs.AP -= gs.CurrentCard.Points[0];
            ui.displayText($"Subtracted {gs.CurrentCard.Points[0]} AP\n");
            gs.isPreempt = true; // signals that points were subtracted
            gm.stepStack.Push(initialUndo);
            playerOperation.Begin();
        }

        void noEnemyOps()
        {
            // no backclick added because handled by back to initialUndo
            gs.isPreempt = false;
            ui.displayText("Calling Enemy Ops");
            gm.stepStack.Push(initialUndo);
            EnemyOperation();
        }

        void backtoCardDraw()
        {
            gs.isPreempt = false;
            gm.PrevScene();
        }

        void EnemyOperation()
        {
            ui.displayHeadline("Select an Enemy Operation");
            ui.displayText(""); 

            // after button clicked, change back to come here
            ui.unsubBack();
            ui.OnBackClick += EnemyOperation;
            // add call to enemy action GameStep
        }

        void PlayerOpsDone(GameStep caller)
        {
            gs.canBackToDraw = false;
            // caller is at top of stepStack, leave any sub to next or back as fallback?
            gs.isPlayerOpsDone = true;
            ConfirmScreen();
        }

        void EnemyOpsDone(GameStep caller)
        {
            gs.canBackToDraw = false;
            ui.unsubNext();
            ui.unsubBack();
            gm.SaveUndo(caller);
            ConfirmScreen();
        }

        public override void SceneComplete()
        {
            ui.unsubNext();
            ui.unsubBack();
            gm.stepStack.Clear();
            gm.LoadNewScene("CardDraw");
        }

        public override void instructFromStep(GameStep caller, string instruction) 
        {
            // used instead of initialUndo to come back "down" after action completed
            if (instruction == "PlayerOpsDone") PlayerOpsDone(caller);
            if (instruction == "EnemyOpsDone") EnemyOpsDone(caller);
            //if (instruction == "ChooseAnotherAction") ChooseAnotherAction();
        }

        public override void initialUndoTarget() 
        {
            // if coming back from PlayerOperation or EnemyOperation
            if (gs.isPreempt) 
            {
                gs.AP += gs.CurrentCard.Points[0];
                gs.isPreempt = false;
            }
            if (gs.completedActions == 0 && gs.completedFamilies == 0) 
            { gs.canBackToDraw = true; }
            ConfirmScreen();
        }

        public static int numMissing(GameState.Family family)
        {
            int missing = 0;
            if (!family.HasMan) missing++;
            if (!family.HasWoman) missing++;
            if (!family.HasChild) missing++;
            return missing;
        }
    }
}
