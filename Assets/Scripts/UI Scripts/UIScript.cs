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

        public abstract void ShowChoiceButtons(List<ButtonInfo> choices);

        public void MakeChoiceButtonsAsync(List<ButtonInfo> choices)
        {
            foreach (var choice in choices) { choice.waiting = true; }
            ShowChoiceButtons(choices);
        }

        public abstract void ClearChoicePanel();

        protected void choiceButtonClicked(ClickEvent evt) 
        {
            // any choice-specific step to add?
            buttonClicked(evt);
        }

        protected void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;

            if (clickedButton.userData is GameStep) //.GetType() == typeof(GameStep))
            // does not work:  if (clickedButton.userData as GameStep)
            {
                ClearChoicePanel(); // would a next step ever need to keep the panel?
                GameStep clickedStep = (GameStep)clickedButton.userData;
                clickedStep.Begin();
            }
            else if (clickedButton.userData is ButtonInfo) //.GetType() == typeof(ButtonInfo))
            {
                ButtonInfo clickedParams = (ButtonInfo)clickedButton.userData;

                if (clickedParams.clearPanel) ClearChoicePanel(); 
                // set to false to leave panel (eg for foldout)

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
                    // would then default to methodManager call ?
                }
                else
                {
                    foreach (var receiver in Receivers) receiver.methodManager(clickedParams.name);
                }
            }
            else
            {
                // do not clear panel in this case; do that in called method?
                foreach (var receiver in Receivers) receiver.methodManager(clickedButton.name);
            }
        }

        protected void foldoutButtonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;

            if (clickedButton.userData.GetType() == typeof(ButtonInfo))
            {
                ButtonInfo clickedParams = (ButtonInfo)clickedButton.userData;

                if (clickedParams.closeFoldout) // can set to false in button
                { 
                    clickedButton.parent.style.display = DisplayStyle.None;
                    // if necessary: clickedButton.parent.visible = false;
                    // may need a reference to foldout if not direct parent
                    // foldout is still present, just not displayed
                    // setting foldout "value=false" would close up foldout but leave it displayed
                }
            }

            print("Foldout Parent: " + clickedButton.parent);

            buttonClicked(evt);
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

        public abstract void ShowChoiceFoldouts(List<FoldoutInfo> foldouts);
    }
}
