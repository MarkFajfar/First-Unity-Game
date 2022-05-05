using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class CardDrawUIScript : MonoBehaviour, IsUIScript
    {
        GameManager gm;
        GameState gs;

        Label headline;
        Button confirm;
        Button back;
        Button quit;
        
        TextField cardNumInput;

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
            print("Start Card Draw");
            headline.visible = true;
            cardNumInput.visible = true;
            confirm.visible = true;
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");

            cardNumInput = root.Q<TextField>("CardNumInput");

            confirm = root.Q<Button>("Confirm");
            confirm.clicked += confirmClicked;
            
            back = root.Q<Button>("Back");
            back.clicked += backClicked;

            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;

            TouchScreenKeyboard.hideInput = true;
        }
       
        void confirmClicked()
        { 
            int num = int.Parse(cardNumInput.text);
            if (num != 0 && num < 56)
            {                
                if (gs.PlayedCards.Contains(num))
                {
                    headline.text = "That Card Has Already Been Played";
                }
                else
                {
                    print($"Card Number {num} Selected");
                    gs.CurrentCardNum = num;
                    gm.CardNumInput();   // "I'm finished; this is what happened"
                    // gs.PlayedCards.Add(num); //add to played cards when completed
                }
            }
            else
            {
                headline.text = "Number Not Valid";
            }
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
