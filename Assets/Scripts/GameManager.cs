using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Unity.VisualScripting;

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

        public Stack<GameStep> stepStack;

        void Awake()
        {
            if (Instance)
            { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gs = GetComponent<GameState>();
            savePath = Application.persistentDataPath + "/";
        }

        void Start()
        {
            print("Start Game Manager");
            //called only when game is loaded
            //initialize current scene name so that undo save works
            gs.CurrentSceneName = SceneManager.GetActiveScene().name;
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
            CreateFamiliesList();
            SceneManager.LoadScene(gs.CurrentSceneName);
        }

        void CreateFamiliesList()
        {
            gs.Families = new();
            //gs.Families.AddRange(gs.AllFamilies.Where(f => f.IsActive));
            gs.Families = gs.AllFamilies.FindAll(f => f.IsActive);
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
                CreateFamiliesList();
            }
            else
            {
                print($"Call to load undo data for {step.stepName} but no undo data found.");
            }
        }

        public void ExitGame()
        {
            // FileUtil available only in Editor mode; specify true to delete files and subdirectories
            if (Directory.Exists(savePath + gs.CurrentSceneName)) Directory.Delete(savePath + gs.CurrentSceneName, true);
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
            if (Directory.Exists(savePath + gs.CurrentSceneName)) Directory.Delete(savePath + gs.CurrentSceneName, true); 
            gs.PriorSceneName = SceneManager.GetActiveScene().name;
            gs.CurrentSceneName = newScene;
            SaveGame();
            SceneManager.LoadScene(newScene);
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