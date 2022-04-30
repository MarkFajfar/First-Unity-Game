using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavajoWars
{
    public enum Territory { SantaFe, Splitrock, SanJuan, Zuni, Monument, Hopi, BlackMesa, Canyon }
    public enum Person { Man, Woman, Child, Elder }
    public enum CardType { Operations, Ceremony, Event }

    [Serializable]
    public class GameState : MonoBehaviour
    {
        //public string chosenScenarioName;
        public Scenario ChosenScenario
        { get => chosenScenario; set => chosenScenario = value; }
        [SerializeField] Scenario chosenScenario;
        
        public string CurrentSceneName
        { get => currentSceneName; set => currentSceneName = value; }
        [SerializeField] string currentSceneName;
        public string PriorSceneName
        { get => priorSceneName; set => priorSceneName = value; }
        [SerializeField] string priorSceneName;

        public int CurrentCardNum
        { get => currentCardNum; set => currentCardNum = value; }
        [SerializeField] int currentCardNum;

        public List<int> PlayedCards = new();
 
        public Card CurrentCard
        { get => currentCard; set => currentCard = value; }
        [SerializeField] Card currentCard;

        //public List<Card> CeremonyCardsInHand = new();
        //public List<Card> EventCardsInPlay = new();

        public int AP
        { get => ap; set => ap = value; }
        [SerializeField] int ap; 
        public int CP
        { get => cp; set => cp = value; }
        [SerializeField] int cp;
        public int MP
        { get => mp; set => mp = value; }
        [SerializeField] int mp;
        public int Morale
        { get => morale; set => morale = value; }
        [SerializeField] int morale;
        public int Ferocity
        { get => ferocity; set => ferocity = value; }
        [SerializeField] int ferocity;
        public int EnemyAP
        { get => enemyAP; set => enemyAP = value; }
        [SerializeField] int enemyAP;
        public int EnemyRaid
        { get => enemyRaid; set => enemyRaid = value; }
        [SerializeField] int enemyRaid;
        public int TradeGoodsHeld
        { get => tradeGoodsHeld; set => tradeGoodsHeld = value; }
        [SerializeField] int tradeGoodsHeld;
        public int Firearms
        { get => firearms; set => firearms = value; }
        [SerializeField] int firearms;

        //cubes?

        [Serializable]
        public class Family
        {
            public string Name { get => name; set => name = value; }
            [SerializeField] string name;
            public bool IsActive { get => isActive; set => isActive = value; }
            [SerializeField] bool isActive = false;
            public bool HasMan { get => hasMan; set => hasMan = value; }
            [SerializeField] bool hasMan = true;
            public bool HasWoman { get => hasWoman; set => hasWoman = value; }
            [SerializeField] bool hasWoman = true;
            public bool HasChild { get => hasChild; set => hasChild = value; }
            [SerializeField] bool hasChild = true;
            public bool HasHorse { get => hasHorse; set => hasHorse = value; }
            [SerializeField] bool hasHorse = false;
            public Territory IsWhere { get => isWhere; set => isWhere = value; }
            [SerializeField] Territory isWhere = Territory.Splitrock;
            public int Ferocity { get => ferocity; set => ferocity = value; }
            [SerializeField] int ferocity = 0;
        }

        public Family FamilyA = new() { Name = "A" };
        public Family FamilyB = new() { Name = "B" };
        public Family FamilyC = new() { Name = "C" };
        public Family FamilyD = new() { Name = "D" };
        public Family FamilyE = new() { Name = "E" };
        public Family FamilyF = new() { Name = "F" };

        public List<Family> Families = new();
       
        // create list of families in Awake?
        //{ FamilyA, FamilyB, FamilyC, FamilyD, FamilyE, FamilyF };
    }
}
 
