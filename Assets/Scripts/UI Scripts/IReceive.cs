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

        async static Task<string> testGetChoiceAsync() 
        {
            Debug.Log("starting task");
            ChoiceMadeString.ChoiceMadeStringEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<string>();
            Debug.Log(result.Task.Status);
            choiceEventHandler = (s, e) =>
            {
                Debug.Log("event triggered");
                result.SetResult(e.ChoiceText);
            };
            ChoiceMadeString.ChoiceMadeStringEvent += choiceEventHandler;
            await result.Task;
            ChoiceMadeString.ChoiceMadeStringEvent -= choiceEventHandler;
            return result.Task.Result;
        }


        async static Task<ButtonInfo> GetChoiceAsyncParams()
        {
            ChoiceMadeParams.unsubChoiceMadeParams();
            
            ChoiceMadeParams.ChoiceMadeParamsEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<ButtonInfo>();
            choiceEventHandler = (s, e) =>
            {
                result.SetResult(e.cParams);
            };
            ChoiceMadeParams.ChoiceMadeParamsEvent += choiceEventHandler;
            await result.Task;
            ChoiceMadeParams.ChoiceMadeParamsEvent -= choiceEventHandler;
            return result.Task.Result;
        }

        async static Task<GameStep> GetChoiceAsyncGameStep()
        {
            ChoiceMadeGameStep.unsubChoiceMadeGameStep();

            ChoiceMadeGameStep.ChoiceMadeGameStepEventHandler choiceEventHandler = null;
            var result = new TaskCompletionSource<GameStep>();
            choiceEventHandler = (s, e) =>
            {
                result.SetResult(e.cGameStep);
            };
            ChoiceMadeGameStep.ChoiceMadeGameStepEvent += choiceEventHandler;
            await result.Task;
            ChoiceMadeGameStep.ChoiceMadeGameStepEvent -= choiceEventHandler;
            return result.Task.Result;
        }
    }
}