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
            gs.ChosenScenario = scenario;
            gs.AP = scenario.AP;
            gs.CP = scenario.CP;
            gs.MP = scenario.MP;
            gs.Ferocity = scenario.Ferocity;
            gs.Morale = scenario.Morale;

            gs.TradeGoodsMax = 3;
            gs.Firearms = 0;

            // resources

            gs.CeremonyCardsInHand = new();
            gs.EventCardsInPlay = new();
            gs.PersonsInPassage = new();
            gs.ElderDisplay = new int[7] { 1, 1, 1, 0, 0, 0, 0 };

            gs.FamilyA = new() { Name = "Family A" };
            gs.FamilyB = new() { Name = "Family B" };
            gs.FamilyC = new() { Name = "Family C" };
            gs.FamilyD = new() { Name = "Family D" };
            gs.FamilyE = new() { Name = "Family E" };
            gs.FamilyF = new() { Name = "Family F" };

            gs.Families = new();

            gs.FamilyA.IsActive = scenario.IsActiveFamilyA;
            if (gs.FamilyA.IsActive) gs.Families.Add(gs.FamilyA);
            gs.FamilyB.IsActive = scenario.IsActiveFamilyB;
            if (gs.FamilyB.IsActive) gs.Families.Add(gs.FamilyB);
            gs.FamilyC.IsActive = scenario.IsActiveFamilyC;
            if (gs.FamilyC.IsActive) gs.Families.Add(gs.FamilyC);
            gs.FamilyD.IsActive = scenario.IsActiveFamilyD;
            if (gs.FamilyD.IsActive) gs.Families.Add(gs.FamilyD);
            gs.FamilyE.IsActive = scenario.IsActiveFamilyE;
            if (gs.FamilyE.IsActive) gs.Families.Add(gs.FamilyE);
            gs.FamilyF.IsActive = scenario.IsActiveFamilyF;
            if (gs.FamilyF.IsActive) gs.Families.Add(gs.FamilyF);

            if (scenario.Name == "Rope") 
            { 
                gs.Firearms = 1;
                // Rope also has Manuelito counter and different family setup
            }

            locationNames = new()
            { " Splitrock", " San Juan", " Zuni", " Monument", " Hopi", " Black Mesa", " C. de Chelly" };
            ferocityNames = new()
            { "  0", "  1", "  2", "  3" };
            fNum = 0;
            assignFamilyValues();
        }

        void assignFamilyValues()
        {
            GameState.Family family = gs.Families[fNum];
            ui.headline.text = family.Name;
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
