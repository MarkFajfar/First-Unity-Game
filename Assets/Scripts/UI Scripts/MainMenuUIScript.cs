using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class MainMenuUIScript : MonoBehaviour, IsUIScript
    {
        GameManager gm;
        GameState gs;
        SetupLogic setup;

        public Label headline;
        VisualElement buttonPanel;
        VisualElement loadPanel;
        VisualElement hideTitle;
        Button loadSave;
        Button newGame;
        ScrollView scrollView;
        public Label viewSetup;
        public Button back;
        public Button next;
        Button confirm;
        Button loadBack;
        Button loadConfirm;
        public RadioButtonGroup locations;
        public RadioButtonGroup ferocities;
        Button quit;

        Scenario ChosenScenario;
        int stepSetup;

        void Awake()
        {
            //"The Awake function is called for each object in the scene at the time when the scene loads. All the Awakes will have finished before the first Start is called. Code in a Start function can make use of other initializations previously carried out in the Awake phase."
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            setup = gameObject.GetComponent<SetupLogic>();
        }

        void OnEnable()
        {
            //queries in OnEnable not Awake?
            getVisualElements();
        }

        void Start()
        {
            // called if Scene is reloaded
            print("Start Main Menu");
            gm.checkForSavedGame(this);
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");
            hideTitle = root.Q<VisualElement>("HideTitle");
            buttonPanel = root.Q<VisualElement>("ButtonPanel");

            scrollView = root.Q<ScrollView>("ScrollView");
            viewSetup = root.Q<Label>("viewSetup");
            back = root.Q<Button>("Back");
            next = root.Q<Button>("Next");
            confirm = root.Q<Button>("Confirm");
            loadBack = root.Q<Button>("LoadBack");
            loadConfirm = root.Q<Button>("LoadConfirm");

            locations = root.Q<RadioButtonGroup>("Locations");
            ferocities = root.Q<RadioButtonGroup>("Ferocities");

            loadPanel = root.Q<VisualElement>("LoadPanel");
            loadSave = root.Q<Button>("LoadSave");
            newGame = root.Q<Button>("NewGame");
            
            quit = root.Q<Button>("Quit");
            quit.visible = true;
            quit.clicked += quitClicked;
        }

        internal void showScenarios()
        {
            // called from GameManager if no saved game
            headline.text = "Choose a Scenario";
            buttonPanel.visible = true;            
            loadPanel.visible = false;
            scrollView.visible = false;
            viewSetup.visible = false;
            hideTitle.visible = false;
            initializeScenarioButtons();
        }

        void initializeScenarioButtons()
        {
            List<Button> buttons = buttonPanel.Query<Button>().ToList();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].RegisterCallback<ClickEvent>(buttonClicked);
            }
        }

        void buttonClicked(ClickEvent evt)
        {
            buttonPanel.visible = false;
            
            loadPanel.visible = false;
            loadBack.visible = false;
            loadConfirm.visible = false;

            hideTitle.visible = true;
            scrollView.visible = true;
            viewSetup.visible = true;
            back.visible = true;
            next.visible = true;
            confirm.visible = false;
            back.clicked += backClicked;
            next.clicked += nextClicked;
            confirm.clicked += confirmClicked;

            var clickedButton = evt.target as Button;

            headline.text = clickedButton.text;

            string chosenScenarioName = clickedButton.name;
            ChosenScenario = Resources.Load<Scenario>($"Scenarios/{chosenScenarioName}");

            stepSetup = 0;
            viewSetup.text = ChosenScenario.Setup[stepSetup];
        }

        void backClicked()
        {
            if (stepSetup > 0)
            {
                stepSetup--;
                viewSetup.text = ChosenScenario.Setup[stepSetup];
                if (confirm.visible) // if back from screen with confirm button
                {
                    confirm.visible = false;
                    next.visible = true;
                }
            }
            else
            {
                viewSetup.text = "";
                viewSetup.visible = false;
                hideTitle.visible = false;
                back.visible = false;
                next.visible = false;
                confirm.visible = false;
                buttonPanel.visible = true;
                headline.text = "Choose a Scenario";
            }
        }

        void nextClicked()
        {
            stepSetup++;

            if (ChosenScenario.Setup[stepSetup] != "END")
            {
                viewSetup.text = ChosenScenario.Setup[stepSetup];
            }
            else
            {
                viewSetup.text = "Setup is now complete. Click confirm to start the game.";
                next.visible = false;
                confirm.visible = true;
            }
        }

        void confirmClicked()
        {
            //deInitializeButtons();
            //show setup selection buttons
            //new back and confirm functions
            confirm.visible = false;
            next.visible = true;
            back.clicked -= backClicked;
            next.clicked -= nextClicked;
            back.clicked += setup.backFamily;
            next.clicked += setup.nextFamily;
            setup.SetupNewGame(ChosenScenario); 
        }       

        void deInitializeButtons() // what happens to buttons when scene changes?
        {
            back.clicked -= backClicked;
            next.clicked -= nextClicked;
            confirm.clicked -= confirmClicked;
            //quit.clicked -= quitClicked;
            back.visible = false;
            next.visible = false;
            confirm.visible = false;
            viewSetup.visible = false;
            scrollView.visible = false;
            hideTitle.visible = false;
        }

        internal void showLoadPanel()
        {
            // called from GameManager if saved game
            headline.text = "Saved Game Found";
            buttonPanel.visible = false;
            loadPanel.visible = true;
            viewSetup.visible = false;
            hideTitle.visible = false;
            initializeLoadButtons();
        }
        void initializeLoadButtons()
        {
            Button[] buttons = { loadSave, newGame, loadBack, loadConfirm };
            foreach (Button button in buttons)
            {
                button.RegisterCallback<ClickEvent>(loadFunction);
            }
        }

        void loadFunction(ClickEvent evt)
        {
            loadPanel.visible = false;
            viewSetup.visible = true;
            hideTitle.visible = true;
            var clickedButton = evt.target as Button;
            string action = clickedButton.name;
            switch (action)
            {
                case "LoadSave":
                    viewSetup.text = "Loading Saved Game";
                    gm.LoadSave();
                    break;
                case "NewGame":
                    viewSetup.text = "This will delete the saved game. Press Confirm to continue.";
                    loadBack.visible = true;
                    loadConfirm.visible = true;
                    break;
                case "LoadBack":
                    loadBack.visible = false;
                    loadConfirm.visible = false;
                    showLoadPanel();
                    break;
                case "LoadConfirm":
                    ;
                    loadBack.visible = false;
                    loadConfirm.visible = false;
                    viewSetup.visible = false;
                    gm.DeleteSaveAndStartNew();
                    break;
                default:
                    loadBack.visible = false;
                    loadConfirm.visible = false;
                    showLoadPanel();
                    break;
            };
        }

        void quitClicked()
        {
            // no save in MainMenu because game not yet initialized
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit(); // original code to quit Unity player
#endif
        }
    }
}
