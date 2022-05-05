using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ChoiceUIScript : MonoBehaviour, IsUIScript
    {
        VisualElement choicePanel;
        List<Button> buttons;

        string choiceText;

        public delegate void ChoiceMade(string choiceText);
        public event ChoiceMade OnChoiceMade;

        /*void Awake()
        {
            gameObject.SetActive(false);
        }*/

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
        public void DisplayChoices(List<string> choices)
        {
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
            choiceText = clickedButton.text;
            OnChoiceMade?.Invoke(choiceText);
        }     
    }
}
