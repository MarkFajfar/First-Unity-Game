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
        public GameObject LogicObject;
        protected LogicScript logic;

        public delegate void ChoiceMadeEventHandler(object sender, ChoiceMade args);
        public static event ChoiceMadeEventHandler ChoiceMadeEvent;

        public delegate void ChoiceMadeObjectEventHandler(object sender, ChoiceMadeObject args);
        public static event ChoiceMadeObjectEventHandler ChoiceMadeObjectEvent;

        protected IReceive sender = null;
        protected List<IReceive> Receivers;

        public bool isEvent;

        protected virtual void OnChoiceMade(ChoiceMade e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeEventHandler raiseEvent = ChoiceMadeEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        protected virtual void OnChoiceMadeObject(ChoiceMadeObject e)
        {
            // use this same OnChoiceMade method whether by button, foldout, etc.
            ChoiceMadeObjectEventHandler raiseEvent = ChoiceMadeObjectEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            logic = LogicObject.GetComponent<LogicScript>();
            
            // make list of all receivers in the scene
            // necessary only to "broadcast" method calls
            Receivers = new List<IReceive>();
            var Scripts = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is IReceive);
            foreach (var script in Scripts)
            {
                foreach (IReceive receiver in
                    script.gameObject.GetComponents<IReceive>().Where(r => !Receivers.Contains(r)))
                { Receivers.Add(receiver); }
            }
            print("No. of Receivers = " + Receivers.Count);
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

        public delegate void ClickNext();
        public event ClickNext OnOpsNextClick;
        public delegate void ClickBack();
        public event ClickBack OnOpsBackClick;

        public virtual void nextClicked()
        { OnOpsNextClick?.Invoke(); }

        public virtual void backClicked()
        { OnOpsBackClick?.Invoke(); }

        public virtual void prevClicked()
        { gm.PrevScene(); }

        public virtual void quitClicked()
        { gm.ExitGame(); }

        public virtual void unsubBack()
        {
            if (OnOpsBackClick != null)
            {
                foreach (UIScript.ClickBack subscriber in OnOpsBackClick.GetInvocationList().Cast<UIScript.ClickBack>())
                { OnOpsBackClick -= subscriber; }
            }
        }

        public virtual void unsubNext()
        {
            if (OnOpsNextClick != null)
            {
                foreach (UIScript.ClickNext subscriber in OnOpsNextClick.GetInvocationList().Cast<UIScript.ClickNext>())
                { OnOpsNextClick -= subscriber; }
            }
        }

        public void DisplayChoiceButtons(List<bParams> choices)
        {
            isEvent = false;
            sender = null;
            MakeChoiceButtons(choices);
        }

        public void DisplayChoiceButtons(IReceive senderobj, List<bParams> choices)
        {
            isEvent = false;
            sender = senderobj;
            MakeChoiceButtons(choices);
        }

        public void DisplayChoiceButtonsEvent(List<bParams> choices)
        {
            isEvent = true;
            sender = null;
            MakeChoiceButtons(choices);
        }

        protected abstract void MakeChoiceButtons(List<bParams> choices);

        public abstract void ClearChoicePanel();
        
        protected void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;
            int choiceIndex = clickedButton.tabIndex; 
            string choiceText = clickedButton.name;

            ClearChoicePanel();

            if (sender != null)
            {
                print("Choice sender: " + choiceText);
                sender.methodManager(choiceText);
                //sender = null; // reset in display
            }
            else if (isEvent)
            {
                print("Choice event: " + choiceText);
                if (clickedButton.userData as GameStep)
                {
                    GameStep gamestep = (GameStep)clickedButton.userData;
                    OnChoiceMadeObject(new ChoiceMadeObject(gamestep));
                }
                else
                {
                    OnChoiceMade(new ChoiceMade(choiceIndex, choiceText));
                }
                //isEvent = false;
            }
            else
            {
                print("Choice broadcast: " + choiceText);
                // how to test if cast is valid
                //var clickedStep = clickedButton.userData as tGameStep;
                //if (clickedStep != null)
                if (clickedButton.userData as GameStep)
                {
                    GameStep clickedStep = (GameStep)clickedButton.userData;
                    clickedStep.Begin();
                }
                else
                {
                    foreach (var receiver in Receivers)
                        receiver.methodManager(choiceText);
                }
                // if (clickedButton.userData.GetType() == typeof(tGameStep))
            }
        }

        public abstract void DisplayLocations();

        public abstract void CloseLocations();

        public abstract void showButton(Button button);
        
        public abstract void showButton(bParams bparams);

        public abstract void MakeFamilyFoldouts(Dictionary<Person, GameState.Family> foldouts);

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
    }
}
