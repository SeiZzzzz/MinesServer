namespace MinesServer.GameShit
{
    public class Cell
    {
        public Cell(int x, int y, byte type)
        {
            this.x = x; this.y = y; this.type = type;
        }
        public Cell(byte type)
        {
            this.type = type;
        }
        public Cell()
        {

        }
        public virtual void Update()
        {

        }
        public bool isPackPart;
        public bool is_destructible_by_block;
        public bool can_place_boom;
        public bool can_place_road;
        public int fall_damage;
        public bool is_destructible_byboom;
        public bool is_destructible;
        public bool isPickable;
        public bool isFallable;
        public bool isCry;
        public int durability;
        public int damage;
        public bool isEmpty;
        public byte type;
        [NonSerialized]
        public int x;
        [NonSerialized]
        public int y;
    }
}
