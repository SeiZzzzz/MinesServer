using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Resp : Pack
    {
        public Resp(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Resp) {
            cost = 1000;
            charge = 100;
            maxcharge = 1000;
            using var db = new DataBase();
            db.resps.Add(this);
            db.SaveChanges();
        }
        public void Build()
        {
            World.SetCell(x, y, 37);
            World.SetCell(x + 1, y, 37);
            World.SetCell(x - 1, y, 106);
            World.SetCell(x, y - 1, 106);
            World.SetCell(x, y + 1, 106);
            World.SetCell(x + 1, y + 1, 106);
            World.SetCell(x - 1, y + 1, 106);
            World.SetCell(x + 1, y - 1, 106);
            World.SetCell(x - 1, y - 1, 106);
            World.SetCell(x + 1, y + 2, 106);
            World.SetCell(x - 1, y + 2, 106);
            World.SetCell(x, y + 2, 37);
            for(int xx = x + 2;xx < x + 6;xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35);
                }
            }
        }
        public int charge { get; set; }
        public int maxcharge { get; set; }
        public int cost { get; set; }
        public int cid { get; set; }

        public override GUI.Window? GUIWin(Player p)
        {
            return null;
        }
    }
}
