using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class OperationsUIScript : MonoBehaviour, IsUIScript, IReceive
    {
        GameManager gm;
        GameState gs;
        ChoiceUIScript choice;
        ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
        //PlanningLogic planning;
        //PassageLogic passage;
        //PlayerActionLogic player;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
            //planning = gameObject.GetComponent<PlanningLogic>();
            //passage = gameObject.GetComponent<PassageLogic>();
            //player = gameObject.GetComponent<PlayerActionLogic>();
        }

        public Label headline;
        public Label message;
        Button prev;
        Button quit;
        public Button back;
        public Button next;

        void OnEnable()
        {
            getVisualElements();
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");

            message = root.Q<Label>("Message");
            
            back = root.Q<Button>("Back");
            back.clicked += backClicked;
            
            next = root.Q<Button>("Next");
            next.clicked += nextClicked; 
            
            prev = root.Q<Button>("Prev");
            prev.clicked += prevClicked;
            
            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;
        }

        void Start()
        {
            // reset UI for reload
            headline.visible = true;
            message.visible = false;
            back.visible = false;
            next.visible = false; 
            prev.visible = false;
            quit.visible = true;

            questionPreempt();
        }

        void questionPreempt()
        {
            if (gs.AP >= gs.CurrentCard.Points[0])
            {
                headline.text = $"Preempt for {gs.CurrentCard.Points[0]} AP?";
                choice.DisplayChoices(this, new List<string> { "Yes Preempt", "Do Not Preempt" });
                // with "this" could use "Yes" or "No" if not used elsewhere in this script
            }
            else
            {
                headline.text = "Insufficient AP to Preempt. Starting Enemy Action Phase.";
                EnemyOperation(); 
            }
        }

        public void clickedYesPreempt()
        {
            gs.AP -= gs.CurrentCard.Points[0];
            headline.text = $"Subtracted {gs.CurrentCard.Points[0]} AP.\nSelect One Operation";
            choice.DisplayChoices(new List<string> { "Planning", "Take Actions", "Passage of Time" });
            // cannot use "this" because response back to different scripts
        }

        int StepNum;
        public void Initialize()
        {
            quit.visible = false;
            prev.visible = false;
            back.visible = true;
            next.visible = true;
            message.visible = true;
            StepNum = 1;
        }

        public delegate void ChangeStep(int stepNum);
        public event ChangeStep OnChangeStep;

        public void clickedDoNotPreempt() => EnemyOperation();

/*       int planningStepNum;
        bool isPlanning;

        public void InitializePlanning() 
        {
            quit.visible = false;
            prev.visible = false;
            back.visible = true;
            next.visible = true;
            message.visible = true;
            planningStepNum = 1;
            isPlanning = true;
        }

        int passageStepNum;
        bool isPassage;
        public void InitializePassage()
        {
            quit.visible = false;
            prev.visible = false;
            back.visible = true;
            next.visible = true;
            message.visible = true;
            passageStepNum = 1;
            isPassage = true;
        }

        int actionStepNum;
        bool isAction;
        public void InitializeAction()
        {
            quit.visible = false;
            prev.visible = false;
            back.visible = true;
            next.visible = true;
            message.visible = true;
            actionStepNum = 1;
            isAction = true;
            headline.text = "Player Actions";
        }*/
        public void hideBackNext()
        {
            back.visible = false;
            next.visible = false;
        }

        public void showBackNext()
        {
            back.visible = true;
            next.visible = true;
        }

        void EnemyOperation()
        {
            print("Enemy Ops");
        }

        public void nextClicked()
        {
            StepNum++;
            OnChangeStep?.Invoke(StepNum);

            /*if (isPlanning)
            {
                planningStepNum++;
                planning.doStep(planningStepNum);
            }
            if (isPassage)
            {
                passageStepNum++;
                passage.doStep(passageStepNum);
            }
            if (isAction)
            {
                actionStepNum++;
                player.doStep(actionStepNum);
            }*/
        }

        public void backClicked()
        {
            StepNum--;
            OnChangeStep?.Invoke(StepNum);

            /*if (isPlanning)
            {
                planningStepNum--;
                planning.doStep(planningStepNum);
            }
            if (isPassage)
            {
                passageStepNum--;
                passage.doStep(passageStepNum);
            }
            if (isAction)
            {
                actionStepNum--;
                player.doStep(actionStepNum);
            }*/
        }

        void prevClicked() 
        {
            gm.PrevScene();
        }

        void quitClicked()
        {
            gm.ExitGame();
        }
    }
}
