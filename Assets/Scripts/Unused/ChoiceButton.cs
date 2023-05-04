using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public enum ButtonType { One, Two, Three, Four }
    
    public class ChoiceButton : Button
    {
        public string testString { get; set; }
        public Action passback;
        public GameStep gameStep { get; set; }
        public ButtonType buttonType { get; set; }

        public ChoiceButton()
        {
            text = "Test Button";
        }

        public void ChangeText(string newtext)
        {
            text = newtext;
        }

        public static void PrintOut()
        {
            Debug.Log("Print Out worked");
        }

        // https://docs.unity3d.com/Packages/com.unity.ui.builder@1.0/manual/uib-structuring-ui-custom-elements.html
        public new class UxmlFactory : UxmlFactory<ChoiceButton, UxmlTraits> { }

        // this replaces existing fields for text, etc.
        // cannot use custom types enum could be useful?
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription testString =
                new UxmlStringAttributeDescription { name = "test-string", defaultValue = "default" };

            UxmlEnumAttributeDescription<ButtonType> buttonType
                = new UxmlEnumAttributeDescription<ButtonType> { name = "button-type", defaultValue = ButtonType.One };

            // this doesn't seem necessary
            //public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            //{
            //    get { yield break; }
            //}

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as ChoiceButton;

                ate.testString = testString.GetValueFromBag(bag, cc);
                ate.buttonType = buttonType.GetValueFromBag(bag, cc);
            }
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            // Call the base function.
            base.ExecuteDefaultActionAtTarget(evt);

            //var gmobj = GameObject.FindWithTag("GameController");
            //var gs = gmobj.GetComponent<GameState>();

            if (evt.eventTypeId == MouseDownEvent.TypeId())
            {
                // ...
            }
            else if (evt.eventTypeId == ClickEvent.TypeId())
            {
                var clickedButton = evt.target as ChoiceButton;
                //gs.AP = 10;
                Debug.Log($"test button clicked {clickedButton.buttonType}");
            }
            // More event types
        }
    }
}
