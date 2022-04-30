using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class CardDrawUIScript : MonoBehaviour, IsUIScript
    {
        GameManager GameManager;
        GameState gs;

        Label headline;
        //Label message;
        Button confirm;
        //Button reDraw;
        //Button reopen;
        Button back;
        Button quit;
        
        TextField cardNumInput;

        //TouchScreenKeyboard keyboard;
        //string inputText;
        //TouchScreenKeyboard.Status status;
        // bool keyboardDone = false;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            GameManager = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            getVisualElements();
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");
            //message = root.Q<Label>("Message");

            cardNumInput = root.Q<TextField>("CardNumInput");

            confirm = root.Q<Button>("Confirm");
            confirm.clicked += confirmClicked;

            /*reDraw = root.Q<Button>("ReDraw");
            reDraw.clicked += reDrawClicked;

            reopen = root.Q<Button>("Reopen");
            reopen.clicked += reopenClicked;*/
            
            back = root.Q<Button>("Back");
            back.clicked += backClicked;

            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;

            TouchScreenKeyboard.hideInput = true;
        }

        void Start()
        {
            //openKeyboard();
        }

        /*void openKeyboard()
        {
            confirm.visible = false;
            reDraw.visible = false;
            reopen.visible = false;
            message.visible = true;
            message.text = "Enter Card Number";
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
            keyboard.characterLimit = 2;
                //, false, false, false, false, "Enter Card Number", 2);
            TouchScreenKeyboard.hideInput = true;
        }

        void Update()
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                headline.text = "Visible";
                if (keyboard.text == "")
                { message.text = "Enter Card Number"; }
                else
                { message.text = keyboard.text; }
            }
            else
            {
                if (keyboard.status == TouchScreenKeyboard.Status.Done)
                { closeKeyboard(); }
                else
                {
                    if (keyboard.status == TouchScreenKeyboard.Status.Canceled && confirm.visible != true)
                    { reopen.visible = true; }
                }
            }
        }*/

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
                    GameManager.CardNumInput();
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
            GameManager.PrevScene();
        }

        void quitClicked()
        {
            GameManager.ExitGame();
        }
    }
}
