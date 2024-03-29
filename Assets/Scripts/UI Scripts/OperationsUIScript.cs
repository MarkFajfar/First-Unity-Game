using System;
using System.Collections.Generic;
using System.Linq;
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
        Button status;
        public string[] choiceButtonStyles;

        VisualElement bodyPanel;
        VisualElement choicePanel;
        VisualElement statusPanel;
        RadioButtonGroup locations;
        
        [SerializeField] VisualTreeAsset foldoutChild;

        void OnEnable()
        {
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

            status = root.Q<Button>("Status");
            status.clicked += statusClicked;

            bodyPanel = root.Q<VisualElement>("Body");
            choicePanel = root.Q<VisualElement>("ChoicePanel");
            statusPanel = root.Q<VisualElement>("StatusPanel");
            locations = root.Q<RadioButtonGroup>("Locations");
        }

        public override void nextClicked() 
        { 
            base.nextClicked();
        }

        public override void backClicked()
        {
            // always clear choice panel?
            ClearChoicePanel();
            // check first if any step in stack; if not, go to subscribers to backClicked
            if (gm.stepStack.Count > 0) gm.stepStack.Pop().Undo(); 
            else base.backClicked();
        }

        protected override void statusClicked()
        {
            // base.statusClicked();
            // switch display of statusPanel and bodyPanel
            /*var showing = statusPanel.style.display;
            statusPanel.style.display = bodyPanel.style.display;
            bodyPanel.style.display = showing;*/
            if (statusPanel.style.display == DisplayStyle.None)
            {
                statusPanel.style.display = DisplayStyle.Flex;
                bodyPanel.style.display = DisplayStyle.None;
            }
            else 
            {
                statusPanel.style.display = DisplayStyle.None;
                bodyPanel.style.display = DisplayStyle.Flex;
            }
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
            status.visible = true;
            bodyPanel.style.display = DisplayStyle.Flex;
            choicePanel.style.display = DisplayStyle.Flex;
            statusPanel.style.display = DisplayStyle.None;
            // ClearChoicePanel(); // this is called later, messing up load?
        }

        public override void Initialize()
        {
            prev.visible = false;
            quit.visible = true;
            back.visible = true;
            next.visible = true;
            status.visible= true;
            headline.visible = true;
            message.visible = true;
            bodyPanel.style.display = DisplayStyle.Flex;
            choicePanel.style.display = DisplayStyle.Flex;
            statusPanel.style.display = DisplayStyle.None;
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
        { prev.visible = true; }
        
        public override void hidePrev()
        { prev.visible = false; }

        public override Button makeButton(ButtonInfo info)
        {
            Button button = info.Make();
            button.RegisterCallback<ClickEvent>(ButtonClicked);
            return button;
        }

        public override void ShowChoiceButtons(List<ButtonInfo> choices)
        {
            ClearChoicePanel();
            
            bodyPanel.style.display = DisplayStyle.Flex;
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            
            foreach (ButtonInfo info in choices)
            {
                Button choiceButton = makeButton(info); // MakeButtonFromInfo(choice);
                choicePanel.Add(choiceButton);
                choiceButton.visible = true;
                choiceButton.style.display = DisplayStyle.Flex;
            }
        }

        public override void showChoiceButton(Button button)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            button.RegisterCallback<ClickEvent>(ButtonClicked);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }
        
        public override void showChoiceButton(ButtonInfo info)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            Button button = makeButton(info);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }

        public override void ShowChoiceFoldouts(List<FoldoutInfo> foldouts)
        {
            ClearChoicePanel();

            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;

            foreach (FoldoutInfo info in foldouts)
            {
                Foldout choiceFoldout = info.Make();
                registerCallbacks(choiceFoldout);
                choiceFoldout.style.display = DisplayStyle.Flex;
                choiceFoldout.visible = true;
                choicePanel.Add(choiceFoldout);
            }  
            /*// https://gamedev.stackexchange.com/questions/199609/how-to-create-instances-of-a-unity-ui-toolkit-template-from-code
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

        public void ShowChoiceToggles(List<ToggleInfo> toggles)
        {
            ClearChoicePanel();

            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;

            foreach (ToggleInfo info in toggles)
            {
                Toggle choiceToggle = info.Make();
                choiceToggle.RegisterValueChangedCallback(toggleValueChanged);
                choicePanel.Add(choiceToggle);
                choiceToggle.visible = true;
                choiceToggle.style.display = DisplayStyle.Flex;
            }
        }

        void registerCallbacks(VisualElement ve) 
        {
            foreach (var button in ve.Query<Button>().ToList())
                button.RegisterCallback<ClickEvent>(ButtonClicked);
            foreach (var toggle in ve.Query<Toggle>().ToList())
            {
                // check if toggle is sub-element of a foldout 
                //List<string> classes = (List<string>)toggle.GetClasses();
                if (!toggle.ClassListContains(Foldout.toggleUssClassName))
                toggle.RegisterValueChangedCallback(toggleValueChanged);
            }
        }

        internal void hideFoldout(FoldoutInfo info)
        {
            Foldout foldout = choicePanel.Query<Foldout>(info.name);
            foldout.style.display = DisplayStyle.None;
        }

        public void DisplayLocations()
        {
            ClearChoicePanel();
            List<string> locationNames = new()
            { " Splitrock", " San Juan", " Zuni", " Monument", " Hopi", " Black Mesa", " C. de Chelly" };
            if (!choicePanel.Contains(locations)) choicePanel.Add(locations); // is this necessary?
            locations.visible = true;
            locations.style.display = DisplayStyle.Flex;
            locations.choices = locationNames;
        }

        public Territory ReturnLocation() => gs.Territories[locations.value + 1];

        public void CloseLocations()
        { locations.style.display = DisplayStyle.None; }

        public override void ClearChoicePanel()
        {
            CloseLocations();

            foreach (var element in choicePanel.Children().ToList())
            { element.RemoveFromHierarchy(); }

            //List<Button> buttons = choicePanel.Query<Button>().ToList();
            //choiceElements.AddRange(choicePanel.Query<Button>().ToList());
            //choiceElements.AddRange(choicePanel.Query<Toggle>().ToList());
            //choiceElements.AddRange(choicePanel.Query<Foldout>().ToList());
            //choiceElements.AddRange(choicePanel.Query<RadioButtonGroup>().ToList());
            // instead of removing radio button group, use CloseLocations
            // IEnumerable<VisualElement> choiceElements = new[] { buttons, foldouts, radios };
        }
    }
}
