namespace MinesServer.GameShit.Buildings
{
    public interface IDamagable
    {
        public void Damage(int i)
        {
            if (charge - 100 > 0)
            {
                charge -= 100;
            }
            else
            {
                charge = 0;
            }
            if (ownerid == 0 || hp == 0)
            {
                return;
            }
            if (hp - i >= 0)
            {
                hp -= i;
                if (hp == 0)
                {
                    brokentimer = DateTime.Now;
                }
                return;
            }
            hp = 0;
            brokentimer = DateTime.Now;
        }
        public bool CanDestroy()
        {
            if (DateTime.Now - brokentimer < TimeSpan.FromHours(8))
            {
                return false;
            }
            return hp == 0;
        }
        public bool NeedEffect()
        {
            return hp == 0;
        }
        public abstract void Destroy(Player p);
        public void SendBrokenEffect()
        {
            World.W.GetChunk(x, y).SendFx(x, y, 12);
        }
        public DateTime brokentimer { get; set; }
        public int ownerid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public float charge { get; set; }
        public int hp { get; set; }
    }
}
