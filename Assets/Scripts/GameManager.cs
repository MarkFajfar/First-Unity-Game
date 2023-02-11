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
            savePath = Application.persistentDataPath + "/";
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
            if (File.Exists(savePath + "savefile.json")) mainMenu.showLoadPanel(); 
            else mainMenu.showScenarios();
        }


        // save and load functions
        public void SaveGame()
        {
            string sd = JsonUtility.ToJson(gs);
            print("Saving Data: " + sd);
            File.WriteAllText(savePath + "savefile.json", sd);
        }

        public void LoadSave()
        {
            string sd = File.ReadAllText(savePath + "savefile.json");
            JsonUtility.FromJsonOverwrite(sd, gs);
            SceneManager.LoadScene(gs.CurrentSceneName);
        }

        internal void DeleteSaveAndStartNew()
        {
            File.Copy(savePath + "savefile.json", savePath + "savefile.bak", true);
            if (File.Exists(savePath + "savefile.json")) File.Delete(savePath + "savefile.json"); 
            checkForSavedGame(GameObject.Find("MainMenuUI").GetComponent<MainMenuUIScript>());
        }

        public void SaveUndo(GameStep step)
        {
            string sd = JsonUtility.ToJson(gs);
            print("Saving Undo Data: " + sd);
            if (!Directory.Exists(savePath + gs.CurrentSceneName)) Directory.CreateDirectory(savePath + gs.CurrentSceneName);
            File.WriteAllText(savePath + gs.CurrentSceneName + "/" + step.stepName, sd);
        }

        public void LoadUndo(GameStep step)
        {
            if (File.Exists(savePath + gs.CurrentSceneName + "/" + step.stepName))
            {
                print("Loading Undo Data: " + step.stepName);
                string sd = File.ReadAllText(savePath + gs.CurrentSceneName + "/" + step.stepName);
                JsonUtility.FromJsonOverwrite(sd, gs);
            }
        }

        public void ExitGame()
        {
            if (Directory.Exists(savePath + gs.CurrentSceneName)) FileUtil.DeleteFileOrDirectory(savePath + gs.CurrentSceneName);
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