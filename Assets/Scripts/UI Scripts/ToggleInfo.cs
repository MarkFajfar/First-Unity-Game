using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class ToggleInfo
    {
        public bool value = false;
        public string label;
        public string name;
        public string text = "";
        public int tabIndex = 0;
        public string style = "Toggle";
        public object toggleData = null;
        public GameStep gameStep = null;
        public Action call = null;
        public Action<ToggleInfo, bool> passBack = null;
        public bool waiting = false;
        public bool clearPanel = true;
        public bool closeFoldout = true;

        // experimental:
        public string parentName = "";
        // foldoutName never added in constructor, but only when button put into foldout
        public object parentData = null;
    }
}
