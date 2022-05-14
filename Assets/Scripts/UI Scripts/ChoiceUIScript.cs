using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ChoiceMade : EventArgs
    {
        public ChoiceMade(int choiceIndex, string choiceText)
        { 
            ChoiceText = choiceText;
            ChoiceIndex = choiceIndex;
        }
        public string ChoiceText { get; set; }
        public int ChoiceIndex { get; set; }
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

        IReceive sender = null;
        List<IReceive> Receivers;

        void Awake()
        {
            // make list of all receivers in the scene
            // necessary only to "broadcast" method calls
            Receivers = new List<IReceive>();
            
            var Scripts = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is IReceive);

            foreach (var script in Scripts)
            {   foreach (IReceive receiver in 
                    script.gameObject.GetComponents<IReceive>().Where(r => !Receivers.Contains(r)))
                { Receivers.Add(receiver); }
            }
        }

        VisualElement choicePanel;
        List<Button> buttons;
        List<string> choicesList;

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
            choicesList = choices;
            isEvent = false;
            sender = null;
            for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].style.display = DisplayStyle.Flex;
                buttons[i].text = choices[i];
            }
        }

        public void DisplayChoices(IReceive senderobj, List<string> choices)
        {
            choicesList = choices;
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
            choicesList = choices;
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
            int choiceIndex = choicesList.IndexOf(clickedButton.text); 
            string choiceText = "clicked" + clickedButton.text;
            if (sender != null)
            {
                sender.methodManager(choiceText);
                //sender = null; // reset in display
            }
            else if (isEvent)
            {
                OnChoiceMade(new ChoiceMade(choiceIndex, choiceText));
                //isEvent = false;
            }
            else
            {
                foreach (var receiver in Receivers)
                    receiver.methodManager(choiceText);
            }
        }        
    }
}
