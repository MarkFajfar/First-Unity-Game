using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace NavajoWars
{
    public class PassageSeven : GameStep
    {
        public override string stepName { get => "PassageSeven"; }

        public override void Begin()
        {
            gm.SaveUndo(this);
            ui.displayHeadline("Passage of Time\nStep Seven");
            // Elders in left most box to not need to be checked
            if (gs.ElderDisplay.Sum() - gs.ElderDisplay[0] > 0) elderCheck();
            else ui.displayText("No Elders to check. ");
            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        async void elderCheck()
        {
            ui.displayText("");
            for (int i = 1; i < gs.ElderDisplay.Length; i++)
            {
                if (gs.ElderDisplay[i] > 0)
                {
                    ui.addText($"There are {gs.ElderDisplay[i]} Elder(s) in Slot {i + 1}. Roll the die for each one. If a roll is {gs.ElderTarget[i]} or less, that Elder dies and is returned to the Out of Play box. Open the Status menu and select any Elder that is lost. Press 'Check Next Slot' to continue when all Elder(s) in Slot {i + 1} have been checked.");
                    //ButtonInfo next = new("Check Next Slot");
                    //List<ButtonInfo> choices = new() { next };
                    ui.MakeChoiceButtonsAsync(new() { new("Check Next Slot") });
                    // ButtonInfo result = await IReceive.GetChoiceAsyncParams();
                    // await will just pause until choice made?
                    await IReceive.GetChoiceAsyncParams();
                }
            }
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageFour>().Begin();
        }

        public override void Undo()
        {
            if (isCompleted)
            {
                isCompleted = false;
                gs.completedActions--;
            }
            gm.LoadUndo(this);
            Begin();
        }
    }
}
