using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ButtonInfo : Info
    {
        public string name;
        public string text;
        public int tabIndex = 0;
        public string style = "ButtonMenu";

        public Action<ButtonInfo> passBack = null;

        /// <summary>
        /// make button from info, including style
        /// </summary>
        /// <returns>button object</returns>
        public Button Make()
        {
            Button button = new()
            {
                name = name,
                text = text,
                tabIndex = tabIndex,
                userData = this
            };
            button.AddToClassList(style);
            if (remove) RemoveWhenClicked(button);
            return button;
        }

        // not necessary because called in ButtonClicked
        /// <summary>
        /// make button from info that when clicked 
        /// will invoke callback and pass info to passback
        /// </summary>
        /// <returns>button with call assigned to click function</returns>
        public Button MakeWithCall()
        {
            Button button = Make(); 
            // first Make using method, then add click fuctions
            if (call != InvalidMessage) button.clicked += call.Invoke;
            
            if (passBack != null)
            {
                button.clickable.clickedWithEventInfo += (EventBase evt) =>
                {
                    var dButton = evt.target as Button;
                    if (dButton.userData is ButtonInfo info)
                        info.passBack.Invoke(info);
                };
            }
            ;
            //button.AddToClassList(style);
            return button;
        }

        public static void RemoveWhenClicked(Button button) 
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                var b = evt.target as Button;
                b.RemoveFromHierarchy();
            });
        }

        public ButtonInfo()
        {
            name = "NewButton";
            text = "New Button";
        }

        public ButtonInfo(string text)
        {
            name = text.Replace(" ", "");
            this.text = text;
        }

        // change to data rather than tabIndex?
        public ButtonInfo(string text, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.tabIndex = tabIndex;
        }

        public ButtonInfo(string text, GameStep gameStep)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.gameStep = gameStep;
        }

        public ButtonInfo(string text, Action call)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.call = call;
        }

        public ButtonInfo(string text, int tabIndex, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.tabIndex = tabIndex;
            passBack = passBackInfo; 
        }

        public ButtonInfo(string text, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            passBack = passBackInfo;
        }

        public ButtonInfo(string name, string style, Action<ButtonInfo> passBackInfo)
        {
            this.name = name.Replace(" ", "");
            text = "";
            this.style = style;
            passBack = passBackInfo;
        }

        public ButtonInfo(string name, string style, int tabIndex, Action<ButtonInfo> passBackInfo)
        {
            this.name = name.Replace(" ", "");
            text = "";
            this.style = style;
            this.tabIndex = tabIndex;
            passBack = passBackInfo;
        }

        public ButtonInfo(string text, GameStep gameStep, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.gameStep = gameStep;
            passBack = passBackInfo;
        }

        public ButtonInfo(string text, Action call, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.call = call;
            passBack = passBackInfo;
        }

        public ButtonInfo(string name, string text, int tabIndex)
        {
            this.name = name;
            this.text = text;
            this.tabIndex = tabIndex;
        }

        public ButtonInfo(string name, string text, int tabIndex, GameStep gameStep, Action call)
        {
            this.name = name;
            this.text = text;
            this.tabIndex = tabIndex;
            this.gameStep = gameStep;
            this.call = call;
        }

        public ButtonInfo(string name, string text, int tabIndex, GameStep gameStep, Action call, Action<ButtonInfo> passBackInfo)
        {
            this.name = name;
            this.text = text;
            this.tabIndex = tabIndex;
            this.gameStep = gameStep;
            this.call = call;
            passBack = passBackInfo;
        }
    }

    public class ButtonInfoEventArgs : EventArgs
    {
        public ButtonInfoEventArgs(List<ButtonInfo> choices)
        {
            eButtonInfo = choices;
        }

        public List<ButtonInfo> eButtonInfo { get; set; }
    }
}
