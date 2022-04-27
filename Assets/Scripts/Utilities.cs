using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NavajoWars
{
    public class Utilities : MonoBehaviour
    {
        GameManager GameManager;
        GameState gs;

        void Awake()
        {
            GameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            gs = GameManager.GetComponent<GameState>();
            //CreateSO();
            //TestSO();
        }


/*
        //comment out menu items for build
        [MenuItem("Utilities/RunTest")]
        public static void RunTest()
        {
            print("Test function:\n");

            // make list of scenarios
            List<Scenario> scenarios = Resources.LoadAll("Scenarios", typeof(Scenario)).Cast<Scenario>().ToList();

            var testScenario =
                from scenario in scenarios
                where scenario.Name == "Rise"
                select scenario;
            foreach (var scenario in testScenario)
            {
                print(scenario.LongTitle);
            }

            //var shortTest = scenarios.Where(s => s.Name == "Mundo");
            foreach (var s in scenarios.Where(s => s.Name == "Mundo"))
            { print(s.LongTitle); }

            var shortTest2 = scenarios.Max(s => s.CP);
            print(shortTest2);
        }

        [MenuItem("Utilities/SceneName")]
        public static void SceneName()
        {
            print(SceneManager.GetActiveScene().name);
        }

        public static void CreateSO()
        {
            Card cardTest = ScriptableObject.CreateInstance("Card") as Card;
            AssetDatabase.CreateAsset(cardTest, "Assets/Resources/Cards/cardTest.asset");
        }

        [MenuItem("Utilities/TestSO")]
        public static void TestSO()
        {
            var testName = "Test";
            Card currentCard = Resources.Load<Card>("Cards/card" + testName);
            currentCard.AddToPassage = Person.Woman;
            print(currentCard.AddToPassage);
            //print(currentCard.TestAddSO.Name);
            //Card.StepOne(currentCard); 
            //call to method uses class name, specific card info is in parameter
        }*/


    }
}
