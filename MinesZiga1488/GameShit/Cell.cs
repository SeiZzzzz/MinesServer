namespace MinesServer.GameShit
{
    public class Cell
    {
        public Cell(int x, int y, byte type)
        {
            this.x = x; this.y = y; this.type = type;
        }
        public virtual void Update()
        {

        }
        [NonSerialized]
        public bool islocked;
        public bool isFallable;
        public bool isCry;
        public int durability;
        public int damage;
        public byte type; public int x; public int y;
    }
}
