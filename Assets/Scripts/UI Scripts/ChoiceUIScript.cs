using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            // use this same OnChoiceMade method whether by button, foldout, etc.
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

        public string[] choiceButtonStyles;
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

            // move so only find buttons after they are created in choicePanel
            // same for other types of UI - radio buttons, foldouts, etc.
 /*           buttons = choicePanel.Query<Button>().ToList();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].RegisterCallback<ClickEvent>(buttonClicked);
            }*/

            locations = root.Q<RadioButtonGroup>("Locations");
        }

        // use this where button click goes back to another script, or multiple scripts in scene
        public void DisplayChoiceButtons(List<string> choices)
        {
            isEvent = false;
            sender = null;
            MakeChoiceButtons(choices);
 /*           for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].style.display = DisplayStyle.Flex;
                buttons[i].text = choices[i];
            }*/
        }

        public void DisplayChoiceButtons(IReceive senderobj, List<string> choices)
        {
            isEvent = false;
            sender = senderobj;
            MakeChoiceButtons(choices);
        }

        public void DisplayChoiceButtonsEvent(List<string> choices)
        {
            isEvent = true;
            sender = null;
            MakeChoiceButtons(choices);
        }

        public void MakeChoiceButtons(List<string> choices)
        {
            choicesList = choices;
            // TODO: fix so separate variable for choicesList not necessary
            choicePanel.style.display = DisplayStyle.Flex;
            for (int i = 0; i < choices.Count; i++)
            {
                var choiceButton = new Button();
                choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
                choiceButton.AddToClassList("ButtonMenu");
                //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
                choiceButton.text = choices[i];
                choiceButton.userData = Person.Man;
                choicePanel.Add(choiceButton);
                choiceButton.style.display = DisplayStyle.Flex;
            }
        }

        public void CloseChoiceButtons()
        {
            buttons = choicePanel.Query<Button>().ToList();
            foreach (var button in buttons)
            {
                // print("Remove " + button.text);
                choicePanel.Remove(button);
            }
            // foreach (var button in buttons) button.style.display = DisplayStyle.None; 
        }

        void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;
            int choiceIndex = choicesList.IndexOf(clickedButton.text); 
            string choiceText = "clicked" + clickedButton.text;
            print("Parent: " + clickedButton.parent);
            print("Data: " + clickedButton.userData);
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
            // CloseChoiceButtons(); // call here occurs after new buttons made
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
