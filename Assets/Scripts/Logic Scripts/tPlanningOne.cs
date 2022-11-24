using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class tPlanningOne : tGameStep
    {
        public int tIndex;
        public override void choiceIndex(int index)
        {
            print("Planning Index: " + tIndex);
        }

        public override void choiceText(string text)
        {
            print("Planning Text: " + text);
        }
    }
}
