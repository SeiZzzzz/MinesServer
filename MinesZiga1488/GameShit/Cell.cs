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
        public bool isFallable()
        {
            return type == 91 || type == 222;
        }
        public bool isCrys()
        {
            return type == 1 || type == 2;
        }
        public int durability;
        public int damage;
        public byte type; public int x; public int y;
    }
}
