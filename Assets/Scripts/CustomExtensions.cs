using System.Linq;
// using NavajoWars;
using UnityEngine;
using UnityEngine.UIElements;

//namespace CustomExtensions
namespace NavajoWars
{
    public static class CustomExtensions
    {
        /// <summary>
        /// returns Territory with given Tag
        /// </summary>
        public static Territory ByTag(this eTerritory eT)
        {
            var gmobj = GameObject.FindWithTag("GameController");
            var gs = gmobj.GetComponent<GameState>();
            return gs.Territories.Where(t => t.Tag == eT).First();
        }

        /// <summary>
        /// returns ve in given area with given string name
        /// </summary>
        public static VisualElement ByStringInArea(this VisualElement area, string s)
        {
            VisualElement ve = area.Q(className: s);
            if (ve != null) return ve;
            else
            {
                Debug.LogError($"No VisualElement found for class {s} in {area}");
                return null;
            }
        }

        /// <summary>
        /// set all children in ve to <see cref="DisplayStyle.None"/>
        /// </summary>
        public static void HideChildren(this VisualElement ve)
        {
            foreach (var child in ve.Children())
            { child.style.display = DisplayStyle.None; }
        }

        /// <summary>
        /// set all children in ve to <see cref="DisplayStyle.Flex"/>
        /// </summary>
        /// <param name="ve"></param>
        public static void ShowChildren(this VisualElement ve)
        {
            foreach (var child in ve.Children())
            { child.style.display = DisplayStyle.Flex; }
        }
    }
}
