using UnityEngine.UIElements;

namespace NavajoWars
{
    public static class MakeFromInfo 
    {
        

        public static Foldout MakeFoldoutFromInfo(FoldoutInfo info)
        {
            Foldout foldout = new()
            {
                value = false,
                name = info.name,
                text = info.text,
                tabIndex = info.tabIndex,
                userData = info
            };

            foreach (ButtonInfo bparams in info.buttons)
            {
                // add foldout name and even data to bparams before making button?
                bparams.parentName = info.name;
                bparams.parentData = info;
                // note allows treatment of info as object, but does not "know" that parentData in this case is FoldoutInfo
                bparams.clearPanel = false; // do not clear panel when foldout clicked
                Button foldoutButton = bparams.Make();
                foldout.Add(foldoutButton);
                foldoutButton.style.display = DisplayStyle.Flex;
                foldoutButton.visible = true;
            }

            foldout.AddToClassList(info.style);
            return foldout;
        }

        public static Toggle MakeToggleFromInfo(ToggleInfo info) 
        {
            Toggle toggle = new()
            {
                value = false,
                label = info.label,
                name = info.name,
                text = info.text,
                tabIndex = info.tabIndex,
                userData = info,
            };
            toggle.AddToClassList(info.style);
            return toggle;
        }
    }
}
