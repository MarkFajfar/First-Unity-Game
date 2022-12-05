namespace NavajoWars
{
    public interface IsUIScript 
    {
        public delegate void ClickNext();
        public event ClickNext OnOpsNextClick;
        public delegate void ClickBack();
        public event ClickBack OnOpsBackClick;
        void getVisualElements();
        public void Initialize();
        public void displayHeadline(string text);
        public void addHeadline(string text); 
        public void displayText(string text);
        public void addText(string text);
        public void showBackNext();
        public void hideBackNext();
        public void showPrev();
        public void hidePrev();
        public void unsubBack();
        public void unsubNext();
    }
}
