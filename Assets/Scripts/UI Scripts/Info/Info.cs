using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public abstract class Info
    {
        public object data = null;
        public GameStep gameStep = null;
        // public GameStep gameStep = UnityEngine.Object.FindObjectOfType<DefaultStep>();
        // assigning GameStep works only if one is instantiated
        public Action call = InvalidMessage;
        public GameStateObject gso = null;
        public bool waiting = false;
        public bool clearPanel = true;
        public bool closeFoldout = true;
        public bool remove = false; // if true will be deleted when selected

        public const string Default = "Default";
        public Family family = new() { Name = Default };
        public Person person = Person.Default;
        public Territory territory = null;
        public Resource resource = Resource.Default;
        public Cube cube = Cube.Default;
        public List<Cube> bowl = null; 
        public CardType cardType = CardType.Default;

        public GameObject parent = null;

        // experimental:
        public string parentName = "";
        // foldoutName never added in constructor, but only when button put into foldout
        public object parentData = null;

        public static void InvalidMessage()
        {
            UnityEngine.Object.FindObjectOfType<UIScript>().
                displayText("Invalid Choice Selected");
            Debug.LogError("Info field not defined.");
        }

        public static void InvalidMessage(Info info)
        {
            UnityEngine.Object.FindObjectOfType<UIScript>().
                displayText("Invalid Choice Selected");
            Debug.LogError($"Info field not defined for {info.GetType()}.");
        }
    }

    public class DefaultStep : GameStep
    {
        public override string stepName { get => "DefaultStep"; }

        public override void Begin()
        {
            FindObjectOfType<UIScript>().
                displayText("Invalid Choice Selected");
            Debug.LogError($"Info field not defined for {stepName}.");
        }

        public override void Undo()
        {
            Begin();
        }
    }
}
