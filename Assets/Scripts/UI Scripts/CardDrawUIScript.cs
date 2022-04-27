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

        public Label headline;
        Button confirm;
        Button quit;
        TextField cardNumInput;

        string inputText;
        TouchScreenKeyboard keyboard;
        //TouchScreenKeyboard.Status status;
        // bool keyboardDone = false;

        void Awake()
        {
            GameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            getVisualElements();
        }

        void Start()
        {
            openKeyboard();
        }

        void openKeyboard()
        {
            keyboard = TouchScreenKeyboard.Open(inputText, TouchScreenKeyboardType.NumberPad, false, false, false, true);
            keyboard.characterLimit = 2;
            //TouchScreenKeyboard.hideInput = true;
        }

        void Update()
        {
            if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done) 
            { 
                confirmClicked(); //headline.text = "Card is " + inputText; 
            }
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            headline = root.Q<Label>("Headline");

            cardNumInput = root.Q<TextField>("CardNumInput");
            cardNumInput.visible = false;


            confirm = root.Q<Button>("Confirm");
            confirm.visible = false;
            confirm.clicked += confirmClicked;

            quit = root.Q<Button>("Quit");
            quit.visible = true;
            quit.clicked += quitClicked;
        }
        
        internal void showKeyboard()
        {
            print("keyboard active");
        }

        void confirmClicked()
        {
            //try
            //{
                int num = int.Parse(inputText);
                byte number = byte.Parse(inputText);//Convert.ToByte(inputText); //(cardNumInput.text);
                if(num != 0 && num < 56)
                { 
                    GameManager.CardNumInput(num);
                    //cardNumInput.visible = false;
                    //confirm.visible = false;
                    headline.text = "Card is " + inputText; //cardNumInput.text;
                }
                else
                { 
                    headline.text = "Number Not Valid";
                    openKeyboard();
                }
            /*}
            catch (FormatException)
            {
                headline.text = "Number Not Valid";
                openKeyboard();
            }
            catch (OverflowException)
            {
                headline.text = "Number Not Valid";
                openKeyboard();
            }*/
        }

        void quitClicked()
        {
            GameManager.ExitGame();
        }
    }
}
