using System;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class ToggleInfo : Info
    {
        public bool value = false;
        public string label;
        public string name;
        public string text = "";
        public int tabIndex = 0;
        public string style = "Toggle";

        public Action<ToggleInfo, bool> passBack = null;

        public Toggle Make()
        {
            Toggle toggle = new()
            {
                value = false,
                label = label,
                name = name,
                text = text,
                tabIndex = tabIndex,
                userData = this,
            };
            toggle.AddToClassList(style);
            return toggle;
        }

        public ToggleInfo(string label, Family family, Person person, Action<ToggleInfo, bool> passBackInfo)
        {
            this.label = label;
            name = label.Replace(" ", "");
            this.family = family;
            this.person = person;
            passBack = passBackInfo;
        }
    }
}
