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
            print("No. of Receivers = " + Receivers.Count());
        }

        VisualElement choicePanel;
        List<Button> buttons;
        List<string> choicesList;
        public RadioButtonGroup locations;

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

            locations = root.Q<RadioButtonGroup>("Locations");
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

        public void CloseChoices()
        { 
            foreach (var button in buttons) button.style.display = DisplayStyle.None; 
        }

        void buttonClicked(ClickEvent evt)
        {
            CloseChoices();
            var clickedButton = evt.target as Button;
            int choiceIndex = choicesList.IndexOf(clickedButton.text); 
            string choiceText = "clicked" + clickedButton.text;
            if (sender != null)
            {
                print("Choice sender: " + choiceText);
                sender.methodManager(choiceText);
                //sender = null; // reset in display
            }
            else if (isEvent)
            {
                print("Choice event: " + choiceText);
                OnChoiceMade(new ChoiceMade(choiceIndex, choiceText));
                //isEvent = false;
            }
            else
            {
                print("Choice broadcast: " + choiceText); 
                foreach (var receiver in Receivers)
                    receiver.methodManager(choiceText);
            }
        }

        public void DisplayLocations()
        {
            List<string> locationNames = new()
            { " Splitrock", " San Juan", " Zuni", " Monument", " Hopi", " Black Mesa", " C. de Chelly" };
            isEvent = false;
            sender = null;
            locations.style.display = DisplayStyle.Flex;
            locations.choices = locationNames;
        }

        public void CloseLocations() 
            { locations.style.display = DisplayStyle.None; }

    }
}
