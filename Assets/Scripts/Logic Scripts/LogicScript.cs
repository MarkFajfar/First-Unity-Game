using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public abstract class LogicScript : MonoBehaviour
    {
        protected GameManager gm;
        protected GameState gs;
        public GameObject UIObject;
        protected UIScript ui;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            ui = UIObject.GetComponent<UIScript>();
        }

        public abstract void instructFromStep(GameStep caller, string instruction);
        
        public abstract void setUndo(GameStep caller, GameStep target);

        public abstract void setUndo(GameStep target);

        public abstract void callUndo(GameStep caller, GameStep target);

        public abstract void callUndo(GameStep caller);

        public abstract void callUndo();

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
