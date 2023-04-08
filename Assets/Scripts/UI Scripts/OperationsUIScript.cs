using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NavajoWars
{
    public class OperationsUIScript : UIScript, IReceive
    {
        public GameObject LogicObject;
        public OperationsLogic logic; 
        
        Label headline;
        Label message;
        Button prev;
        Button quit;
        Button back;
        Button next;
        public string[] choiceButtonStyles;

        VisualElement choicePanel;
        RadioButtonGroup locations; 
        List<Foldout> foldouts;

        [SerializeField] VisualTreeAsset foldoutChild;

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
            // ClearChoicePanel(); // this is called later, messing up load?
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
            ClearChoicePanel();
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

        Button MakeButtonFromInfo(ButtonInfo info)
        {
            Button button = new()
            {   
                name = info.name,
                text = info.text, 
                tabIndex = info.tabIndex,
                userData = info
                // note info can be altered prior to creation of button, eg to add foldout Name
            };
            button.AddToClassList(info.style);
            //foreach (string style in choiceButtonStyles) choiceButton.AddToClassList(style);
            return button;
        }

        public override void ShowChoiceButtons(List<ButtonInfo> choices)
        {
            ClearChoicePanel();
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            foreach (ButtonInfo choice in choices)
            {
                Button choiceButton = MakeButtonFromInfo(choice);
                choiceButton.RegisterCallback<ClickEvent>(choiceButtonClicked);
                choicePanel.Add(choiceButton);
                choiceButton.visible = true;
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
            Button button = MakeButtonFromInfo(bparams);
            button.RegisterCallback<ClickEvent>(buttonClicked);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }

        Foldout MakeFoldoutFromInfo(FoldoutInfo info)
        {
            Foldout foldout = new()
            {
                value = false,
                name = info.name,
                text = info.text,
                tabIndex = info.tabIndex,
                userData = info
            };

            foreach (ButtonInfo bparams in info.buttons)
            {
                // add foldout name and even data to bparams before making button?
                bparams.parentName = info.name;
                bparams.parentData = info;
                // note allows treatment of info as object, but does not "know" that parentData in this case is FoldoutInfo
                bparams.clearPanel = false; // do not clear panel when foldout clicked
                Button foldoutButton = MakeButtonFromInfo(bparams);
                foldoutButton.RegisterCallback<ClickEvent>(foldoutButtonClicked);
                foldout.Add(foldoutButton);
                foldoutButton.style.display = DisplayStyle.Flex;
                foldoutButton.visible = true;
            }

            foldout.AddToClassList(info.style);
            return foldout;
        }

        public override void ShowChoiceFoldouts(List<FoldoutInfo> foldouts)
        {
            ClearChoicePanel();
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;

            foreach (FoldoutInfo info in foldouts)
            {
                Foldout choiceFoldout = MakeFoldoutFromInfo(info);
                choiceFoldout.style.display = DisplayStyle.Flex;
                choiceFoldout.visible = true;
                choicePanel.Add(choiceFoldout);
            }/*// https://gamedev.stackexchange.com/questions/199609/how-to-create-instances-of-a-unity-ui-toolkit-template-from-code
            // [SerializeField] VisualTreeAsset to be assigned in inspector
            // instantiate a copy of that VisualTreeAsset as a VisualElement
            // query to find the Foldout in the VisualElement
            // make any changes to the Foldout (this method for when not making changes)
            // add the Visual Element to the choicePanel
            VisualElement foldoutToAdd = foldoutChild.Instantiate();
            //Foldout testfoldout = foldoutToAdd as Foldout; //does not work
            Foldout embedded = foldoutToAdd.Q<Foldout>();
            embedded.text = "Test Name";
            foldoutToAdd.visible = true;
            choicePanel.Add(foldoutToAdd);

            foreach (FoldoutInfo foldout in foldouts)
            // for (int i = 0; i < (childrenInPassage.Count() + childrenInFamilies.Count()); i++)
            {
                // Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);

                var choiceFoldout = new Foldout();
                choiceFoldout.AddToClassList("FoldoutChild");
                choiceFoldout.value = false;
                choiceFoldout.style.display = DisplayStyle.Flex;
                choiceFoldout.text = foldout.name.ToString(); // child? family name?

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
            }*/
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

        public void ShowChoiceToggles(List<ToggleInfo> toggles)
        {
            ClearChoicePanel();
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            foreach (ToggleInfo info in toggles)
            {
                Toggle toggle = new()
                {
                    label = info.label,
                    name = info.name,
                    userData = info,
                };
                toggle.AddToClassList(info.style);
                toggle.RegisterValueChangedCallback(toggleValueChanged);
                choicePanel.Add(toggle);
                toggle.visible = true;
                toggle.style.display = DisplayStyle.Flex;
            }
            // move to makeToggleFromInfo method? or used only here?
        }

        void toggleValueChanged(ChangeEvent<bool> evt) 
        {
            var toggle = evt.target as Toggle;
            var info = toggle.userData as ToggleInfo;
            //Action<ToggleInfo, bool> passBack = info.passBack;
            info.passBack?.Invoke(info, toggle.value);
        }

        public override void ClearChoicePanel()
        {
            List<VisualElement> choiceElements = new();
            //List<Button> buttons = choicePanel.Query<Button>().ToList();
            choiceElements.AddRange(choicePanel.Query<Button>().ToList());
            choiceElements.AddRange(choicePanel.Query<Toggle>().ToList());
            choiceElements.AddRange(choicePanel.Query<Foldout>().ToList());
            //choiceElements.AddRange(choicePanel.Query<RadioButtonGroup>().ToList());
            // instead of removing radio button group, use CloseLocations
            // IEnumerable<VisualElement> choiceElements = new[] { buttons, foldouts, radios };

            CloseLocations();

            foreach (VisualElement element in choiceElements)
            {
                element.style.display = DisplayStyle.None;
                element.RemoveFromHierarchy();
            }
        }



        public void makeTestButton()
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            ChoiceButton button = new()
            {
                testString = "Test",
                passback = testResult
            };
            button.RegisterCallback<ClickEvent>(testButtonClicked);
            button.AddToClassList("ButtonMenu");
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }

        void testButtonClicked(ClickEvent evt)
        {
            var clickedButton = evt.target as ChoiceButton;
            print(clickedButton.testString);
            clickedButton.ChangeText("Clicked!");
            clickedButton.passback?.Invoke();
        }

        void testResult()
        {
            displayText("this is the test Result");
        }
    }
}
