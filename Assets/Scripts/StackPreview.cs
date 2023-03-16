using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NavajoWars
{
    /*[CustomEditor(typeof(GameManager)), CanEditMultipleObjects]
    public class StackPreview : Editor
    {
        GameManager gm;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
        }

        *//*public override void OnInspectorGUI()
        {

            // get the target script and get the stack from it
            //var ts = (GameManager)target;
            //Stack<GameStep> stack = ts.stepStack;

            // some styling for the header, this is optional
            var bold = new GUIStyle();
            bold.fontStyle = FontStyle.Bold;
            bold.normal.textColor = Color.white;


            GUILayout.Label("GameStep Stack:", bold);
            if (gm.stepStack == null)
            {
                GUILayout.Label("No stack initialized", bold);
            }
            else
            {
                GUILayout.Label($"Stack count: {gm.stepStack.Count}", bold);
            }

            // add a label for each item, you can add more properties
            // you can even access components inside each item and display them
            // for example if every item had a sprite we could easily show it 
            if (gm.stepStack != null && gm.stepStack.Count > 0) 
            {
                GUILayout.Label($"Top of stack:  {gm.stepStack.Peek().stepName}", bold);
                *//*foreach (GameStep step in stack)
                {
                    GUILayout.Label(step.stepName, bold);
                }*//*
            }
        }*//*
    }*/
}
