using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public class OperationsLogic : LogicScript, IReceive
    {
        /*public GameObject ChoiceManager; // CHANGE to interface
        //protected ChoiceUIScript choice;
        protected ChoiceMade.ChoiceMadeEventHandler choiceEventHandler = null;
        protected ChoiceMadeObject.ChoiceMadeObjectEventHandler choiceMadeObjectEventHandler = null;
*/
        // Player Action GameSteps
        // public because assigned in inspector?
        protected InitialUndo initialUndo;
        public PlayerOperation playerOperation;
        public ChooseFamily chooseFamily;
        public ChooseAction chooseAction;
        public FindWater findWater;
        //public QuestionPreempt questionPreempt;

        void OnEnable()
        {
            //var chobj = GameObject.FindWithTag("ChoiceManager");
            //choice = ChoiceManager.GetComponent<ChoiceUIScript>();
            //choice = GetComponent<ChoiceUIScript>();

            // Get Player Action GameStep Components
            initialUndo = gameObject.AddComponent<InitialUndo>();
            chooseFamily = GetComponent<ChooseFamily>();
            chooseAction = GetComponent<ChooseAction>();
            findWater = GetComponent<FindWater>();
            playerOperation = GetComponent<PlayerOperation>();
            //NEED TO ADD ALL COMPONENTS HERE

        }

        public (GameStep caller, GameStep target) gotoUndo;
        //public List<GameStep> steps;

        void Start()
        {
            // called when scene "Operations" is loaded, called from gm
            // but what if called from loaded save game? then go to saved currentStep?
            if (gs.stepStack == null) { gs.stepStack = new(); }
            if (gs.stepStack.Count > 0) { gs.stepStack.Pop().Begin(); } 
            else
            {
                // coming from card draw, neither can be done:
                gs.isPlayerOpsDone = false;
                gs.isEnemyOpsDone = false;
                gs.isPreempt = false;
                gs.canBackToDraw = true;
                ConfirmScreen();
                //questionPreempt.Begin(questionPreempt);
            }
        }

        async void ConfirmScreen()
        {            
            List<bParams> choices = new();

            bParams redo = new("Go Back to Card Draw");
            if (gs.canBackToDraw) 
            {
                choices.Add(redo);
                gs.canBackToDraw = false; // do not show again unless coming from card draw or coming back and no actions completed
            }

            if (!gs.isEnemyOpsDone && !gs.isPlayerOpsDone && gs.AP >= gs.CurrentCard.Points[0])
            {
                ui.hideBackNext();
                ui.displayText($"\nPreempt for {gs.CurrentCard.Points[0]} AP?");
                bParams yes = new("Yes Preempt");
                choices.Add(yes);
                bParams no = new("Do Not Preempt");
                choices.Add(no);
                ui.DisplayChoiceButtonsEvent(choices);
                (int index, string text) result = await IReceive.GetChoiceAsync(choices);
                // after button clicked, activate back to come here if stack is empty
                // but set back to use stack for initial Undo (because may come back here with steps in stack)
                ui.showBackNext();
                if (result.text == yes.name)
                {
                    gs.isPreempt = true; // signals that points were subtracted
                    gs.AP -= gs.CurrentCard.Points[0];
                    ui.displayText($"Subtracted {gs.CurrentCard.Points[0]} AP\n");
                    gs.stepStack.Push(initialUndo);
                    playerOperation.Begin();
                }
                if (result.text == no.name)
                {
                    gs.isPreempt = false;
                    ui.displayText("Calling Enemy Ops");
                    gs.stepStack.Push(initialUndo);
                    EnemyOperation();
                }
                if (result.text == redo.name)
                {
                    gs.isPreempt = false;
                    gm.PrevScene();
                }
            }
            else
            {
                ui.showBackNext();
                gs.isPreempt = false;

                bParams draw = new("Next Card");
                if (gs.isEnemyOpsDone && gs.isPlayerOpsDone)
                {
                    ui.displayText("All Operations completed. Click 'Next Card' to draw a new card.");
                    choices.Add(draw);
                }

                bParams player = new("Player Operations");
                if (gs.isEnemyOpsDone && !gs.isPlayerOpsDone)
                {
                    ui.displayText("Enemy Operations completed. Click 'Player Operations' to continue.");
                    choices.Add(player);
                }

                bParams enemy = new("Enemy Operations");
                if ((gs.isPlayerOpsDone && !gs.isEnemyOpsDone) || gs.AP < gs.CurrentCard.Points[0])
                {
                    if (gs.AP < gs.CurrentCard.Points[0] && !gs.isPlayerOpsDone)
                    { ui.displayText("Insufficient AP to Preempt.\n"); }
                    else
                    { ui.displayText("Player Operations completed.\n"); }
                    ui.addText("Click 'Enemy Operations' to continue.");
                    choices.Add(enemy);
                } 
                ui.DisplayChoiceButtonsEvent(choices);
                (int index, string text) result = await IReceive.GetChoiceAsync(choices);

                if (result.text == redo.name)
                {
                    // no back function, just reenter card name
                    gm.PrevScene();
                }
                if (result.text == draw.name)
                {
                    // no back function, this screen confirms
                    SceneComplete();
                }
                if (result.text == player.name)
                {
                    gs.stepStack.Push(initialUndo);
                    playerOperation.Begin();
                }
                if (result.text == enemy.name)
                {
                    gs.stepStack.Push(initialUndo);
                    EnemyOperation();
                }
            }
        }

        // all further functions need to be game steps

        /*async void mQuestionPreempt()
        {
            // just in case here by mistake when scene is done or if enemy ops done 
            if (gs.isEnemyOpsDone && gs.isPlayerOpsDone) SceneComplete();
            if (gs.isEnemyOpsDone) PlayerOperation();
            // otherwise:
            // set gotoUndo to null, so back button can work
            gotoUndo.caller = null;
            gotoUndo.target = null;
            // set back button to here; previous button returns to card draw
            ui.OnOpsBackClick += mQuestionPreempt;
            // just in case here by mistake when scene is done or if enemy ops done 
            if (gs.isEnemyOpsDone && gs.isPlayerOpsDone) SceneComplete();
            else if (gs.isEnemyOpsDone) PlayerOperation();
            else if (gs.isPlayerOpsDone) EnemyOperation();
            else
            {
                // if coming back on undo and points were subtracted, add back
                if (gs.isPreempt) 
                {
                    ui.displayText($"Adding {gs.CurrentCard.Points[0]} AP previously subtracted.\n");
                    gs.AP += gs.CurrentCard.Points[0];
                    gs.isPreempt = false;
                } 
                if (gs.AP >= gs.CurrentCard.Points[0])
                {
                    // previous button only if waiting for preempt answer (otherwise back to here)
                    ui.hideBackNext();
                    ui.addHeadline($"\nPreempt for {gs.CurrentCard.Points[0]} AP?");
                    bParams yes = new("Yes Preempt");
                    bParams no = new("Do Not Preempt");
                    (int index, string text) result = await IReceive.GetChoiceAsync(new List<bParams> { yes, no });
                    // after button clicked, activate back to come here
                    ui.showBackNext();
                    if (result.text == yes.name)
                    {
                        gs.isPreempt = true; // signals that points were subtracted
                        gs.AP -= gs.CurrentCard.Points[0];
                        ui.displayText($"Subtracted {gs.CurrentCard.Points[0]} AP\n");
                        PlayerOperation();
                    }
                    if (result.text == no.name)
                    {
                        gs.isPreempt = false;
                        ui.displayText("Calling Enemy Ops");
                        EnemyOperation();
                    }
                }
                else
                {
                    ui.displayText("Insufficient AP to Preempt; starting Enemy Operations.");
                    EnemyOperation();
                }
            }
        }*/

        async void PlayerOperation() // GameStep instead
        {
            // back button is assigned before calling this method
            ui.showBackNext();
            ui.displayHeadline("Select a Player Operation");
            ui.displayText("");
            // add gamestep objects to buttons to call directly 
            bParams planning = new("Planning");
            bParams actions = new("Take Actions", chooseFamily);
            bParams passage = new("Passage of Time");
            List<bParams> choices = new() { planning, actions, passage };
            ui.DisplayChoiceButtonsEvent(choices);
            GameStep result = await IReceive.GetChoiceAsyncObject(choices);
            // after button clicked, change back to come here
            // but will be superseded if there is any step in the stack 
            ui.unsubBack();
            ui.OnOpsBackClick += PlayerOperation;
            result.Begin();            
        }

        void EnemyOperation()
        {
            ui.displayHeadline("Select an Enemy Operation");
            ui.displayText(""); 

            // after button clicked, change back to come here
            ui.unsubBack();
            ui.OnOpsBackClick += EnemyOperation;
            // add call to enemy action GameStep
        }

        void PlayerOpsDone(GameStep caller)
        {
            // called only from chooseFamily
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
            setUndo(caller, caller);
            ConfirmScreen();
        }

        public override void SceneComplete()
        {
            ui.unsubNext();
            ui.unsubBack();
            gs.stepStack.Clear();
            gm.LoadNewScene("CardDraw");
        }

        public override void instructFromStep(GameStep caller, string instruction) 
        {
            // used instead of initialUndo to come back after action completed
            if (instruction == "PlayerOpsDone") PlayerOpsDone(caller);
            if (instruction == "EnemyOpsDone") EnemyOpsDone(caller);
            //if (instruction == "ChooseAnotherAction") ChooseAnotherAction();
        }

        public override void setUndo(GameStep caller, GameStep target)
        {
            //no need for this method?
            ui.unsubBack();
            // caller is where the undo point was set, target is where to go on undo
            gotoUndo = (caller, target); 
            gm.tSaveGame();
        }

        public override void setUndo(GameStep target)
        {
            // only one argument sets new target, but leaves caller the same
            ui.unsubBack(); 
            GameStep caller = gotoUndo.caller;
            gotoUndo = (caller, target);
            gm.tSaveGame();
        }

        public override void callUndo()
        {
            // no need for this method? instead call undo function in the "popped" step?
            // CALLED WHEN BACK BUTTON IS PRESSED, if there is any step in the stack
            //gm.tLoadGame();
            gs.stepStack.Pop().Undo();
            //target.Undo(caller);
        }

        public override void initialUndoTarget() 
        {
            // if coming back from PlayerOperation or EnemyOperation
            if (gs.isPreempt) 
            {
                gs.AP += gs.CurrentCard.Points[0];
                gs.isPreempt = false;
            }
            if (gs.completedActions.Count == 0 && gs.completedFamilies.Count == 0) 
            { gs.canBackToDraw = true; }
            ConfirmScreen();
        }        

        // FOLLOWING ARE NOT NEEDED?

        public override void callUndo(GameStep caller, GameStep target)
        {
            throw new NotImplementedException();
        }

        public override void callUndo(GameStep caller)
        {
            throw new NotImplementedException();
        }
    }
}
