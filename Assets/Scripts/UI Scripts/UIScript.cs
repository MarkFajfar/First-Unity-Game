using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected void choiceButtonClicked(EventBase evtBase)
        {
            // any choice-specific step to add?
            var evt = evtBase as ClickEvent;
            buttonClicked(evt);
        }

        protected void choiceButtonClicked(ClickEvent evt) 
        {
            // any choice-specific step to add?
            buttonClicked(evt);
        }

        protected void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;

            if (clickedButton.userData is GameStep clickedStep) 
            {
                ClearChoicePanel(); // would a next step ever need to keep the panel?
                clickedStep.Begin();
            }
            else if (clickedButton.userData is Action call) call?.Invoke();
            // clear panel in called action
            else if (clickedButton.userData is ButtonInfo info) 
            {
                //ButtonInfo clickedParams = (ButtonInfo)clickedButton.userData;

                if (info.clearPanel) ClearChoicePanel();
                // set to false to leave panel (eg for foldout)

                if (info.waiting)
                // if waiting, send back choice (usually to a loop)
                {
                    //ChoiceMadeParams choice = new(info);
                    //ChoiceMadeParams choice = new ChoiceMadeParams((ButtonInfo)clickedButton.userData);
                    //choice.OnChoiceMadeParams(choice);
                    OnChoiceMadeParams(info);
                }
                else if (info.passBack != null)
                // if there is a passBack function, send back the params and stop
                {
                    info.passBack.Invoke(info);
                }
                else if (info.call != Info.InvalidMessage && info.gameStep == null)
                {
                    info.call?.Invoke();
                }
                else if (info.gameStep != null && info.call == Info.InvalidMessage)
                {
                    info.gameStep.Begin();
                }
                else if (info.gameStep != null && info.call != Info.InvalidMessage)
                {
                    displayText("Invalid Choice Selected");
                    Debug.Log("Cannot have both GameStep and Action call");
                    // would then default to methodManager call ?
                }
                else
                {
                    Debug.Log($"ButtonInfo received, but no valid action. Trying to call {info.name} on receivers");
                    foreach (var receiver in Receivers) receiver.methodManager(info.name);
                }
            }
            else
            {
                // do not clear panel in this case; do that in called method?
                Debug.Log($"No button userdata received. Trying to call {clickedButton.name} on receivers");
                foreach (var receiver in Receivers) receiver.methodManager(clickedButton.name);
            }
        }

        Action<ButtonInfo> OnChoiceMadeParams;
        
        public async Task<ButtonInfo> GetChoiceAsyncParams()
        {
            var result = new TaskCompletionSource<ButtonInfo>();
            Action<ButtonInfo> eventHandler = (info) => result.SetResult(info);
            OnChoiceMadeParams += eventHandler;
            await result.Task;
            OnChoiceMadeParams -= eventHandler;
            return result.Task.Result;
        }

        protected void foldoutButtonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;

            if (clickedButton.userData is ButtonInfo info)
            {
                //ButtonInfo clickedParams = (ButtonInfo)clickedButton.userData;

                if (info.closeFoldout) // can set to false in button
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

        protected void toggleValueChanged(ChangeEvent<bool> evt)
        {
            var toggle = evt.target as Toggle;
            var info = toggle.userData as ToggleInfo;
            info.passBack?.Invoke(info, toggle.value);
            // no other reaction other than passback?
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

        protected virtual void statusClicked()
        { }

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
