namespace MinesServer.GameShit.GUI
{
    public interface IPage
    {
        public string? Title { get; init; }
        public string? Text { get; init; }
        public Action? OnAdmin { get; init; }

        public bool ProcessButton(string action);
    }
}
