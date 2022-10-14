using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class PassageUIScript : MonoBehaviour, IsUIScript, IReceive
    {
        GameManager gm;
        GameState gs;
        ChoiceUIScript choice;
        ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;

        public Label headline;
        public Label message;
        public Button prev;
        public Button quit;
        public Button back;
        public Button next;

        public ScrollView scrollView;
        public List<Foldout> foldouts;

        public delegate void ClickNext();
        public event ClickNext OnOpsNextClick;
        public delegate void ClickBack();
        public event ClickBack OnOpsBackClick;

        bool isPassageDone;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            //choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
        }

        void OnEnable()
        {
            getVisualElements();
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");

            message = root.Q<Label>("Message");

            back = root.Q<Button>("Back");
            back.clicked += backClicked;

            next = root.Q<Button>("Next");
            next.clicked += nextClicked;

            prev = root.Q<Button>("Prev");
            prev.clicked += prevClicked;

            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;

            scrollView = root.Q<ScrollView>("ScrollView");
            foldouts = scrollView.Query<Foldout>().ToList();
        }

        public void nextClicked()
        { OnOpsNextClick?.Invoke(); }

        public void backClicked()
        { OnOpsBackClick?.Invoke(); }

        void prevClicked()
        { gm.PrevScene(); }

        void quitClicked()
        { gm.ExitGame(); }

        void Start()
        {
            // called when scene "Passage" is loaded, called from gm  
            // reset UI for reload
            headline.text = "Passage of Time";
            headline.visible = true;
            message.visible = false;
            scrollView.visible = false;
            back.visible = true;
            next.visible = true;
            prev.visible = false;
            quit.visible = true;
        }



        public void Initialize()
        {
            quit.visible = false;
            prev.visible = false;
            back.visible = true;
            next.visible = true;
            message.visible = true;
        }

        public void hideBackNext()
        {
            back.visible = false;
            next.visible = false;
        }

        public void showBackNext()
        {
            back.visible = true;
            next.visible = true;
        }
    }
}
