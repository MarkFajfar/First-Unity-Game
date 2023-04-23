using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace NavajoWars
{
    public class SetupLogic : MonoBehaviour
    {
        GameManager gm;
        GameState gs;
        MainMenuUIScript ui;

        List<string> locationNames;
        List<string> ferocityNames;

        int fNum = 0;

        void Awake()
        {
            var gmobj = GameObject.FindWithTag("GameController");
            gm = gmobj.GetComponent<GameManager>();
            gs = gmobj.GetComponent<GameState>();
            ui = gameObject.GetComponent<MainMenuUIScript>();
        }

        // called from main menu to start a new game
        public void SetupNewGame(Scenario scenario)
        {
            // load values from Scenario
            
            gs.ChosenScenario = scenario;
            gs.AP = scenario.AP;
            gs.CP = scenario.CP;
            gs.MP = scenario.MP;
            gs.Ferocity = scenario.Ferocity;
            gs.Morale = scenario.Morale;

            gs.HorsesHeld = scenario.Horses;
            gs.HorsesMax = scenario.HorsesMax;
            gs.SheepHeld = scenario.Sheep;
            gs.SheepMax = scenario.SheepMax; 
            gs.TradeGoodsMax = 3;
            gs.Firearms = 0;

            gs.HasDrought = scenario.HasDrought;

            if (scenario.Name == "Broken" || scenario.Name == "Fearing")
                gs.Subjugation.Red = 3;

            if (scenario.Name == "Rope")
            {
                gs.Firearms = 1;
                // Rope also has Manuelito counter and different family setup
            }

            // setup tables and families

            gs.CeremonyCardsInHand = new();
            gs.EventCardsInPlay = new();
            gs.PersonsInPassage = new();
            gs.ElderDisplay = new int[7] { 1, 1, 1, 0, 0, 0, 0 };

            Family FamilyA = new() { Name = "Family A" };
            Family FamilyB = new() { Name = "Family B" };
            Family FamilyC = new() { Name = "Family C" };
            Family FamilyD = new() { Name = "Family D" };
            Family FamilyE = new() { Name = "Family E" };
            Family FamilyF = new() { Name = "Family F" };
            Family DefaultFamily = new() { Name = Info.Default };

            gs.Families = new();

            FamilyA.IsActive = scenario.IsActiveFamilyA;
            if (FamilyA.IsActive) gs.Families.Add(FamilyA);
            FamilyB.IsActive = scenario.IsActiveFamilyB;
            if (FamilyB.IsActive) gs.Families.Add(FamilyB);
            FamilyC.IsActive = scenario.IsActiveFamilyC;
            if (FamilyC.IsActive) gs.Families.Add(FamilyC);
            FamilyD.IsActive = scenario.IsActiveFamilyD;
            if (FamilyD.IsActive) gs.Families.Add(FamilyD);
            FamilyE.IsActive = scenario.IsActiveFamilyE;
            if (FamilyE.IsActive) gs.Families.Add(FamilyE);
            FamilyF.IsActive = scenario.IsActiveFamilyF;
            if (FamilyF.IsActive) gs.Families.Add(FamilyF);

            gs.AllFamilies = new() { FamilyA, FamilyB, FamilyC, FamilyD, FamilyE, FamilyF };

            // setup initial game info

            gs.isPlayerOpsDone = false;
            gs.isEnemyOpsDone = false;
            gs.isPreempt = false;
            gs.canBackToDraw = true;

            // setup family values

            locationNames = new()
            { " Splitrock", " San Juan", " Zuni", " Monument", " Hopi", " Black Mesa", " C. de Chelly" };
            ferocityNames = new()
            { "  0", "  1", "  2", "  3" };
            fNum = 0;
            assignFamilyValues();
        }

        void assignFamilyValues()
        {
            Family family = gs.Families[fNum];
            //ui.displayHeadline(family.Name);
            ui.viewSetup.text = $"Select {family.Name}'s\nLocation and Ferocity";
            ui.locations.visible = true;
            ui.locations.choices = locationNames;
            ui.ferocities.visible = true;
            ui.ferocities.choices = ferocityNames;
        }

        public void nextFamily()
        {
            gs.Families[fNum].IsWhere = (Territory)ui.locations.value+1;
            gs.Families[fNum].Ferocity = ui.ferocities.value;
            print($"Location of {gs.Families[fNum].Name} is {gs.Families[fNum].IsWhere.ToString()}");
            print($"Ferocity of {gs.Families[fNum].Name} is {gs.Families[fNum].Ferocity}");
            fNum++;
            if (fNum < gs.Families.Count) assignFamilyValues();
            else
            {
                gm.SaveGame();
                gm.LoadNewScene("CardDraw");
            }
        }

        public void backFamily()
        {
            fNum--;
            if (fNum < 0) ui.viewSetup.text = "To select a new scenario, restart the game.";
            // call ui.Start(); ?
            else assignFamilyValues();
        }
    }
}
