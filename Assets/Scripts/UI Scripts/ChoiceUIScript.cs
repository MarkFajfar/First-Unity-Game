using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    
    public class ChoiceUIScript : MonoBehaviour
    {
        public bool isEvent;

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
        // List<Button> buttons;
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
        public void DisplayChoiceButtons(List<bParams> choices)
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

        public void DisplayChoiceButtons(IReceive senderobj, List<bParams> choices)
        {
            isEvent = false;
            sender = senderobj;
            MakeChoiceButtons(choices);
        }

        /*public void DisplayChoiceButtonsEvent(List<string> choices)
        {
            isEvent = true;
            sender = null;
            MakeChoiceButtons(choices);
        }
*/
        public void DisplayChoiceButtonsEvent(List<bParams> choices)
        {
            isEvent = true;
            sender = null;
            MakeChoiceButtons(choices);
        }

        public void MakeChoiceButtons(List<bParams> choices)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            foreach (bParams choice in choices)
            {
                var choiceButton = new Button();
                choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
                choiceButton.AddToClassList(choice.style);
                //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
                choiceButton.name = choice.name;
                choiceButton.text = choice.text;
                choiceButton.tabIndex = choice.tabIndex;
                choiceButton.userData = choice.userData;
                choicePanel.Add(choiceButton);
                choiceButton.style.display = DisplayStyle.Flex;
            }
        }

        /*public void MakeChoiceButtons(List<string> choices)
        {
            choicesList = choices;
            // TODO: fix so separate variable for choicesList not necessary
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            for (int i = 0; i < choices.Count; i++)
            {
                var choiceButton = new Button();
                choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
                choiceButton.AddToClassList("ButtonMenu");
                //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
                choiceButton.text = choices[i];
                choiceButton.tabIndex = i;
                int n = i + 5;
                choiceButton.userData = (PlayerActionLogic.oldGameSteps)n;
                choicePanel.Add(choiceButton);
                choiceButton.style.display = DisplayStyle.Flex;
            }
        }*/

        public void CloseChoiceButtons()
        {
            List<Button> buttons = choicePanel.Query<Button>().ToList();
            foreach (var button in buttons)
            {
                // print("Remove " + button.text);
                choicePanel.Remove(button);
            }
            // foreach (var button in buttons) button.style.display = DisplayStyle.None; 

            List<VisualElement> choiceElements = choicePanel.Query<VisualElement>().ToList();
        }

        void buttonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;
            int choiceIndex = clickedButton.tabIndex; // default is 0
            //if (clickedButton.name == "") clickedButton.name = "clicked" + clickedButton.text.Replace(" ", "");
            //following does not work because blank string, not null
            //clickedButton.name ??= "clicked" + clickedButton.text.Replace(" ", "");
            string choiceText = clickedButton.name;

            CloseChoiceButtons();

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
                    //OnChoiceMadeObject(new ChoiceMadeObject(gamestep));
                }
                else
                {
                    //OnChoiceMade(new ChoiceMade(choiceIndex, choiceText));
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
                    // can successfully cast
                    GameStep clickedStep = (GameStep)clickedButton.userData;
                    clickedStep.Begin();
                }
                else
                {
                    // cast failed
                    foreach (var receiver in Receivers)
                    receiver.methodManager(choiceText);
                }
                // if (clickedButton.userData.GetType() == typeof(tGameStep))
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

        public void MakeFamilyFoldouts(Dictionary<Person, GameState.Family> foldouts)
        // (List<Person> childrenInPassage, List<GameState.Family> childrenInFamilies)
        {
            // TODO: make this function generic ?? "MakeFoldouts"
            
            CloseChoiceButtons();

            choicePanel.style.display = DisplayStyle.Flex;

            foreach (KeyValuePair<Person, GameState.Family> foldout in foldouts)
            // for (int i = 0; i < (childrenInPassage.Count() + childrenInFamilies.Count()); i++)
            {
                // Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);

                var choiceFoldout = new Foldout();
                choiceFoldout.AddToClassList("FoldoutChild");
                choiceFoldout.value = false;
                choiceFoldout.style.display = DisplayStyle.Flex;
                choiceFoldout.text = foldout.Key.ToString(); // child? family name?

                // each child can be converted into a man, woman or elder

                List<Person> foldoutPersons = new () { Person.Man, Person.Woman, Person.Elder} ;
                int n = 0;
                foreach (var person in foldoutPersons)
                {
                    var foldoutButton = new Button();
                    foldoutButton.RegisterCallback<ClickEvent>(foldoutClicked);
                    foldoutButton.AddToClassList("ButtonFoldout");  // create this style
                    foldoutButton.text = person.ToString();
                    n++;
                    foldoutButton.tabIndex = n;
                    foldoutButton.userData = person;
                    choiceFoldout.Add(foldoutButton);
                    foldoutButton.style.display = DisplayStyle.Flex;
                }
            }
        }

        void foldoutClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as Button;
            int choiceIndex = clickedButton.tabIndex; // choicesList.IndexOf(clickedButton.text); 
            string choiceText = "clicked" + clickedButton.parent + clickedButton.text;

            print("Parent: " + clickedButton.parent);
            print("Data: " + clickedButton.userData);
            print("Index: " + clickedButton.tabIndex);

            CloseChoiceButtons();

            // add call to foldout action -- how to make it generic?
        }

        public void showButton(Button button)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            button.RegisterCallback<ClickEvent>(buttonClicked);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }

        public void showButton(bParams bparams) 
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            Button choiceButton = new();
            choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
            choiceButton.AddToClassList(bparams.style);
            //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
            choiceButton.name = bparams.name;
            choiceButton.text = bparams.text;
            choiceButton.tabIndex = bparams.tabIndex;
            choiceButton.userData = bparams.userData;
            choiceButton.style.display = DisplayStyle.Flex;
            choicePanel.Add(choiceButton);
        }
        
 /*       public void showButton(PlanningLogic.buttonParams buttonParams) 
        {
            var choiceButton = new Button();
            choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
            choiceButton.AddToClassList("ButtonMenu");
            //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
            choiceButton.text = buttonParams.text;
            choiceButton.tabIndex = buttonParams.tabIndex;
            choiceButton.userData = buttonParams;
            choicePanel.Add(choiceButton);
        }*/
    }
}
