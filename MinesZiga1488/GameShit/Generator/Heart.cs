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
            if (update.Count == 0)
            {
                Console.WriteLine("sec");
            }    
            for (int i = 0;i < update.Count;i++)
            {
                var c = update.Dequeue();
                sectorcells.Add(new Point(c.x,c.y));
                if (Gen.THIS.map[c.x + c.y * Gen.height].Item2 == -1)
                {
                    continue;
                }
                c.Heat();
                if (c.CanBeWall && new Random().Next(0,100) > 50)
                {
                    if (World.GetProp(World.W.GetCell(c.x, c.y)).is_destructible)
                    {
                        World.W.SetCell(c.x, c.y, 117);
                    }
                    Gen.THIS.map[c.x + c.y * Gen.height] = (0, -1, false);
                    var cc = sectorcells.FirstOrDefault(p => p.X == c.x && p.Y == c.x);
                    if (cc != default(Point))
                    {
                        sectorcells.Remove(cc);
                    }
                    continue;
                }
                if (!c.Closed && Gen.THIS.map[c.x + c.y * Gen.height].Item2 != -1)
                {
                    update.Enqueue(c);
                }
            }
        }
        public List<Point> sectorcells = new List<Point>();
        private (int, int)[] dirs = {(1,0),(0,1),(-1,0),(0,-1) };
        public Queue<VulcCell> update = new Queue<VulcCell>();
        public int id; public int x; public int y;
    }
}
