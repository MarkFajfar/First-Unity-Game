using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class Raid : GameStep
    {
        public override string stepName { get => "Raid"; }

        public override void Begin()
        {
            //ui.displayHeadline($"{selectedFamily.Name} Raids New Mexico";
            //ui.displayText("Placeholder for raid. ";

        }

        protected override void actionComplete()
        {
            base.actionComplete();
            ChooseAnotherAction chooseAnotherAction = GetComponent<ChooseAnotherAction>();
            chooseAnotherAction.Begin();
        }

        public override void Undo()
        {
            
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            Begin();
            // stuff to do on undo
        }
    }
}