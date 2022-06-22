using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class OperationsUIScript : MonoBehaviour, IsUIScript, IReceive
    {
        GameManager gm;
        GameState gs;
        ChoiceUIScript choice;
        ChoiceUIScript.ChoiceMadeEventHandler choiceEventHandler = null;

        public Label headline;
        public Label message;
        Button prev;
        Button quit;
        public Button back;
        public Button next;

        public delegate void ClickNext();
        public event ClickNext OnOpsNextClick;
        public delegate void ClickBack();
        public event ClickBack OnOpsBackClick;

        bool isPlayerOpsDone;
        bool isEnemyOpsDone;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
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
            // called when scene "Operations" is loaded, called from gm  
            // reset UI for reload
            headline.visible = true;
            message.visible = false;
            back.visible = true;
            next.visible = true; 
            prev.visible = false;
            quit.visible = true;

            questionPreempt();
        }

        void questionPreempt()
        {
            unsubBack();
            OnOpsBackClick += prevClicked;
            if (gs.AP >= gs.CurrentCard.Points[0])
            {
                headline.text = $"Preempt for {gs.CurrentCard.Points[0]} AP?";
                choice.DisplayChoices(this, new List<string> { "Yes Preempt", "Do Not Preempt" });
                // with "this" could use "Yes" or "No" if not used elsewhere in this script
            }
            else
            {
                headline.text = "Insufficient AP to Preempt. Starting Enemy Action Phase.";
                EnemyOperation(); 
            }
        }

        public void clickedYesPreempt()
        {
            gs.AP -= gs.CurrentCard.Points[0];
            headline.text = $"Subtracted {gs.CurrentCard.Points[0]} AP\n";
            unsubBack();
            OnOpsBackClick += backYesPreempt;
            PlayerOperation();
        }

        void backYesPreempt()
        {
            print("backYesPreempt");
            gs.AP += gs.CurrentCard.Points[0];
            //headline.text = $"Adding back {gs.CurrentCard.Points[0]} AP\n";
            questionPreempt(); 
            // only way to get here is through questionPreempt; headline text replaced there
        }

        public void PlayerOperation()
        {
            headline.text += "Select a Player Operation";
            choice.DisplayChoices(new List<string> { "Planning", "Take Actions", "Passage of Time" });
            unsubBack();
            // cannot use "this" because response back to different scripts
        }

        public void clickedDoNotPreempt()
        {
            unsubBack();
            OnOpsBackClick += questionPreempt; 
            EnemyOperation();
        }

        void EnemyOperation()
        {
            headline.text = "Enemy Ops";
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

        public void PlayerOpsDone()
        {
            print("Finished with Player Operations"); 
            OnOpsNextClick -= PlayerOpsDone;
            isPlayerOpsDone = true;
            if (isEnemyOpsDone) OpsDone();
            else EnemyOperation();
            //ADD BACK FUNCTION??
        }

        public void EnemyOpsDone()
        {
            print("Finished with Player Operations");
            OnOpsNextClick -= EnemyOpsDone;
            isEnemyOpsDone = true;
            if (isPlayerOpsDone) OpsDone();
            else PlayerOperation();
            //ADD BACK FUNCTION??
        }

        public void OpsDone()
        {
            print("Finished with Operations Card");
            gm.LoadNewScene("CardDraw");
            //ADD BACK FUNCTION??
        }

        public void unsubBack()
        {
            if (OnOpsBackClick != null)
            {
                foreach (ClickBack subscriber in OnOpsBackClick.GetInvocationList())
                {
                    OnOpsBackClick -= subscriber;
                }
            }
        }
    }
}
