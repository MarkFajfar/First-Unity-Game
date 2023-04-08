using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class FoldoutInfo 
    {
        public string name;
        public string text;
        public int tabIndex = 0;
        public string style;
        public List<ButtonInfo> buttons = null;
        public bool waiting = false;
        public object foldoutData = null;

        public FoldoutInfo()
        {
            name = "NewFoldout";
            text = "New Foldout";
            style = "FoldoutChild";
        }

        public FoldoutInfo(string text)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "FoldoutChild";
        }

        public FoldoutInfo(string text, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            style = "FoldoutChild";
            this.tabIndex = tabIndex;
        }

        public FoldoutInfo(string text, string style)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.style = style;
        }

        public FoldoutInfo(string text, string style, List<ButtonInfo> buttons)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.style = style;
            this.buttons = buttons;
        }

        public FoldoutInfo(string text, string style, int tabIndex)
        {
            name = text.Replace(" ", "");
            this.text = text;
            this.style = style;
            this.tabIndex = tabIndex;
        }
    }
}
