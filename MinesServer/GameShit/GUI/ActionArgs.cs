using System.Collections.Generic;

namespace MinesServer.GameShit.GUI
{
    public class ActionArgs
    {
        public string? Input { get; set; }
        public Dictionary<string, string>? RichList { get; set; }
        public bool[]? PaintGrid { get; set; }
        public long[]? CrystalSliders { get; set; }
    }
}
