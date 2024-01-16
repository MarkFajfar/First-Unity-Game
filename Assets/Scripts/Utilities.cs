using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace NavajoWars
{
    public class Utilities : MonoBehaviour
    {

        public static string Are(int i) => i == 1 ? "is one" : $"are {i}";

#if UNITY_EDITOR

        public class MyClass
        {
            public string name;
            public int number;

            public Action<int> UtilChoice;

            public async void runAsyncTest()
            {
                print("Starting Test");
                number = 0;
                //number = await DoTheThingInt();
                for (int i = 0; i < 5; i++)
                {
                    number = await DoTheThingInt();
                    print($"the number is {i * number}");
                } 
                print($"Finished Test, the number is {number}");
            }

            async Task<int> DoTheThingInt()
            {
                print("waiting ...");
                var result = new TaskCompletionSource<int>();
                Action<int> eventHandler = (i) => result.SetResult(i);
                UtilChoice += eventHandler;
                //print("Finished The Thing");
                //await Task.Delay(5000);
                //if (result.Task.Result != 1000) return 0;
                await result.Task;
                UtilChoice -= eventHandler;
                return result.Task.Result;
            }
        }

        static MyClass change(MyClass obj)
        {
            obj.number++;
            return obj;
        }

        public static MyClass mc;

        [MenuItem("Utilities/Async Test")]
        public static void AsyncTest()
        {
            mc = new MyClass();
            mc.runAsyncTest();
        }

        [MenuItem("Utilities/Async Increment")]
        public static void AsyncIncrement()
        {
            int i = 1000;
            mc.UtilChoice?.Invoke(i);
        }

        [MenuItem("Utilities/String Test")]
        public static void StringTest() 
        {
            string s = $"{Person.Man}";
            Debug.Log(s);
        }

        [MenuItem("Utilities/Dropdown Test")]
        public static void SetupTest() 
        {
            //var gmobj = GameObject.FindWithTag("GameController");
            //var gs = gmobj.GetComponent<GameState>();
            print("placeholder");
        }
        

        [MenuItem("Utilities/ObjTest")]
        public static void ObjTest()
        {
            MyClass obj = new() { name = "Test", number = 1};
            var newObj = change(obj);
            print(obj.number);
            print(newObj.number);
            print(obj == newObj);
        }       


        struct sf1 
        { 
            // when put in struct, class is still ref type
            public Family family1; 
            public void setHorse(bool horse) 
            { 
                family1.HasHorse = horse;
            }
        }
        
        struct sf2 { public Family family2; }

        static List<int> testlist = new () { 1, 2, 3, 4, 5, 6 };

        [MenuItem("Utilities/ListTest")]
        public static void ListTest() 
        {

            List<int> small = testlist.Where(x => x < 4).ToList();
            foreach (int i in small) Debug.Log("First: " + i);
            small = testlist.FindAll(x => x > 3);
            foreach (int i in small) Debug.Log("Second: " + i);



            Family tfA = new() { Name = "Test A" };
            Family tfB = new() { Name = "Test B" };

            sf1 sf11 = new() { family1 = tfA };
            sf2 sf12 = new() { family2 = tfB };
            
            List<Family> list1 = new() { tfA, tfB };
            list1[0].HasHorse = true;
            Debug.Log($"First Horse Test tfA: {tfA.HasHorse}");
            Debug.Log($"First Horse Test struct: {sf11.family1.HasHorse}");
            if (sf11.family1.HasHorse == true) sf11.family1.HasHorse = false;
            Debug.Log($"Second Horse Test tfA: {tfA.HasHorse}, {sf11.family1 == list1[0]}");
        }

        static Family FamilyA = new() { Name = "Family A" };

        //private MyClass Mc { get => mc; set => mc = value; }

        [MenuItem("Utilities/FamilyTest")]
        public static void FamilyTest() 
        { 
            Debug.Log("nameof: " + nameof(FamilyA));
            Debug.Log("Name:   " + FamilyA.Name); 
        }


        [MenuItem("Utilities/RunScenarioTest")]
        public static void RunScenarioTest()
        {
            Debug.Log("Test function:\n");

            // make list of scenarios
            List<Scenario> scenarios = Resources.LoadAll("Scenarios", typeof(Scenario)).Cast<Scenario>().ToList();

            var testScenario =
                from scenario in scenarios
                where scenario.Name == "Rise"
                select scenario;
            foreach (var scenario in testScenario)
            {
                Debug.Log(scenario.LongTitle);
            }

            //var shortTest = scenarios.Where(s => s.Name == "Mundo");
            foreach (var s in scenarios.Where(s => s.Name == "Mundo"))
            { Debug.Log(s.LongTitle); }

            var shortTest2 = scenarios.Max(s => s.CP);
            Debug.Log(shortTest2);
        }

        [MenuItem("Utilities/SceneName")]
        public static void SceneName()
        {
            Debug.Log(SceneManager.GetActiveScene().name);
        }

        [MenuItem("Utilities/CreateSO")]
        public static void CreateSO()
        {
            Card cardEvent = ScriptableObject.CreateInstance("Card") as Card;
            AssetDatabase.CreateAsset(cardEvent, "Assets/Resources/Cards/cardEvent.asset");
        }

        [MenuItem("Utilities/TestSO")]
        public static void TestSO()
        {
            var testName = "Test";
            Card currentCard = Resources.Load<Card>("Cards/card" + testName);
            currentCard.AddToPassage = Person.Woman;
            Debug.Log(currentCard.AddToPassage);
            //Debug.Log(currentCard.TestAddSO.Name);
            //Card.StepOne(currentCard); 
            //call to method uses class name, specific card info is in parameter
        }

        [MenuItem("Utilities/UpdateSO")]
        public static void UpdateSO()
        {
            var testName = "Fearing";
            Scenario scenario = Resources.Load<Scenario>("Scenarios/" + testName);
            //scenario.Subjugation.Red = 3;
            //Debug.Log(testName + " Red is " + scenario.Subjugation.Red);
        }

        /*[MenuItem("Utilities/RunTest")]

        public static void RunTest()
        {
            Debug.Log("Test function:\n");

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
            Debug.Log(k + "\n");
        }

        static void number() { Debug.Log(k + "\n"); }*/

#endif
    }
}
