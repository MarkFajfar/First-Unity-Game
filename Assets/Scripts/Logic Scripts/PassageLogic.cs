using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class PassageLogic : OperationsLogic
    {
        void Start()
        {
            Steps = new();
            Steps.Add(clickedPassage);
            Steps.Add(StepOne);
            stepDone = new() { false, false, false, false, false, false };
        }

        public void clickedPassage()
        {
            print("Clicked Passage");
            ui.Initialize();
            ui.OnOpsNextClick += nextStep;
            ui.OnOpsBackClick += backStep;
            stepDone[0] = true;
            step = 0;
            StepOne();
        }

        void StepOne()
        {
            int numChildInPassage = gs.PersonsInPassage.FindAll(p => p==Person.Child).Count();
            int numChildInFamilies = gs.Families.Where(f => f.HasChild).Count();
            int numManInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Man).Count();
            int numManInFamilies = gs.Families.Where(f => f.HasMan).Count();
            int numWomanInPassage = gs.PersonsInPassage.FindAll(p => p == Person.Woman).Count();
            int numWomanInFamilies = gs.Families.Where(f => f.HasWoman).Count();
            //if (numChildInPassage + numChildInFamilies > 0) 
        }

        void StepSeven()
        {
            ui.OnOpsNextClick -= nextStep;
            ui.OnOpsBackClick -= backStep;
            ui.PlayerOpsDone();
        }
    }
}
