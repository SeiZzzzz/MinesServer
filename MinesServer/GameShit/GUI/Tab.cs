using MinesServer.Network;

namespace MinesServer.GameShit.GUI
{
    public class Tab
    {
        IPage _initPage;

        public string Title { get; init; }

        public required string Label { get; init; }

        public bool IsHidden { get; init; } = false;

        public required string Action { get; init; }

        public IPage InitialPage
        {
            get => _initPage;
            set
            {
                _initPage = value;
                ClearHistory();
            }
        }

        public Stack<IPage> History { get; } = new();

        public void ClearHistory()
        {
            History.Clear();
            Open(InitialPage!);
        }

        public void Open(IPage page)
        {
            History.Push(page);
        }

        public void Replace(IPage page)
        {
            History.Pop();
            Open(page);
        }

        public bool ProcessButton(string action)
        {
            if (action[0] == '<')
            {
                if (action[1..] != Action)
                    throw new InvalidPayloadException("Action does not match the tab");
                if (History.Count <= 1)
                    throw new InvalidPayloadException("Back button is not supported in this state");
                History.Pop();
                return true;
            }

            // Pass the action to the current page
            return History.Peek().ProcessButton(action);
        }
    }
}
