using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class OperationsUIScript : UIScript, IReceive
    {
        public GameObject LogicObject;
        public OperationsLogic logic; 
        
        public Label headline;
        public Label message;
        Button prev;
        Button quit;
        Button back;
        Button next;
        public string[] choiceButtonStyles;

        VisualElement choicePanel;
        RadioButtonGroup locations; 
        List<Foldout> foldouts;
        
        void OnEnable()
        {
            //choice = GameObject.Find("ChoiceUI").GetComponent<ChoiceUIScript>();
            //choice = GetComponent<ChoiceUIScript>();
            getVisualElements();
        }

        public void getVisualElements()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            headline = root.Q<Label>("Headline");

            message = root.Q<Label>("Message");
            
            back = root.Q<Button>("Back");
            back.clicked += backClicked;
            
            next = root.Q<Button>("Next");
            next.clicked += nextClicked; 
            
            prev = root.Q<Button>("Prev");
            prev.clicked += prevClicked;
            
            quit = root.Q<Button>("Quit");
            quit.clicked += quitClicked;
                        
            choicePanel = root.Q<VisualElement>("ChoicePanel");
            locations = root.Q<RadioButtonGroup>("Locations");
            //foldouts = choicePanel.Query<Foldout>().ToList();
        }

        public override void nextClicked() 
        { 
            base.nextClicked();
        }

        public override void backClicked()
        {
            // check first if any step in stack; if not, go to subscribers to backClicked
            if (gm.stepStack.Count > 0) gm.stepStack.Pop().Undo(); 
            else base.backClicked();
        }

        void Start()
        {
            // called when scene "Operations" is loaded, called from gm
            // no previous button
            prev.visible = false;
            // reset UI for reload
            headline.text = "Operations";
            headline.visible = true;
            message.visible = true;
            choicePanel.visible = true;
            back.visible = false;
            next.visible = false;
            quit.visible = true;
            CloseLocations();
        }

        public override void Initialize()
        {
            prev.visible = false;
            quit.visible = true;
            back.visible = true;
            next.visible = true;
            headline.visible = true;
            message.visible = true;
            choicePanel.visible = false;
            CloseLocations();
        }

        public override void displayText(string text)
        {
            message.visible = true;
            message.text = text;   
        }

        public override void addText(string text)
        {
            message.visible = true;
            message.text += text;
        }

        public override void displayHeadline(string text)
        {
            headline.visible = true;
            headline.text = text;
        }

        public override void addHeadline(string text)
        {
            headline.visible = true;
            headline.text += text;
        }

        public override void showBackNext()
        {
            back.visible = true;
            next.visible = true;
        }
        
        public override void hideBackNext()
        {
            back.visible = false;
            next.visible = false;
        }

        public override void showPrev()
        { prev.visible = false; }
        
        public override void hidePrev()
        { prev.visible = false; }

        public override void MakeChoiceButtons(List<ButtonInfo> choices)
        {
            ClearChoicePanel();
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            foreach (ButtonInfo choice in choices)
            {
                var choiceButton = new Button();
                choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
                choiceButton.AddToClassList(choice.style);
                //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
                choiceButton.name = choice.name;
                choiceButton.text = choice.text;
                choiceButton.tabIndex = choice.tabIndex;
                choiceButton.userData = choice;
                choicePanel.Add(choiceButton);
                choiceButton.style.display = DisplayStyle.Flex;
            }
        }

        public override void showButton(Button button)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            button.RegisterCallback<ClickEvent>(buttonClicked);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }
        
        public override void showButton(ButtonInfo bparams)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            Button choiceButton = new();
            choiceButton.RegisterCallback<ClickEvent>(buttonClicked);
            choiceButton.AddToClassList(bparams.style);
            //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
            choiceButton.name = bparams.name;
            choiceButton.text = bparams.text;
            choiceButton.tabIndex = bparams.tabIndex;
            choiceButton.userData = bparams;
            choiceButton.style.display = DisplayStyle.Flex;
            choicePanel.Add(choiceButton);
        }

        public override void MakeFamilyFoldouts(Dictionary<Person, GameState.Family> foldouts)
        // (List<Person> childrenInPassage, List<GameState.Family> childrenInFamilies)
        {
            // TODO: make this function generic ?? "MakeFoldouts"

            ClearChoicePanel();

            choicePanel.style.display = DisplayStyle.Flex;

            foreach (KeyValuePair<Person, GameState.Family> foldout in foldouts)
            // for (int i = 0; i < (childrenInPassage.Count() + childrenInFamilies.Count()); i++)
            {
                // Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);

                var choiceFoldout = new Foldout();
                choiceFoldout.AddToClassList("FoldoutChild");
                choiceFoldout.value = false;
                choiceFoldout.style.display = DisplayStyle.Flex;
                choiceFoldout.text = foldout.Key.ToString(); // child? family name?

                // each child can be converted into a man, woman or elder

                List<Person> foldoutPersons = new() { Person.Man, Person.Woman, Person.Elder };
                int n = 0;
                foreach (var person in foldoutPersons)
                {
                    var foldoutButton = new Button();
                    foldoutButton.RegisterCallback<ClickEvent>(foldoutClicked);
                    foldoutButton.AddToClassList("ButtonFoldout");  // create this style
                    foldoutButton.text = person.ToString();
                    n++;
                    foldoutButton.tabIndex = n;
                    foldoutButton.userData = person;
                    choiceFoldout.Add(foldoutButton);
                    foldoutButton.style.display = DisplayStyle.Flex;
                }
            }
        }

        public void DisplayLocations()
        {
            ClearChoicePanel();
            List<string> locationNames = new()
            { " Splitrock", " San Juan", " Zuni", " Monument", " Hopi", " Black Mesa", " C. de Chelly" };
            //choicePanel.Add(locations); // is this necessary?
            locations.visible = true;
            locations.style.display = DisplayStyle.Flex;
            locations.choices = locationNames;
        }

        public Territory ReturnLocation() => (Territory)locations.value + 1;

        public void CloseLocations()
        { locations.style.display = DisplayStyle.None; }

        public override void ClearChoicePanel()
        {
            List<VisualElement> choiceElements = new();
            //List<Button> buttons = choicePanel.Query<Button>().ToList();
            choiceElements.AddRange(choicePanel.Query<Button>().ToList());
            choiceElements.AddRange(choicePanel.Query<Foldout>().ToList());
            //choiceElements.AddRange(choicePanel.Query<RadioButtonGroup>().ToList());
            // instead of removing radio button group, use CloseLocations
            // IEnumerable<VisualElement> choiceElements = new[] { buttons, foldouts, radios };

            CloseLocations();

            foreach (var element in choiceElements)
            {
                element.style.display = DisplayStyle.None;
                element.RemoveFromHierarchy();
            }
        }
    }
}
