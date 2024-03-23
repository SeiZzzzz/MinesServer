namespace MinesServer.GameShit.GChat
{
    public readonly record struct GLine(Player player, string message);
    public static class GlobalChat
    {
        public static List<GLine> lines { get; } = new();
        public static void AddMessage(Player p, string msg)
        {
            lines.Add(new GLine(p, msg));
        }
    }
}
