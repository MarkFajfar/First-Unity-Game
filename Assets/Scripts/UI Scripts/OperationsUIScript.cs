using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static NavajoWars.MakeFromInfo;

namespace NavajoWars
{
    public enum gsFunc { GameStateAP, ChildTest }

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
        VisualElement statusPanel;

        [SerializeField] VisualTreeAsset foldoutChild;

        // create a Dictionary of enum/Delegate
        Dictionary<gsFunc, Delegate> functions = new();
        // create a delegate that takes an int as a parameter
        delegate void gsFuncDelInt(int v);        
        // create a method that takes an int as a parameter
        void gsFuncAP(int v) { gs.AP = v; } 
        // in onEnable:
        //   instantiate a new delegate of type gsFuncDelInt
        //   set it equal to the method
        //   add the delegate to the dictionary with its enum key
        // same for a bool 
        delegate void gsFuncDelFamilyBool(Family f, bool b, string s);
        void gsFuncChild(Family f, bool b, string s) { f.HasChild = b; Debug.Log(s); }



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
                        
            choicePanel = root.Q<VisualElement>("ChoicePanel");
            locations = root.Q<RadioButtonGroup>("Locations");

            statusPanel = root.Q<VisualElement>("StatusPanel");

            gsFuncDelInt APDel = (int v) => gs.AP = v; 
            gsFuncDelFamilyBool ChildDel = gsFuncChild;

            functions.Add(gsFunc.GameStateAP, APDel);
            functions.Add(gsFunc.ChildTest, ChildDel);

            foreach (string function in Enum.GetNames(typeof(gsFunc)))
            {
                VisualElement element = statusPanel.Query(className: function);
                var key = (gsFunc)Enum.Parse(typeof(gsFunc), function);
                if (element is SliderInt slider) 
                {
                    gsFuncDelInt sliderFunc = (gsFuncDelInt)functions[key];
                    slider.RegisterValueChangedCallback(v => sliderFunc(v.newValue));
                }
                if (element is Toggle toggle && 
                    !element.GetClasses().ToList().
                    Contains(Foldout.toggleUssClassName)) 
                {
                    //make separate method to check parent against Territories, etc.
                    var parentClasses = toggle.parent.GetClasses().ToList();
                    string parentClass = "";
                    Family foldoutFamily = null;
                    foreach (string terr in Enum.GetNames(typeof(Territory))) 
                    { 
                        if (parentClasses.Contains(terr)) parentClass = terr;
                        // could register callback here
                    }
                    foreach (Family family in gs.AllFamilies) 
                    {
                        string name = family.Name.Replace(" ", "");
                        if (parentClasses.Contains(name))
                        {
                            parentClass = name;
                            foldoutFamily = family;
                        }
                    }
                    // assign a family to test
                    foldoutFamily = gs.Families[1]; // delete after testing
                    if (foldoutFamily != null) 
                    { 
                        // register callback that passes v and foldoutFamily
                    }
                    if (parentClass != "")
                    {
                        // make delegate that takes v and parentClass
                        gsFuncDelFamilyBool toggleFunc = (gsFuncDelFamilyBool)functions[key];
                        toggle.RegisterValueChangedCallback(v => toggleFunc(foldoutFamily, v.newValue, parentClass));
                    }
                }
            }
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

        public override void ShowChoiceButtons(List<ButtonInfo> choices)
        {
            ClearChoicePanel();
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            foreach (ButtonInfo info in choices)
            {
                Button choiceButton = info.Make(); // MakeButtonFromInfo(choice);
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
            //button.RegisterCallback<ClickEvent>(buttonClicked);
            button.style.display = DisplayStyle.Flex;
            choicePanel.Add(button);
        }
        
        public override void showButton(ButtonInfo bparams)
        {
            choicePanel.style.display = DisplayStyle.Flex;
            choicePanel.visible = true;
            Button button = bparams.Make();
            button.RegisterCallback<ClickEvent>(buttonClicked);
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
                button.RegisterCallback<ClickEvent>(buttonClicked);
            foreach (var toggle in ve.Query<Toggle>().ToList())
            {
                // check if toggle is sub-element of a foldout 
                List<string> classes = (List<string>)toggle.GetClasses();
                if (!classes.Contains(Foldout.toggleUssClassName))
                toggle.RegisterValueChangedCallback(toggleValueChanged);
            }
        }

        internal void closeFoldout(FoldoutInfo info)
        {
            Foldout foldout = choicePanel.Query<Foldout>(info.name);
            foldout.style.display = DisplayStyle.None;
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
    }
}
