using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public abstract class GameStep : MonoBehaviour
    {
        protected GameManager gm;
        protected GameState gs;
        protected UIScript ui;
        protected LogicScript logic;

        public abstract string stepName { get; }

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            var LogicObj = GameObject.FindWithTag("Logic");
            logic = LogicObj.GetComponent<LogicScript>();
            var UIObj = GameObject.FindWithTag("UI");
            ui = UIObj.GetComponent<UIScript>();
        }

        /*public virtual void Begin(GameStep caller)
        {
            print($"Loading {this.name} from {caller.name}");
            gs.currentOpsStep = this;
        }*/

        public abstract void Begin();

        /*public virtual void Undo(GameStep caller)
        {
            //gs.currentOpsStep = this;
        }*/

        public abstract void Undo();
    }
}
