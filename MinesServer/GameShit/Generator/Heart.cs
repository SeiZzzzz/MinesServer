using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Generator
{
    public class Heart
    {
        public Heart(int _x, int _y, int _id)
        {
            x = _x; y = _y; id = _id;
            Gen.THIS.map[x + y * Gen.height] = (1, id, true);
            foreach (var rofl in dirs)
            {
                if (World.W.ValidCoord(x + rofl.Item1, y + rofl.Item2))
                {
                    update.Enqueue(new VulcCell(x + rofl.Item1, y + rofl.Item2, this,1));
                }
            }
        }
        public void Update()
        {
            if (update.Count == 0 && !cry)
            {
                Console.WriteLine("cr");
                Crystalizing();
                cry = true;
            }    
            for (int i = 0;i < update.Count;i++)
            {
                var c = update.Dequeue();
                sectorcells.Add(new SectorCell(c.x,c.y));
                if (Gen.THIS.map[c.x + c.y * Gen.height].Item2 == -1)
                {
                    continue;
                }
                c.Heat();
                c.Diffusion();
                if (c.CanBeWall && new Random().Next(0,100) > 50)
                {
                    if (World.GetProp(World.W.GetCell(c.x, c.y)).is_destructible)
                    {
                        World.W.SetCell(c.x, c.y, 117);
                    }
                    Gen.THIS.map[c.x + c.y * Gen.height] = (0, -1, false);
                    var cc = sectorcells.FirstOrDefault(p => p.x == c.x && p.y == c.x);
                    if (cc != default)
                    {
                        sectorcells.Remove(cc);
                    }
                    continue;
                }
                if (Gen.THIS.map[c.x + c.y * Gen.height].Item2 == -1)
                {
                    continue;
                }
                if (!c.Closed)
                {
                    update.Enqueue(c);
                }
            }
        }
        public void Crystalizing()
        {
            var r = new Random();
            foreach(var c in sectorcells)
            {
                var d = World.W.CrysTypeByDepth(c.y);

                if (World.GetProp(World.W.GetCell(c.x, c.y)).is_destructible)
                {
                    Console.WriteLine((byte)d[r.Next(0, d.Count)]);
                    World.W.SetCell(c.x, c.y, (byte)d[r.Next(0,d.Count)]);
                }
            }
        }
        public bool cry = false;
        public List<SectorCell> sectorcells = new List<SectorCell>();
        private (int, int)[] dirs = {(1,0),(0,1),(-1,0),(0,-1) };
        public Queue<VulcCell> update = new Queue<VulcCell>();
        public int id; public int x; public int y;
    }
}
