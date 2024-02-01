using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class GameStateFunctionObject
    {
        public delegate void Delegate(GameStateFunctionObject obj);
        public delegate int DelegateInt();
        public delegate bool DelegateBool(); 
        public GameStateFunction tag;
        public Delegate callback; // not necessary
        public VisualElement ve = null;
        public DelegateInt getValue = null; // not necessary
        public int setValue;
        public DelegateBool getBool = null; // not necessary
        public bool setBool;
        public Person p = Person.Default;
        public Family f = null;
        public Territory t = Territory.Default;
        public List<Cube> bowl = null;
        // public DelegateTerritory getT = null; // necessary?
        // public DelegateListTerritory getListT = null; 
        public VisualElement parent = null;

        public GameStateFunctionObject() { }

        public GameStateFunctionObject(GameStateFunction tag)
        {
            this.tag = tag;
        }
    }
}