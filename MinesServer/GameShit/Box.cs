using MinesServer.Enums;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public class Box
    {
        public int x { get; set; }
        public int y { get; set; }
        [NotMapped]
        public long[] bxcrys = new long[6];
        public Box? GetBox(int x,int y)
        {
            if (!World.W.ValidCoord(x,y))
            {
                return null;
            }
            using var db = new DataBase();
            return db.boxes.FirstOrDefault(t => t.x == x && t.y == y);
        }
        public static void BuildBox(int x,int y, long[] cry, Player p)
        {
            var cell = World.W.map.GetCell(x, y);
            if (!(World.GetProp(cell).isEmpty && World.GetProp(cell).can_place_block))
            {
                return;
            }
            var box = new Box();
            for (int i = 0; i < 6; i++)
            {
                long remcry = cry[i];
                if (p == null)
                {
                   box.bxcrys[i] = remcry;
                }
                else if (p.crys.RemoveCrys(i, remcry))
                {
                    box.bxcrys[i] = remcry;
                }
            }
            if (box.bxcrys.Sum() <= 0)
            {
                return;
            }
            box.y = y;box.x = x;
            using (var db = new DataBase())
            {
                db.boxes.Add(box);
            }
        }
        public long this[CrystalType crystal]
        {
            get => bxcrys[(int)crystal];
            set => bxcrys[(int)crystal] = value;
        }
        public long ze
        {
            get { return bxcrys[0]; }
            set { bxcrys[0] = value; }
        }

        public long cr
        {
            get { return bxcrys[1]; }
            set { bxcrys[1] = value; }
        }

        public long si
        {
            get { return bxcrys[2]; }
            set { bxcrys[2] = value; }
        }

        public long be
        {
            get { return bxcrys[3]; }
            set { bxcrys[3] = value; }
        }

        public long fi
        {
            get { return bxcrys[4]; }
            set { bxcrys[4] = value; }
        }

        public long go
        {
            get { return bxcrys[5]; }
            set { bxcrys[5] = value; }
        }
    }
}
