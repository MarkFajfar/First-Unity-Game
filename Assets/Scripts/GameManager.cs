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
        //IsUIScript currentUIScript;
        GameObject currentGameObjectUI;

        string savePath;

        void Awake()
        {
            if (Instance)
            { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gs = GetComponent<GameState>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            savePath = Application.persistentDataPath + "/savefile.json";
        }

        void Start()
        {
            print("Start Game Manager");
            //called only when game is loaded
            checkForSavedGame(GameObject.Find("MainMenuUI").GetComponent<MainMenuUIScript>());
        }
        
        // initialize main menu
        internal void checkForSavedGame(MainMenuUIScript mainMenu)
        {
            if (File.Exists(savePath)) mainMenu.showLoadPanel(); 
            else mainMenu.showScenarios();
        }


        // save and load functions
        public void SaveGame()
        {
            string sd = JsonUtility.ToJson(gs);
            print("Saving Data: " + sd);
            File.WriteAllText(savePath, sd);
        }

        public void tSaveGame()
        {
            string sd = JsonUtility.ToJson(gs);
            print("Test Saving Data: " + sd);
            File.WriteAllText(savePath, sd);
        }

        public void tLoadGame()
        {
            string sd = File.ReadAllText(savePath);
            print("Test Loading Data");
            JsonUtility.FromJsonOverwrite(sd, gs);
        }

        public void LoadSave()
        {
            string sd = File.ReadAllText(savePath);
            JsonUtility.FromJsonOverwrite(sd, gs);
            SceneManager.LoadScene(gs.CurrentSceneName);
        }

        internal void DeleteSaveAndStartNew()
        {
            File.Copy(savePath, Application.persistentDataPath + "/savefile.bak", true);
            if (File.Exists(savePath)) File.Delete(savePath); 
            checkForSavedGame(GameObject.Find("MainMenuUI").GetComponent<MainMenuUIScript>());
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
        public void LoadNewScene(string newScene)
        {
            gs.PriorSceneName = SceneManager.GetActiveScene().name;
            gs.CurrentSceneName = newScene;
            SaveGame();
            SceneManager.LoadScene(newScene);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            // wait to see which of this is necessary
            //currentScene = scene; // stored in gs
            //currentGameObjectUI = GameObject.FindWithTag("UI"); 
            //currentUIScript = currentGameObjectUI.GetComponent<IsUIScript>();
            // use IsUIScript or IsMethodReceiver to call any method in interface
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
            newScene = gs.PriorSceneName;
            // except if returning to MainMenu, swap prior and current scene names
            if (newScene != "MainMenu")
            {
                gs.PriorSceneName = SceneManager.GetActiveScene().name;
                gs.CurrentSceneName = newScene;
            }
            SaveGame();
            SceneManager.LoadScene(newScene);
        }

        //card draw, then call to card type scene
        internal void CardNumInput()
        {
            Card currentCard = Resources.Load<Card>("Cards/card" + gs.CurrentCardNum.ToString("D2"));
            gs.CurrentCard = currentCard;
            LoadNewScene(currentCard.ThisCardType.ToString());
        }

        public void EventCardTest(Card currentCard)
        {
            print("Running Event for Card: " + currentCard.CardNumber.ToString("D2"));
        }

        public void OnApplicationFocus(bool focus)
        {
            if (!focus) SaveGame();
        }
    }
}