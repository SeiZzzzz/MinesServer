namespace MinesServer.GameShit
{
    public class Cell
    {
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
        public string name;
        public byte type;
        public int fall_damage;
        public int durability;
        public int damage;
        public bool can_place_over;
        public bool is_diggable;
        public bool is_destructible;
        public bool isPickable;
        public bool isSand;
        public bool isBoulder;
        public bool isEmpty;
    }
}
