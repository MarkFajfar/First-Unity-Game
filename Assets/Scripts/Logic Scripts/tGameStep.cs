using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public abstract class tGameStep : MonoBehaviour, IReceive
    {
        public virtual void Begin(string name)
        {
            print("Loading " + name);
            choiceIndex(100);
        }

        void Start()
        {
            print(this.name + "start");
        }

        void OnEnable()
        {
            print(this.name + "enable");   
        }

        public abstract void choiceIndex(int index);
        public abstract void choiceText(string text);
    }
}
