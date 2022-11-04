using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace NavajoWars
{
    public interface IReceive
    {

        void methodManager(string methodText)
        {
            // remove spaces just to be sure
            methodText = methodText.Replace(" ", "");
            Debug.Log("received method: " + methodText);
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(methodText);
            //must be a public method, to be called from another script
            chosenMethod?.Invoke(this, null);
        }

        async static Task<(int choiceIndex, string choiceText)> GetChoiceAsync(List<string> choices)
        {
            ChoiceUIScript choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
            ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<(int choiceIndex, string choiceText)>();
            Debug.Log("New Task ID: " + result.Task.Id.ToString());
            choiceEventHandler = (s, e) =>
            {
                Debug.Log("received " + e.ChoiceText);
                result.SetResult((e.ChoiceIndex, e.ChoiceText));
            };
            choice.ChoiceMadeEvent += choiceEventHandler;
            choice.DisplayChoiceButtonsEvent(choices);
            Debug.Log("Task Status Before: " + result.Task.Status.ToString());
            await result.Task;
            choice.ChoiceMadeEvent -= choiceEventHandler;
            Debug.Log("Task Status After: " + result.Task.Status.ToString());
            return (result.Task.Result.choiceIndex, result.Task.Result.choiceText);
        }
    }
}