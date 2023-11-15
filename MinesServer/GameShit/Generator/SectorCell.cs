namespace MinesServer.GameShit.Generator
{
    public class SectorCell
    {
        public float value;
        public int sector;
        public (int, int) pos;
        public CellType type = CellType.Empty;
        public bool check = false;
    }
}
