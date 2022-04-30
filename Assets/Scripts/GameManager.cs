using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace NavajoWars
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        GameState gs;

        Scene currentScene;
        IsUIScript currentUIScript;
        GameObject currentGameObjectUI;

        // probably don't need references to all the scripts
        MainMenuUIScript mainMenuUIScript;
        CardDrawUIScript cardDrawUIScript;       

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gs = GetComponent<GameState>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Start()
        {
            checkForSavedGame();
        }
        
        // initialize main menu
        internal void checkForSavedGame()
        {
            string path = Application.persistentDataPath + "/savefile.json";
            if (File.Exists(path))
            { currentGameObjectUI.SendMessage("showLoadPanel"); }
            else
            { currentGameObjectUI.SendMessage("showScenarios"); }
        }

        // call from main menu to start a new game
        public void StartNewGame(Scenario scenario)
        {
        gs.ChosenScenario = scenario;
        gs.AP = scenario.AP;
        gs.CP = scenario.CP;
        gs.MP = scenario.MP;
        gs.Ferocity = scenario.Ferocity;
        gs.Morale = scenario.Morale;
            
        gs.FamilyA.IsActive = scenario.IsActiveFamilyA;
        gs.FamilyB.IsActive = scenario.IsActiveFamilyB;
        gs.FamilyC.IsActive = scenario.IsActiveFamilyC;
        gs.FamilyD.IsActive = scenario.IsActiveFamilyD;
        gs.FamilyE.IsActive = scenario.IsActiveFamilyE;
        gs.FamilyF.IsActive = scenario.IsActiveFamilyF;
        LoadNewScene("CardDraw");
        }

        // save and load functions
        public void SaveGame()
        {
            string sd = JsonUtility.ToJson(gs);
            print("Saving Data: " + sd);
            File.WriteAllText(Application.persistentDataPath + "/savefile.json", sd);
        }

        public void LoadSave()
        {
            string sd = File.ReadAllText(Application.persistentDataPath + "/savefile.json");
            JsonUtility.FromJsonOverwrite(sd, gs);            
            LoadNewScene(gs.CurrentSceneName);
        }

        internal void DeleteSaveAndStartNew()
        {
            string path = Application.persistentDataPath + "/savefile.json";
            File.Copy(path, Application.persistentDataPath + "/savefile.bak", true);
            if (File.Exists(path)) File.Delete(path); 
            checkForSavedGame();
        }

        public void ExitGame()
        {
            SaveGame();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit(); // original code to quit Unity player
#endif
        }

        // scene management
        void LoadNewScene(string newScene)
        {
            gs.PriorSceneName = SceneManager.GetActiveScene().name;
            gs.CurrentSceneName = newScene;
            SaveGame();
            SceneManager.LoadScene(newScene);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            currentScene = scene;
            currentGameObjectUI = GameObject.Find(currentScene.name + "UI");
            currentUIScript = currentGameObjectUI.GetComponent<IsUIScript>();
            // use IsUIScript to call any method in interface
            // can also use:  
            // currentGameObjectUI.SendMessage("SayHello");
            
            /* initialization is in each UI script's Start()
            // use switch for anything that the game manager needs to do when scene loads
            switch (scene.name)
            {
                case "CardDraw":
                    currentGameObjectUI.SendMessage("showKeyboard");
                    cardDrawUIScript = currentGameObjectUI.GetComponent<CardDrawUIScript>();
                    break;
            }*/
        }
        internal void PrevScene()
        {
            string newScene;
            if (gs.PriorSceneName == "MainMenu")
            {
                newScene = "MainMenu";
            }
            else
            {
                newScene = gs.PriorSceneName;
                gs.PriorSceneName = SceneManager.GetActiveScene().name;
                gs.CurrentSceneName = newScene;
            }
            SaveGame();
            SceneManager.LoadScene(newScene);
        }

        //card draw
        internal void CardNumInput() 
        {
            Card currentCard = Resources.Load<Card>("Cards/cardTest");
            //Card currentCard = Resources.Load<Card>("Cards/card" + gs.CurrentCardNum.ToString("D2"));
            gs.CurrentCard = currentCard;
            currentCard.StepOne(currentCard);
            // StepOne on each card starts one or more methods ending in call back to NewScene
        }

        public void EventCardTest(Card currentCard)
        {
            print("Running Event for Card: " + currentCard.CardNumber.ToString("D2"));
        }
    }
}