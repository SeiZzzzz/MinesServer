namespace MinesServer.GameShit
{
    public class Cell
    {
        public Cell(int x, int y, byte type)
        {
            this.x = x;this.y = y; this.type = type;
        }
        public static Cell CreateCell(int x, int y,byte type)
        {
            if (CellsSerializer.cells[type].isEmpty)
            {
                return CellsSerializer.cells[type].SetCellProp(new Road(x, y, type));
            }
            if (CellsSerializer.cells[type].isFallable)
            {
                return CellsSerializer.cells[type].SetCellProp(new Fallable(x, y, type));
            }
            return CellsSerializer.cells[type].SetCellProp(new Cell(x,y,type));
        }
        public virtual void Update()
        {

        }
        public bool isFallable;
        public bool isCry;
        public int durability;
        public int damage;
        public bool isEmpty;
        public byte type;
        public int x;
        public int y;
    }
}
