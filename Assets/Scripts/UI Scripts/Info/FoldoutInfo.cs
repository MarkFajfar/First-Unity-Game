using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class FoldoutInfo : Info
    {
        public string name;
        public string text;
        public int tabIndex = 0;
        public string style;
        
        public List<ButtonInfo> buttons = null;
        public Action<FoldoutInfo> passBack = null;

        public Foldout Make()
        {
            Foldout foldout = new()
            {
                value = false,
                name = name,
                text = text,
                tabIndex = tabIndex,
                userData = this
            };

            foreach (ButtonInfo bparams in buttons)
            {
                // add foldout name and even data to bparams before making button?
                bparams.parentName = this.name;
                bparams.parentData = this;
                // note allows treatment of info as object, but does not "know" that parentData in this case is FoldoutInfo
                bparams.clearPanel = false; // do not clear panel when foldout clicked
                Button foldoutButton = bparams.Make();
                foldoutButton.AddToClassList("FoldoutItem");
                foldout.Add(foldoutButton);
                foldoutButton.style.display = DisplayStyle.Flex;
                foldoutButton.visible = true;
            }

            foldout.AddToClassList(style);
            return foldout;
        }

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
