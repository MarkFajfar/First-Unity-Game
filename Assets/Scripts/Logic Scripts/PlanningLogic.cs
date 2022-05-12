using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class PlanningLogic : MonoBehaviour, IMethodReceiver
    {
        GameManager gm;
        GameState gs;
        OperationsUIScript ui;
        ChoiceUIScript choice;
        ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
        List<Action> planningSteps;
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
            planningSteps = new ();
            planningSteps.Add(clickedPlanning);
            planningSteps.Add(StepOne);
            planningSteps.Add(StepTwo);
            planningSteps.Add(StepThree);
            planningSteps.Add(StepFour);
            planningSteps.Add(StepFive);
            planningSteps.Add(StepSix);
            stepDone = new () { false, false, false, false, false, false };
        }
               
        bool isDieRolled = false;
        bool isDieRollSucceeded = false;
        void diceManager(bool isDieRollSuccess)
        {
            //if (isDieRollSuccess) do something ...
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
                gm.SaveGame();
            }
        }

        void StepTwo()
        {
            ui.headline.text = "Planning\nStep Two";
            ui.message.text = "Advance each Elder one box to the right. Elders in the right-most box stay put.";
            if (!stepDone[2])
            {
                ui.message.text += $"Collect {gs.ElderDisplay.Sum()} AP for Elders, so now there are {gs.AP + gs.ElderDisplay.Sum()} AP."; 
                gs.AP += gs.ElderDisplay.Sum();
                for (int i = 6; i > 0; i--)
                {
                    gs.ElderDisplay[i] += gs.ElderDisplay[i - 1];
                    gs.ElderDisplay[i - 1] = 0;
                }
                stepDone[2] = true;
                gm.SaveGame();
            }
            else
            { ui.message.text += "Collect one AP for each Elder."; }
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
            ui.message.text = "";
            for (int i = 1; i < gs.ElderDisplay.Length; i++)
            {
                if (gs.ElderDisplay[i] > 0)
                {
                    int j = 0;
                    while (j < gs.ElderDisplay[i])
                    {
                        ui.headline.text = $"{gs.ElderDisplay[i] - j} Elder Action(s) in\nSlot {i}. Roll Die.";
                        if (!isDieRollSucceeded) ui.message.text = ""; // no need to save message if roll failed
                        ui.message.text += $"For this roll you need a {gs.ElderTarget[i]} or less.";
                        isDieRolled = false;
                        isDieRollSucceeded = false;
                        choice.DisplayChoices(this, new List<string> {"Roll Succeeded", "Roll Failed" });
                        yield return new WaitUntil(() => isDieRolled);
                        if (isDieRollSucceeded)
                        {
                            ui.message.text = " ";
                            ui.headline.text = $"{gs.ElderDisplay[i] - j} Elder Action(s) in\nSlot {i}.  Choose Action."; 
                            isElderActionComplete = false;
                            List<string> elderChoices = new() { "Add 1 AP", "Change Family Ferocity" };
                            if (gs.CP > gs.MP) elderChoices.Add("Add 1 MP for 1 CP");
                            if (gs.MP > gs.CP) elderChoices.Add("Add 1 CP for 1 MP");
                            choice.DisplayChoices(this, elderChoices);
                            yield return new WaitUntil(() => isElderActionComplete);
                        }
                        j++;
                    }
                }
            }
            ui.headline.text = "Elder Actions Completed";
            stepDone[3] = true;
            ui.showBackNext();
            gm.SaveGame();
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
            ui.message.text = "Adding 1 AP\n";
            gs.AP++;
            isElderActionComplete = true;
        }

        List<string> selectedF = new ();
        GameState.Family selectedFamily;
        int selectedIndex;
        List<string> listFerocityNames = new ();
        public async void clickedChangeFamilyFerocity()
        {
            ui.message.text = "Change one Family +/- 1. If no Man, not over 0. If increased and MP<5, add 1 MP. If decreased and CP<5, add 1 CP. Select Family.";
            listFerocityNames = gs.Families.Where(f => f.IsActive && f.HasMan && !selectedF.Contains(f.Name)).Select(f => f.Name).ToList();

                var result = new TaskCompletionSource<string>();
                // redefine the event handler to refer to this specific task
                // this code is the same every time, it just refers to a new instance of result?
                choiceEventHandler = (s,e) =>
                {
                    result.SetResult(e.ChoiceText); 
                    choice.ChoiceMadeEvent -= choiceEventHandler;
                };
                //choice.ChoiceMadeEvent += (s, e) => result.SetResult(e.ChoiceText);
                choice.ChoiceMadeEvent += choiceEventHandler;
                choice.DisplayChoicesEvent(listFerocityNames);
                await result.Task;
                string choiceText = result.Task.Result;

            // convert back to family name and add to end of list
            choiceText = choiceText.Replace("clicked", "");//.Insert(6, " ");  // choiceText still has space
            selectedF.Add(choiceText);
            //selectedIndex = gs.Families.FindIndex(f => f.Name == choiceText);
            selectedFamily = gs.Families.First(f => f.Name == choiceText);
            string MPremind = gs.MP < 5 ? "Increase will add 1 MP. " : "Increase will not add MP. ";
            string CPremind = gs.CP < 5 ? "Decrease will add 1 CP. " : "Decrease will not add CP.";
            if (selectedFamily.Ferocity == 3)
            {
                ui.message.text = CPremind;
                choice.DisplayChoices(new List<string> { "Decrease" });
            }
            else if (selectedFamily.Ferocity == 0)
            {
                ui.message.text = MPremind;
                choice.DisplayChoices(new List<string> { "Increase" });
            }
            else
            {
                ui.message.text = $"Increase or Decrease Ferocity for {choiceText}? " + MPremind + CPremind;
                choice.DisplayChoices(new List<string> { "Increase", "Decrease" }); 
            }
        }

        public void clickedIncrease()
        {
            selectedFamily.Ferocity++; // gs.Families[selectedIndex].Ferocity++;
            gs.MP += gs.MP < 5 ? 1 : 0;
            ui.message.text = $"{selectedFamily.Name} Ferocity is now {selectedFamily.Ferocity} and there are {gs.MP} MP.\n";
            print($"Ferocity A: {gs.Families[0].Ferocity} B: {gs.Families[1].Ferocity} C: {gs.Families[2].Ferocity}");
            print($"Evasion A: {gs.Families[0].Evasion} B: {gs.Families[1].Evasion} C: {gs.Families[2].Evasion}");
            isElderActionComplete = true;
        }

        public void clickedDecrease()
        {
            selectedFamily.Ferocity--;
            gs.CP += gs.CP < 5 ? 1 : 0;
            ui.message.text = $"{gs.Families[selectedIndex].Name} Ferocity is now {gs.Families[selectedIndex].Ferocity} and there are {gs.CP} CP.\n";
            print($"Ferocity A: {gs.Families[0].Ferocity} B: {gs.Families[1].Ferocity} C: {gs.Families[2].Ferocity}");
            print($"Evasion A: {gs.Families[0].Evasion} B: {gs.Families[1].Evasion} C: {gs.Families[2].Evasion}");
            isElderActionComplete = true;
        }


        public void clickedAdd1MPfor1CP()
        {
            ui.message.text = "Adding 1 MP and Reducing CP by 1\n";
            gs.MP++;
            gs.CP--;
            isElderActionComplete = true;
        }

        public void clickedAdd1CPfor1MP()
        {
            ui.message.text = "Adding 1 CP and Reducing MP by 1\n";
            gs.CP++;
            gs.MP--;
            isElderActionComplete = true;
        }

        async void StepFour()
        {
            gs.TradeGoodsMax = 3; // delete after testing
            ui.headline.text = "Planning\nStep Four";
            int tgAvailable = gs.TradeGoodsMax - gs.TradeGoodsHeld;
            if (gs.CP <= 0 || gs.AP <= 0 || tgAvailable <= 0)
            {
                ui.message.text = "Not possible to use CP to buy Trade Goods.";
                stepDone[4] = true;
            }
            else
            {
                int toSpend = Math.Max(gs.Families.Where(f => f.HasWoman).Count(), tgAvailable);
                ui.hideBackNext();
                ui.message.text = $"You may spend up to {toSpend} AP to purchase Trade Goods. How many?";
                List<string> buttonsSpend = new();
                for (int i = 0; i < (toSpend + 1); i++) buttonsSpend.Add((i).ToString());
                choice.DisplayChoices(buttonsSpend);
            }
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
