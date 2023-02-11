using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NavajoWars
{
    
    // DOES NOT SEEM TO WORK AS AN OBJECT
    
    public class QuestionPreempt : GameStep, IReceive
    {
        public override string stepName { get => "QuestionPreempt"; }

        public override async void Begin()
        {
            /*//use Begin only if coming "down" the right way
            //initialUndo not necessary because previous button goes to prior scene
            //tInitialUndo tinitialUndo = gameObject.AddComponent<tInitialUndo>();
            //logic.setUndo(this, tinitialUndo);

            logic.setUndo(this, this);
            // if enemy ops or player ops have been done do not ask about preempting
            if (gs.isEnemyOpsDone && gs.isPlayerOpsDone) 
            { 
                logic.SceneComplete();
            }            
            else if (gs.isEnemyOpsDone) PlayerOperation();
            else if (!gs.isPlayerOpsDone && gs.AP >= gs.CurrentCard.Points[0])
            {
                ui.hideBackNext();
                ui.addHeadline($"\nPreempt for {gs.CurrentCard.Points[0]} AP?");
                bParams yes = new("Yes Preempt");
                bParams no = new("Do Not Preempt");
                (int index, string text) result = await IReceive.GetChoiceAsync(new List<bParams> { yes, no });
                ui.hidePrev();
                ui.showBackNext();
                if (result.text == yes.name)
                {
                    gs.AP -= gs.CurrentCard.Points[0];
                    ui.displayText($"Subtracted {gs.CurrentCard.Points[0]} AP\n");
                    PlayerOperation(); 
                }
                if (result.text == no.name)
                {
                    ui.displayText("Calling Enemy Ops");
                    //EnemyOperation();
                }
            }
            else
            {
                ui.displayText("Calling Enemy Ops");
                //EnemyOperation();
            }*/
        }

        async void PlayerOperation()
        {
            /*// no new undo point, just go back to initial question
            ui.displayHeadline("Select a Player Operation");
            ChooseFamily chooseFamily = gameObject.GetComponent<ChooseFamily>();
            // add gamestep objects to buttons to call directly chooseFamily?.Begin(chooseFamily); 
            bParams planning = new("Planning");
            bParams actions = new("Take Actions", chooseFamily);
            bParams passage = new("Passage of Time");
            GameStep result = await IReceive.GetChoiceAsyncObject(new List<bParams> { planning, actions, passage });
            result.Begin();*/
        }

        public override void Undo()
        {
            // use Undo if coming up from somewhere else

           
        }
    }
}
