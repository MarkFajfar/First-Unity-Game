using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ChoiceMade : EventArgs
    {
        public ChoiceMade(string choiceText)
        { 
            ChoiceText = choiceText;
        }
        public string ChoiceText { get; set; }
    }

    public class ChoiceUIScript : MonoBehaviour, IsUIScript
    {
        public delegate void ChoiceMadeEventHandler(object sender, ChoiceMade args);
        public event ChoiceMadeEventHandler ChoiceMadeEvent;
        bool isEvent = false;

        protected virtual void OnChoiceMade(ChoiceMade e)
        {
            ChoiceMadeEventHandler raiseEvent = ChoiceMadeEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        IMethodReceiver sender = null;
        List<IMethodReceiver> Receivers;

        void Awake()
        {
            // make list of all receivers in the scene
            // necessary only to "broadcast" method calls
            Receivers = new List<IMethodReceiver>();
            
            var Scripts = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is IMethodReceiver);

            foreach (var script in Scripts)
            {   foreach (IMethodReceiver receiver in 
                    script.gameObject.GetComponents<IMethodReceiver>().Where(r => !Receivers.Contains(r)))
                { Receivers.Add(receiver); }
            }
        }

        VisualElement choicePanel;
        List<Button> buttons;
        void OnEnable()
        {
            getVisualElements();
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            choicePanel = root.Q<VisualElement>("ChoicePanel");

            buttons = choicePanel.Query<Button>().ToList();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].RegisterCallback<ClickEvent>(buttonClicked);
            }
        }

        // use this where button click goes back to another script, or multiple scripts in scene
        public void DisplayChoices(List<string> choices)
        {
            isEvent = false;
            sender = null;
            for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].style.display = DisplayStyle.Flex;
                buttons[i].text = choices[i];
            }
        }

        public void DisplayChoices(IMethodReceiver senderobj, List<string> choices)
        {
            isEvent = false;
            sender = senderobj;
            for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].style.display = DisplayStyle.Flex;
                buttons[i].text = choices[i];
            }
        }

        public void DisplayChoicesEvent(List<string> choices)
        {
            isEvent = true;
            sender = null;
            for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].style.display = DisplayStyle.Flex;
                buttons[i].text = choices[i];
            }
        }

        void buttonClicked(ClickEvent evt)
        {
            foreach (var button in buttons)
            {
                button.style.display = DisplayStyle.None;
            }
            var clickedButton = evt.target as Button;
            string choiceText = "clicked" + clickedButton.text;
            if (sender != null)
            {
                sender.methodManager(choiceText);
                sender = null; // reset so only this call
            }
            else if (isEvent)
            {
                OnChoiceMade(new ChoiceMade(choiceText));
                isEvent = false;
            }
            else
            {
                foreach (var receiver in Receivers)
                    receiver.methodManager(choiceText);
            }
        }        
    }
}
