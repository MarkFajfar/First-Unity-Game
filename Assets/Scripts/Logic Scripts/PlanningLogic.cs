using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class PlanningLogic : MonoBehaviour
    {
        GameManager gm;
        GameState gs;
        OperationsUIScript ui;
        ChoiceUIScript choice;
        List<Action> planningSteps = new List<Action>();
        List<bool> stepDone; 

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            ui = gameObject.GetComponent<OperationsUIScript>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
        }

        void Start()
        {
            planningSteps.Add(clickedPlanning);
            planningSteps.Add(StepOne);
            planningSteps.Add(StepTwo);
            planningSteps.Add(StepThree);
            planningSteps.Add(StepFour);
            planningSteps.Add(StepFive);
            planningSteps.Add(StepSix);
            stepDone = new () { false, false, false, false, false, false };
        }

        void OnEnable()
        {
            choice.OnChoiceMade += choiceManager;
            // dice.OnDieRolled += diceManager;
        }

        void OnDisable()
        {
            choice.OnChoiceMade -= choiceManager;
            // dice.OnDieRolled -= diceManager;
        }

        void choiceManager(string choiceText)
        {
            choiceText = "clicked" + choiceText.Replace(" ", "");
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(choiceText);
            chosenMethod?.Invoke(this, null);
        }

        bool isDieRolled = false;
        bool isDieRollSucceeded = false;
        void diceManager(bool isDieRollSuccess)
        {

            isDieRolled = true;
        }

        public void clickedPlanning()
        {
            ui.InitializePlanning();
            stepDone[0] = true;
            StepOne();
        }

        public void doStep(int planningStepNum) => planningSteps[planningStepNum]();

        void StepOne()
        {
            ui.headline.text = "Planning\nStep One";
            ui.message.text = $"Place a {gs.CurrentCard.ThisCardPerson} in the Passage of Time Box";
            if (!stepDone[1])
            {
                gs.PersonsInPassage.Add(gs.CurrentCard.ThisCardPerson);
                stepDone[1] = true; 
            }
        }

        void StepTwo()
        {
            ui.headline.text = "Planning\nStep Two";
            ui.message.text = $"Collect {gs.ElderDisplay.Sum()} AP for Elders, so now there are {gs.AP + gs.ElderDisplay.Sum()} AP. Advance each Elder one box to the right. Elders in the right-most box stay put.";
            if (!stepDone[2])
            {
                gs.AP += gs.ElderDisplay.Sum();
                for (int i = 6; i > 0; i--)
                {
                    gs.ElderDisplay[i] += gs.ElderDisplay[i - 1];
                    gs.ElderDisplay[i - 1] = 0;
                }
                stepDone[2] = true;
            }
        }

        void StepThree()
        {
            ui.headline.text = "Planning\nStep Three";
            // at this point there are no elders in slot 0 (moved right in prev step)
            if (gs.ElderDisplay.Sum() > 0 && !stepDone[3])
            {
                StartCoroutine(elderActionCoroutine());
            }
            else
            {
                ui.message.text = "No Elder Actions available.";
                stepDone[3] = true;
            }
        }

        bool isElderActionComplete = false;
        //bool ElderActionCompleted() => EAC ? true : false;

        // simple coroutine - has to be "complete" all the way to call to button or next action
        // in other words - call to coroutine has to be at the end of the calling method
        IEnumerator elderActionCoroutine()
        {
            ui.hideBackNext();
            ui.message.text = " ";
            for (int i = 1; i < gs.ElderDisplay.Length; i++)
            {
                if (gs.ElderDisplay[i] > 0)
                {
                    int j = 0;
                    while (j < gs.ElderDisplay[i])
                    {
                        int[] targetRoll = { 0, 1, 2, 2, 3, 4, 5 };
                        ui.headline.text = $"{gs.ElderDisplay[i] - j} Elder Action(s) in\nSlot {i}. Roll Die.";
                        ui.message.text = $"You need a {targetRoll[i]} or less.";
                        isDieRolled = false;
                        isDieRollSucceeded = false;
                        choice.DisplayChoices(new List<string> { "Roll Succeeded", "Roll Failed" });
                        yield return new WaitUntil(() => isDieRolled);
                        if (isDieRollSucceeded)
                        {
                            ui.headline.text = $"{gs.ElderDisplay[i] - j} Elder Action(s) in\nSlot {i}.  Choose Action."; 
                            isElderActionComplete = false;
                            List<string> elderChoices = new() { "Add 1 AP", "Change Family Ferocity" };
                            if (gs.CP > gs.MP) elderChoices.Add("Add 1 MP for 1 CP");
                            if (gs.MP > gs.CP) elderChoices.Add("Add 1 CP for 1 MP");
                            choice.DisplayChoices(elderChoices);
                            yield return new WaitUntil(() => isElderActionComplete);
                        }
                        j++;
                    }
                }
            }
            ui.headline.text = "Elder Actions Completed";
            stepDone[3] = true;
            ui.showBackNext();
        }

        IEnumerator dieRollCoroutine(Action<bool> onFinish)
        {
            bool isDieRollSuccess = false;
            // call dice.Display(success test) which sets isDieRolled to true and returns isSuccess
            // dice.Display() has OnDieRolled?.Invoke(isDieRollSuccess);
            // OnDieRolled listener sets isDieRolled to true
            yield return new WaitUntil(() => isDieRolled);
            onFinish(isDieRollSuccess);
        }

        public void clickedRollSucceeded()
        {
            isDieRollSucceeded = true;
            isDieRolled = true;
        }

        public void clickedRollFailed()
        {
            isDieRollSucceeded = false;
            isDieRolled = true;
        }

        public void clickedAdd1AP()
        {
            ui.message.text = "Adding 1 AP";
            gs.AP++;
            isElderActionComplete = true;
        }

        public void clickedChangeFamilyFerocity()
        {
            ui.message.text = "Another Coroutine to change ferocity??";
            isElderActionComplete = true;
        }

        public void clickedAdd1MPfor1CP()
        {
            ui.message.text = "Adding 1 MP and Reducing CP by 1";
            gs.MP++;
            gs.CP--;
            isElderActionComplete = true;
        }

        public void clickedAdd1CPfor1MP()
        {
            ui.headline.text = "Adding 1 CP and Reducing MP by 1";
            gs.CP++;
            gs.MP--;
            isElderActionComplete = true;
        }

        void StepFour()
        {
            ui.headline.text = "Planning\nStep Four";
            ui.message.text = "Step Four";
        }

        void StepFive()
        {
            ui.headline.text = "Planning\nStep Five";
            ui.message.text = "Step Five";
        }

        void StepSix()
        {
            ui.headline.text = "Planning\nStep Six";
            ui.message.text = "Step Six";
        }
    }
}
