using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class bParams
    {
        public string name;
        public string text;
        public int tabIndex;
        public string style;
        public GameStep gameStep = null;
        public Action call = null;
        public bool waiting = false;

        public bParams()
        {
            name = "new button";
            text = "Button";
            style = "ButtonMenu";
            tabIndex = 0;
        }

        public bParams(string text)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
        }

        public bParams(string text, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
        }
        public bParams(string text, GameStep gameStep)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.gameStep = gameStep;
        }

        public bParams(string text, Action call)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.call = call;
        }


        public bParams(string name, string text, int tabIndex)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
        }

        public bParams(string name, string text, int tabIndex, GameStep gameStep, Action call)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            this.gameStep = gameStep;
            this.call = call;
        }
    }

    public class bParamsEventArgs : EventArgs
    {
        public bParamsEventArgs(List<bParams> choices)
        {
            eParams = choices;
        }

        public List<bParams> eParams { get; set; }
    }
}
