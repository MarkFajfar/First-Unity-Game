using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

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
            if (gs.ElderDisplay.Sum() - gs.ElderDisplay[0] < 1)
            {
                ui.displayText("No Elders to check. ");
                ui.addText("Press Next to continue.");
                ui.OnNextClick = actionComplete;
            }
            // do not need await because this is the last step in the method
            else elderCheck();
        }

        // do not use "async void"? https://stackoverflow.com/questions/70660049/how-do-i-avoid-async-void
        async Task elderCheck()
        {
            for (int i = 1; i < gs.ElderDisplay.Length; i++)
            {
                if (gs.ElderDisplay[i] > 0)
                {
                    ui.displayText($"There are {gs.ElderDisplay[i]} Elder(s) in Slot {i + 1}. Roll the die for each one. If a roll is {gs.ElderTarget[i]} or less, that Elder dies and is returned to the Out of Play box. Open the Status menu to remove any Elder that is lost. Press 'Check Next Slot' to continue when all Elder(s) in Slot {i + 1} have been checked.");
                    //ButtonInfo next = new("Check Next Slot");
                    //List<ButtonInfo> choices = new() { next };
                    ui.ShowChoiceButtonsAsync(new() { new("Check Next Slot") });
                    // ButtonInfo result = await IReceive.GetChoiceAsyncParams();
                    // await will just pause until choice made?
                    await ui.GetChoiceAsyncParams(); // IReceive.GetChoiceAsyncParams();
                }
            }
            ui.displayText("All Elders have been checked. ");
            ui.addText("Press Next to continue.");
            ui.OnNextClick = actionComplete;
        }

        protected override void actionComplete()
        {
            base.actionComplete();
            GetComponentInChildren<PassageEight>().Begin();
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
