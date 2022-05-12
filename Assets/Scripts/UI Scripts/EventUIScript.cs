using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;

namespace NavajoWars
{
    public class EventUIScript : MonoBehaviour, IsUIScript, IMethodReceiver
    {
        GameManager gm;
        GameState gs;

        Label headline;
        Button back;
        Button quit;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
        }

        void OnEnable()
        {
            getVisualElements();
        }

        void Start()
        {
            print("Start Event");
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");

            back = root.Q<Button>("Back");
            back.clicked += backClicked;

            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;
        }

        void backClicked()
        {
            gm.PrevScene();
        }

        void quitClicked()
        {
            gm.ExitGame();
        }
    }
}
