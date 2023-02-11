using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavajoWars
{
    public abstract class LogicScript : MonoBehaviour
    {
        protected GameManager gm;
        protected GameState gs;
        public GameObject UIObject;
        public UIScript ui;

        public List<GameStep> steps;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            // ui = UIObject.GetComponent<UIScript>(); // in Inspector
            //steps = FindObjectsOfType<GameStep>().ToList();
        }

        public abstract void instructFromStep(GameStep caller, string instruction);

        public virtual void instructFromStep(GameStep caller, Action instruction)
        {
            instruction?.Invoke();
        }

        public abstract void SceneComplete();

        public abstract void initialUndoTarget();
    }

    public class InitialUndo : GameStep
    {
        public override string stepName { get => "InitialUndo"; }

        public override void Begin()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            logic.initialUndoTarget();
        }
    }
}
