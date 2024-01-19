using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NavajoWars
{
    public enum Territory { SantaFe, Splitrock, SanJuan, Zuni, Monument, Hopi, BlackMesa, Canyon, Default }
    public enum Person { Man, Woman, Child, Elder, Default }
    public enum Resource { Horse, Sheep, Corn, TradeGood, Firearm, Default }
    public enum Cube { Black, White, Brown, Red, Yellow, Green, Blue, Default }
    public enum CardType { Operations, Ceremony, Event, Default }

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

        public List<Resource> AnimalsInPassage;

        public List<Resource> Resources;

        public int[] ElderDisplay;
        public readonly int[] ElderTarget = { 0, 1, 2, 2, 3, 4, 5 };

        public int Population
        {
            get
            {
                int population = ElderDisplay.Sum();
                foreach (var f in Families)
                {
                    if (f.HasMan) population++;
                    if (f.HasWoman) population++;
                    if (f.HasChild) population++;
                }
                return population;
            }
        }

        public int Arability
        {
            get
            {
                int a = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (TerritoryFamily[i]) 
                    { 
                        a = a + 3 - TerritoryDrought[i];
                        if (HasRancho.Contains((Territory)i)) a--;
                    }
                }
                return a;
            }
        }

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
        public int EnemyFerocity
        { get => enemyFerocity; set => enemyFerocity = value; }
        [SerializeField] int enemyFerocity;
        public int EnemyAP
        { get => enemyAP; set => enemyAP = value; }
        [SerializeField] int enemyAP;
        public int EnemyRaid
        { get => enemyRaid; set => enemyRaid = value; }
        [SerializeField] int enemyRaid;

        /// <summary>
        /// sets number of Resource enum in Resources to value
        /// </summary>
        void setResource(int target, Resource resource)
        {
            // int target = Math.Clamp(value, 0, 11);
            int current = Resources.Where(r => r == resource).Count();
            while (target != current)
            {
                if (target > current) Resources.Add(resource);
                if (target < current) Resources.Remove(resource);
                current = Resources.Where(r => r == resource).Count();
            }
        }

        // Game has 9 TradeGoods tokens, and 2 more from intruders; each scenario starts with 3
        public int TradeGoodsHeld
        { 
            get { return Resources.Where(r => r == Resource.TradeGood).Count(); }
            set { setResource(Math.Clamp(value, 0, TradeGoodsMax), Resource.TradeGood);                
                /* int target = Math.Clamp(value, 0, 11);
                int current = Resources.Where(r => r == Resource.TradeGood).Count();
                while (target != current)
                {
                    if (target > current) Resources.Add(Resource.TradeGood);
                    if (target < current) Resources.Remove(Resource.TradeGood);
                    current = Resources.Where(r => r == Resource.TradeGood).Count();
                } */
            }
        }
        //[SerializeField] int tradeGoodsHeld;
        //Resources list is serialized, not number held

        public int TradeGoodsMax 
        { get => tradeGoodsMax; set => tradeGoodsMax = Math.Clamp(value, 0, 11); }
        [SerializeField] int tradeGoodsMax;
        // is tgMax the same in every scenario?

        // Game has 8 sheep tokens; each scenario starts with 3
        /// <summary>
        /// Returns number of Sheep in Resources box
        /// </summary> 
        public int SheepHeld
        { get => Resources.Where(r => r == Resource.Sheep).Count();
        set => setResource(Math.Clamp(value, 0, SheepMax), Resource.Sheep); }
        //{ get => sheepHeld; set => sheepHeld = Math.Clamp(value, 0, 8); }
        //[SerializeField] int sheepHeld;
        public int SheepMax 
        { get => sheepMax; set => sheepMax = Math.Clamp(value, 0, 8); }
        [SerializeField] int sheepMax;

        // Game has 8 horses tokens; each scenario starts with 3
        /// <summary>
        /// Returns number of Horses in Resources box
        /// </summary>         
        public int HorsesHeld
        {
            get => Resources.Where(r => r == Resource.Horse).Count() +
                Families.Where(f => f.HasHorse).Count();
            set 
            { 
                int target = Math.Clamp(value, 0, Math.Max(0, HorsesMax - Families.Where(f => f.HasHorse).Count()));
                setResource(target, Resource.Horse);                
            }
        }
        //{ get => horsesHeld; set => horsesHeld = Math.Clamp(value, 0, 8); }
        //[SerializeField] int horsesHeld;
        public int HorsesMax 
        { get => horsesMax; set => horsesMax = Math.Clamp(value, 0, 8); }
        [SerializeField] int horsesMax;

        // Game has 1 Firearms token, and 4 more from intruders
        public int Firearms
        { get => Resources.Where(r => r == Resource.Firearm).Count();
          set => setResource(Math.Clamp(value, 0, 5), Resource.Firearm); }
        //{ get => firearms; set => firearms = Math.Clamp(value, 0, 5); }
        //[SerializeField] int firearms;

        // number of corn tokens moved to resource box in Passage Step 4; may not be necessary
        public int CornHarvested
        { get => cornHarvested; set => cornHarvested = value; }
        [SerializeField] int cornHarvested;

        public List<Territory> HasDrought;
        public int[] TerritoryDrought;

        /// <summary>
        /// Array of 9 bool, true if Family in that Territory
        /// </summary>
        public bool[] TerritoryFamily
        { 
            get
            {
                bool[] refresh = new bool[9];
                for (int i = 0; i < 9; i++)
                {
                    if (Families.Where(f => f.IsWhere == (Territory)i).Count() > 0) refresh[i] = true;
                }
                return refresh;
            }
        }

        /// <summary>
        /// Returns number of Territories with a Family
        /// </summary>
        public int TerritoriesWithFamily
        {
            get => TerritoryFamily.Where(b => b == true).Count();
        }
        public List<Territory> HasCorn;
        public List<Territory> HasMission;
        public List<Territory> HasRancho;
        public List<Territory> HasFort;

        public List<Cube> Raided;
        public List<Cube> Recovery;
        public List<Cube> Subjugation;

        /*public Family FamilyA;
        public Family FamilyB;
        public Family FamilyC;
        public Family FamilyD;
        public Family FamilyE;
        public Family FamilyF;
        [HideInInspector] public Family DefaultFamily;*/

        public List<Family> AllFamilies;
        [NonSerialized] [HideInInspector] public List<Family> Families;
                
        // variables used in Operations script
        //public Family selectedFamily;
        //public List<Family> completedFamilies;
        //public List<GameStep> completedActions;
        public int completedFamilies;
        public int completedActions;
        
        public bool canBackToDraw
        { get => canbackToDraw; set => canbackToDraw = value; }
        [SerializeField] bool canbackToDraw;

        public bool isPreempt
        { get => ispreempt; set => ispreempt = value; }
        [SerializeField] bool ispreempt;

        public bool isPlayerOpsDone
        { get => isplayerOpsDone; set => isplayerOpsDone = value; }
        [SerializeField] bool isplayerOpsDone;
    
        public bool isEnemyOpsDone
        { get => isenemyOpsDone; set => isenemyOpsDone = value; }
        [SerializeField] bool isenemyOpsDone;
    }
}
 
