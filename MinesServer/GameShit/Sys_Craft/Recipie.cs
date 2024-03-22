namespace MinesServer.GameShit.SysCraft
{
    public struct Recipie
    {
        public required RC result { get; init; }
        public RC[]? costres { get; init; }
        public RC[]? costcrys { get; init; }
        public required int time { get; init; }
    }
    public struct RC
    {
        public RC(int i, int n)
        {
            id = i; num = n;
        }
        public int id;
        public int num;
    }
}
