using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class GameStateObject
    {
        public GameStateTag tag;
        public VisualElement ve = null;
        public int setInt;
        public bool setBool;
        public Person p = Person.Default;
        public Family f = null;
        public Territory t = null;
        public Resource r = Resource.Default;
        public Cube cube = Cube.Default;
        public List<Cube> bowl = null;
        public VisualElement parent = null;

        public GameStateObject() { }

        public GameStateObject(GameStateTag tag)
        {
            this.tag = tag;
        }
    }
}