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
     /*for (int i = 0; i< 3; i++)
            {
                print("loop " + i);
                int j = 0;
                while (j< 5)
                {
                    print("j is: " + j);
                    if (j< 3)
                    {
                        StartCoroutine(waitnumber(j));
                        string result = await IReceive.testGetChoiceAsync();
                        print(result);
                        Debug.Log(result);
                    }
                    else number(j);
                    //ChoiceMadeString choice = new ChoiceMadeString(j.ToString());
                    //choice.OnChoiceMadeString(new ChoiceMadeString(j.ToString()));
                    j++;
                }
            }

            IEnumerator waitnumber(int j)
            {
                print("starting to wait" + j);
                yield return new WaitForSeconds(3f);
                print("done waiting" + j);
                ChoiceMadeString choice = new ChoiceMadeString(j.ToString());
                choice.OnChoiceMadeString(new ChoiceMadeString(j.ToString()));
            }

        void number(int k) { print(k + "\n"); }

        print("done");*/
        
        
        /*[MenuItem("Utilities/RunTest")]

        
        public static void RunTest()
        {
            print("Test function:\n");

            for (int i = 1; i < 3; i++)
            {
                int j = 0;
                while (j < 5)
                {
                    if (j<3) StartCoroutine(waitnumber(j));
                    else number();
                    j++;
                }
            }
        }   

        static IEnumerator waitnumber(int k)
        {
            yield return new WaitForSeconds(3f);
            print(k + "\n");
        }

        static void number() { print(k + "\n"); }*/

        /*GameManager GameManager;
        GameState gs;

        void Awake()
        {
            GameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            gs = GameManager.GetComponent<GameState>();
            //CreateSO();
            //TestSO();
        }



        //comment out menu items for build
        //[MenuItem("Utilities/RunTest")]
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

        //[MenuItem("Utilities/SceneName")]
        public static void SceneName()
        {
            print(SceneManager.GetActiveScene().name);
        }

        //[MenuItem("Utilities/CreateSO")]
        public static void CreateSO()
        {
            Card cardEvent = ScriptableObject.CreateInstance("Card") as Card;
            AssetDatabase.CreateAsset(cardEvent, "Assets/Resources/Cards/cardEvent.asset");
        }

        //[MenuItem("Utilities/TestSO")]
        public static void TestSO()
        {
            var testName = "Test";
            Card currentCard = Resources.Load<Card>("Cards/card" + testName);
            currentCard.AddToPassage = Person.Woman;
            print(currentCard.AddToPassage);
            //print(currentCard.TestAddSO.Name);
            //Card.StepOne(currentCard); 
            //call to method uses class name, specific card info is in parameter
        }

        [MenuItem("Utilities/UpdateSO")]
        public static void UpdateSO()
        {
            var testName = "Fearing";
            Scenario scenario = Resources.Load<Scenario>("Scenarios/" + testName);
            scenario.Subjugation.Red = 3;
            print(testName + " Red is " + scenario.Subjugation.Red);
        }*/


    }
}
