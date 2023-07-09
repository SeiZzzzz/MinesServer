namespace MinesServer.GameShit
{
    public class Cell
    {
        public Cell(int x,int y,byte type)
        {
            CellsSerializer.cells[type].CreateNormalCell(this);
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
