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

        /*async static Task<(int choiceIndex, string choiceText)> GetChoiceAsync(List<string> choices)
        {
            ChoiceUIScript choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
            ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<(int choiceIndex, string choiceText)>();
            choiceEventHandler = (s, e) =>
            {
                result.SetResult((e.ChoiceIndex, e.ChoiceText));
            };
            choice.ChoiceMadeEvent += choiceEventHandler;
            choice.DisplayChoiceButtonsEvent(choices);
            await result.Task;
            choice.ChoiceMadeEvent -= choiceEventHandler;
            return (result.Task.Result.choiceIndex, result.Task.Result.choiceText);
        }*/

        async static Task<(int choiceIndex, string choiceText)> GetChoiceAsync(List<bParams> choices)
        {
            //UIScript ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIScript>();
            UIScript.ChoiceMadeEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<(int choiceIndex, string choiceText)>();
            choiceEventHandler = (s, e) =>
            {
                result.SetResult((e.ChoiceIndex, e.ChoiceText));
            };
            UIScript.ChoiceMadeEvent += choiceEventHandler;
            //ui.DisplayChoiceButtonsEvent(choices);
            await result.Task;
            UIScript.ChoiceMadeEvent -= choiceEventHandler;
            return (result.Task.Result.choiceIndex, result.Task.Result.choiceText);
        }

        async static Task<GameStep> GetChoiceAsyncObject(List<bParams> choices)
        {
            //UIScript ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIScript>();
            UIScript.ChoiceMadeObjectEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<GameStep>();
            choiceEventHandler = (s, e) =>
            {
                result.SetResult(e.cGameStep);
            };
            UIScript.ChoiceMadeObjectEvent += choiceEventHandler;
            //ui.DisplayChoiceButtonsEvent(choices);
            await result.Task;
            UIScript.ChoiceMadeObjectEvent -= choiceEventHandler;
            return result.Task.Result;
        }
    }
}