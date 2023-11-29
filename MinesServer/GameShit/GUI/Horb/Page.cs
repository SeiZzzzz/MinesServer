using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.GUI.Horb.List.Rich;

namespace MinesServer.GameShit.GUI.Horb
{
    public readonly struct Page : IPage
    {
        public required Button[] Buttons { get; init; }
        public CrystalConfig? CrystalConfig { get; init; }
        public ListEntry[]? List { get; init; }
        public RichListConfig? RichList { get; init; }
        public ClanListEntry[]? ClanList { get; init; }
        public InputConfig? Input { get; init; }
        public CanvasElement[]? Canvas { get; init; }
        public Style? Style { get; init; }
        public Card? Card { get; init; }
        public string? Title { get; init; }
        public string? Text { get; init; }
        public Action? OnAdmin { get; init; }

        public bool ProcessButton(string action)
        {
            foreach (var btn in Buttons)
                if (btn.ProcessButton(action))
                    return true;

            foreach (var i in List ?? Enumerable.Empty<ListEntry>())
                if (i.Button?.ProcessButton(action) == true)
                    return true;

            foreach (var i in RichList?.Entries ?? Enumerable.Empty<RichListEntry>())
                if (i.Type == RichListEntryType.Button && i.Buttons![0].ProcessButton(action))
                    return true;
                else if(i.Type == RichListEntryType.Fill)
                {
                    foreach (var btn in i.Buttons!)
                        if (btn.ProcessButton(action))
                            return true;
                }
                else if (i.Type == RichListEntryType.Card)
                    foreach (var e in i.Cards!)
                        if (e.Button.ProcessButton(action))
                            return true;

            foreach (var i in ClanList ?? Enumerable.Empty<ClanListEntry>())
                if (i.Button.ProcessButton(action))
                    return true;

            foreach (var i in Canvas ?? Enumerable.Empty<CanvasElement>())
                if (i.Content?.ProcessButton(action) == true)
                    return true;

            return false;
        }

    }
}
