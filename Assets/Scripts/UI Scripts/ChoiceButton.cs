using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ChoiceButton : Button
    {
        public string testString { get; set; }
        public Action passback;
        public GameStep gameStep { get; set; }

        public ChoiceButton()
        {
            text = "Test Button";
        }

        public void ChangeText(string newtext)
        {
            text = newtext;
        }

        // https://docs.unity3d.com/Packages/com.unity.ui.builder@1.0/manual/uib-structuring-ui-custom-elements.html
        public new class UxmlFactory : UxmlFactory<ChoiceButton, UxmlTraits> { }

        // this replaces existing fields for text, etc.
        // cannot use custom types enum could be useful?
        /*public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription testString =
                new UxmlStringAttributeDescription { name = "test-string", defaultValue = "default" };

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
            }
        }*/
    }
}
