using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public class Card : ScriptableObject
    {
        public CardType ThisCardType;
        public int CardNumber;

        //Ceremony Cards
        public Person AddToPassage;
        public int BlessingWayRoll;
        public EnemyWay ThisCardEnemyWay;

        //Operations Cards
        public int[] Points;
        public Person ThisCardPerson;
        public MajorEvent ThisCardMajorEvent;
        public MinorEvent ThisCardMinorEvent;

        public void StepOne(Card currentCard)
        {
            GameManager gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            switch (currentCard.ThisCardType)
            {
                case CardType.Operations: 
                    Debug.Log("Check Points"); 
                    break;
                case CardType.Ceremony: 
                    Debug.Log("Remind Enemy Way for card " + currentCard.CardNumber.ToString("D2")); break;
                case CardType.Event: 
                    gm.EventCardTest(currentCard); 
                    break;
                default: Debug.Log("none"); break;
            };
        }
    }
}
