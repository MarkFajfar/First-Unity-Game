using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class ButtonInfo
    {
        public string name;
        public string text;
        public int tabIndex;
        public string style;
        public GameStep gameStep = null;
        public Action call = null;
        public Action<ButtonInfo> passBack = null;
        public bool waiting = false;

        public ButtonInfo()
        {
            name = "NewButton";
            text = "New Button";
            style = "ButtonMenu";
            tabIndex = 0;
        }

        public ButtonInfo(string text)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
        }

        public ButtonInfo(string text, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
        }

        public ButtonInfo(string text, GameStep gameStep)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.gameStep = gameStep;
        }

        public ButtonInfo(string text, Action call)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.call = call;
        }

        public ButtonInfo(string text, int tabIndex, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            this.passBack = passBackInfo;
        }

        public ButtonInfo(string text, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.passBack = passBackInfo;
        }

        public ButtonInfo(string text, GameStep gameStep, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.gameStep = gameStep;
            passBack = passBackInfo;
        }

        public ButtonInfo(string text, Action call, Action<ButtonInfo> passBackInfo)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.call = call;
            passBack = passBackInfo;
        }

        public ButtonInfo(string name, string text, int tabIndex)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
        }

        public ButtonInfo(string name, string text, int tabIndex, GameStep gameStep, Action call)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            this.gameStep = gameStep;
            this.call = call;
        }

        public ButtonInfo(string name, string text, int tabIndex, GameStep gameStep, Action call, Action<ButtonInfo> passBackInfo)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
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
