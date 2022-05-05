using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class OperationsUIScript : MonoBehaviour, IsUIScript
    {
        GameManager gm;
        GameState gs;
        ChoiceUIScript choice;
        PlanningLogic planning;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
            planning = gameObject.GetComponent<PlanningLogic>();
        }

        void OnEnable()
        {
            choice.OnChoiceMade += choiceManager;
            getVisualElements();
        }

        void OnDisable()
        {
            choice.OnChoiceMade -= choiceManager;
        }

        public Label headline;
        public Label message;
        Button prev;
        Button quit;
        public Button back;
        public Button next;

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

        void choiceManager(string choiceText)
        {
            choiceText = "clicked" + choiceText.Replace(" ", "");
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(choiceText);
            //must be a public method - to be called from another script
            chosenMethod?.Invoke(this, null);
        }

        void Start()
        {
            // reset UI for reload
            headline.visible = true;
            message.visible = false;
            back.visible = false;
            next.visible = false;
            prev.visible = false;
            quit.visible = false;

            questionPreempt();
        }

        void questionPreempt()
        {
            if (gs.AP >= gs.CurrentCard.Points[0])
            {
                headline.text = $"Preempt for {gs.CurrentCard.Points[0]} AP?";
                choice.DisplayChoices(new List<string> { "Yes Preempt", "Do Not Preempt" });
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
        }

        public void clickedDoNotPreempt() => EnemyOperation();

        int planningStepNum;
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

        public void clickedTakeActions()
        { 
            headline.text = "Take Actions"; 
        }

        public void clickedPassageofTime()
        { 
            headline.text = "Passage of Time"; 
        }

        void EnemyOperation()
        {
            print("Enemy Ops");
        }

        void nextClicked()
        {
            if (isPlanning)
            {
                planningStepNum++;
                planning.doStep(planningStepNum);
            }
        }

        void backClicked()
        {
            if (isPlanning)
            {
                planningStepNum--;
                planning.doStep(planningStepNum);
            }
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
