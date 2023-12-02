using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public interface IDamagable
    {
        public void Damage(int i)
        {
            if (ownerid == 0 || hp == 0)
            {
                return;
            }
            if (hp - i >= 0)
            {
                hp -= i;
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
        public abstract void Destroy(Player p);
        public void SendBrokenEffect()
        {
            World.W.GetChunk(x, y).SendFx(x, y, 12);
        }
        public DateTime brokentimer { get; set; }
        public int ownerid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int hp { get; set; }
    }
}
