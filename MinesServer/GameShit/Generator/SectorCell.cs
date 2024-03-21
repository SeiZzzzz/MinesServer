namespace MinesServer.GameShit.Generator
{
    public class SectorCell
    {
        public double value;
        public int sector;
        public (int, int) pos;
        public CellType type = CellType.Empty;
        public bool check = false;
    }
}
