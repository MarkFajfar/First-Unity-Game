using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public abstract class UIScript : MonoBehaviour
    {
        protected GameManager gm;
        protected GameState gs;

        protected List<IReceive> Receivers;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            //logic = LogicObject.GetComponent<LogicScript>(); // in Inspector
            
            // make list of all receivers in the scene, to "broadcast" method calls
            Receivers = new List<IReceive>();
            var Scripts = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is IReceive);
            foreach (var script in Scripts)
            {
                foreach (IReceive receiver in
                    script.gameObject.GetComponents<IReceive>().
                    Where(r => !Receivers.Contains(r)))
                { Receivers.Add(receiver); }
            }
        }

        public abstract void MakeChoiceButtons(List<ButtonInfo> choices);

        public void MakeChoiceButtonsAsync(List<ButtonInfo> choices)
        {
            foreach (var choice in choices) { choice.waiting = true; }
            MakeChoiceButtons(choices);
        }

        public abstract void ClearChoicePanel();

        protected void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;

            ClearChoicePanel();

            if (clickedButton.userData.GetType() == typeof(GameStep))
            // does not work:  if (clickedButton.userData as GameStep)
            {
                GameStep clickedStep = (GameStep)clickedButton.userData;
                clickedStep.Begin();
            }
            else if (clickedButton.userData.GetType() == typeof(ButtonInfo))
            {
                ButtonInfo clickedParams = (ButtonInfo)clickedButton.userData;

                if (clickedParams.waiting)
                // if waiting, send back choice (usually to a loop)
                {
                    ChoiceMadeParams choice = new ChoiceMadeParams((ButtonInfo)clickedButton.userData);
                    choice.OnChoiceMadeParams(choice);
                }
                else if (clickedParams.passBack != null)
                // if there is a passBack function, send back the params and stop
                {
                    clickedParams.passBack.Invoke(clickedParams);
                }
                else if (clickedParams.call != null && clickedParams.gameStep == null)
                {
                    clickedParams.call.Invoke();
                }
                else if (clickedParams.gameStep != null && clickedParams.call == null)
                {
                    clickedParams.gameStep.Begin();
                }
                else if (clickedParams.gameStep != null && clickedParams.call != null)
                {
                    Debug.Log("Cannot have both GameStep and Action call");
                }
                else
                {
                    foreach (var receiver in Receivers) receiver.methodManager(clickedParams.name);
                }
            }
            else
            {
                foreach (var receiver in Receivers) receiver.methodManager(clickedButton.name);
            }
        }

        protected void foldoutClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;
            int choiceIndex = clickedButton.tabIndex; // choicesList.IndexOf(clickedButton.text); 
            string choiceText = "clicked" + clickedButton.parent + clickedButton.text;

            print("Parent: " + clickedButton.parent);
            print("Data: " + clickedButton.userData);
            print("Index: " + clickedButton.tabIndex);

            ClearChoicePanel();

            // add call to foldout action -- how to make it generic?
        }

        public Action OnNextClick;
        public Action OnBackClick;

        public virtual void nextClicked()
        { OnNextClick?.Invoke(); }

        public virtual void backClicked()
        { OnBackClick?.Invoke(); }

        public virtual void prevClicked()
        { gm.PrevScene(); }

        public virtual void quitClicked()
        { gm.ExitGame(); }

        public virtual void unsubBack()
        {
            if (OnBackClick != null)
            {
                //foreach (UIScript.ClickBack subscriber in OnOpsBackClick.GetInvocationList().Cast<UIScript.ClickBack>())
                foreach (Action subscriber in OnBackClick.GetInvocationList().Cast<Action>())
                { OnBackClick -= subscriber; }
            }
        }

        public virtual void unsubNext()
        {
            if (OnNextClick != null)
            {
                //foreach (UIScript.ClickNext subscriber in OnOpsNextClick.GetInvocationList().Cast<UIScript.ClickNext>())
                foreach (Action subscriber in OnNextClick.GetInvocationList().Cast<Action>())
                { OnNextClick -= subscriber; }
            }
        }

        public abstract void Initialize();
        public abstract void displayHeadline(string text);
        public abstract void addHeadline(string text);
        public abstract void displayText(string text);
        public abstract void addText(string text);
        public abstract void showBackNext();
        public abstract void hideBackNext();
        public abstract void showPrev();
        public abstract void hidePrev();

        public abstract void showButton(Button button);
        
        public abstract void showButton(ButtonInfo bparams);

        public abstract void MakeFamilyFoldouts(Dictionary<Person, GameState.Family> foldouts);
    }
}
