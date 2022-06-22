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

        public List<Card> CeremonyCardsInHand;
        public List<Card> EventCardsInPlay;

        public List<Person> PersonsInPassage;

        public int[] ElderDisplay;
        public readonly int[] ElderTarget = { 0, 1, 2, 2, 3, 4, 5 };

        public int AP
        { get => ap; set => ap = Math.Clamp(value, 0, 19); }
        [SerializeField] int ap;
        public int CP
        { get => cp; set => cp = Math.Clamp(value, 0, 19); }
        [SerializeField] int cp;
        public int MP
        { get => mp; set => mp = Math.Clamp(value, 0, 19); }
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

        // Game has 9 TradeGoods tokens, and 2 more from intruders; each scenario starts with 3
        public int TradeGoodsHeld
        { get => tradeGoodsHeld; set => tradeGoodsHeld = Math.Clamp(value, 0, 11); }
        [SerializeField] int tradeGoodsHeld;
        public int TradeGoodsMax // is this the same in every scenario?
        { get => tradeGoodsMax; set => tradeGoodsMax = Math.Clamp(value, 0, 11); }
        [SerializeField] int tradeGoodsMax;

        // Game has 8 sheep tokens; each scenario starts with 3
        public int SheepHeld
        { get => sheepHeld; set => sheepHeld = Math.Clamp(value, 0, 8); }
        [SerializeField] int sheepHeld;
        public int SheepMax 
        { get => sheepMax; set => sheepMax = Math.Clamp(value, 0, 8); }
        [SerializeField] int sheepMax;

        // Game has 8 horses tokens; each scenario starts with 3
        public int HorsesHeld
        { get => horsesHeld; set => horsesHeld = Math.Clamp(value, 0, 8); }
        [SerializeField] int horsesHeld;
        public int HorsesMax 
        { get => horsesMax; set => horsesMax = Math.Clamp(value, 0, 8); }
        [SerializeField] int horsesMax;

        // Game has 1 Firearms token, and 4 more from intruders
        public int Firearms
        { get => firearms; set => firearms = Math.Clamp(value, 0, 5); }
        [SerializeField] int firearms;

        public List<Territory> HasDrought;
        public List<Territory> HasCorn;
        public List<Territory> HasMission;
        public List<Territory> HasRancho;
        public List<Territory> HasFort;

        [Serializable]
        public struct raided
        {
            public int Black
            { get => black; set => black = value; }
            [SerializeField] int black;
            public int White
            { get => white; set => white = value; }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = value; }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = value; }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = value; }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = value; }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = value; }
            [SerializeField] int blue;
        }

        [Serializable]
        public struct recovery
        {
            public int Black
            { get => black; set => black = value; }
            [SerializeField] int black;
            public int White
            { get => white; set => white = value; }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = value; }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = value; }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = value; }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = value; }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = value; }
            [SerializeField] int blue;
        }

        [Serializable]
        public struct subjugation
        {
            public int Black
            { get => black; set => black = value; }
            [SerializeField] int black;
            public int White
            { get => white; set => white = value; }
            [SerializeField] int white;
            public int Brown
            { get => brown; set => brown = value; }
            [SerializeField] int brown;
            public int Yellow
            { get => yellow; set => yellow = value; }
            [SerializeField] int yellow;
            public int Red
            { get => red; set => red = value; }
            [SerializeField] int red;
            public int Green
            { get => green; set => green = value; }
            [SerializeField] int green;
            public int Blue
            { get => blue; set => blue = value; }
            [SerializeField] int blue;
        }

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

            int[] evReference = { 2, 1, 0, -1 };
            public int Evasion
            { get { return evReference[Ferocity]; } }
            //public int Evasion { get => evasion; set => evasion = value; }
            //[SerializeField] int evasion = 0;
        }

        [HideInInspector] public Family FamilyA;
        [HideInInspector] public Family FamilyB;
        [HideInInspector] public Family FamilyC;
        [HideInInspector] public Family FamilyD;
        [HideInInspector] public Family FamilyE;
        [HideInInspector] public Family FamilyF;

        public raided Raided;
        public recovery Recovery;
        public subjugation Subjugation;

        public List<Family> Families;
    }
}
 
