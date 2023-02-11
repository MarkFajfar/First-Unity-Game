using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public bool isCompleted = false;

        public Action clearCompleted;

        /*public event Action<List<bParams>> tCreateButtons;

        protected virtual void calltCB(List<bParams> choices) 
        { 
            tCreateButtons?.Invoke(choices);
        }*/

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            var LogicObj = GameObject.FindWithTag("Logic");
            logic = LogicObj.GetComponent<LogicScript>();
            var UIObj = GameObject.FindWithTag("UI");
            ui = UIObj.GetComponent<UIScript>();
            clearCompleted = () => isCompleted = false; 
        }

        void OnDisable()
        {
            clearCompleted = null;    
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

        protected virtual void actionComplete()
        {
            ui.OnNextClick -= actionComplete;
            isCompleted = true;
            gs.completedActions++;
            gs.stepStack.Push(this);
            //logic.instructFromStep(this, "ChooseAnotherAction");
        }
    }
}
