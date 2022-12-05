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
        public GameStep userData;

        public bParams()
        {
            name = "new button";
            text = "Button";
            style = "ButtonMenu";
            tabIndex = 0;
            userData = null;
        }

        public bParams(string text)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            userData = null;
        }

        public bParams(string text, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            userData = null;
        }
        public bParams(string text, GameStep userData)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "ButtonMenu";
            tabIndex = 0;
            this.userData = userData;
        }

        public bParams(string name, string text, int tabIndex)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            userData = null;
        }

        public bParams(string name, string text, int tabIndex, GameStep userData)
        {
            this.name = name;
            this.text = text;
            style = "ButtonMenu";
            this.tabIndex = tabIndex;
            this.userData = userData;
        }
    }
}
