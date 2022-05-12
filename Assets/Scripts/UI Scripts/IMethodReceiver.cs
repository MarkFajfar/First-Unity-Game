using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace NavajoWars
{
    public interface IMethodReceiver
    {
        void methodManager(string methodText)
        {
            // remove spaces just to be sure
            methodText = methodText.Replace(" ", "");
            //Debug.Log("received method: " + methodText);
            Type thisType = GetType();
            MethodInfo chosenMethod = thisType.GetMethod(methodText);
            //must be a public method, to be called from another script
            chosenMethod?.Invoke(this, null);
        }
    }
}