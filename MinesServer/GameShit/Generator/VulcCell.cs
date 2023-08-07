using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Generator
{
    public static class Ext
    {
        public static void Remove<T>(this Queue<T> queue, T itemToRemove) where T : class
        {
            var list = queue.ToList();
            queue.Clear();
            foreach (var item in list)
            {
                if (item == itemToRemove)
                    continue;

                queue.Enqueue(item);
            }
        }
    }
    public class VulcCell
    {
        public VulcCell(int _x, int _y, Heart _father, float startheat = 0.2f)
        {
            x = _x; y = _y; father = _father;
            Gen.THIS.map[x + y * Gen.height] = (startheat, father.id, false);
        }
        public void Heat()
        {
            var r = new Random();
            for (int px = -1; px <= 1; px++)
            {
                for (int py = -1; py <= 1; py++)
                {
                    var nx = x + px;
                    var ny = y + py;
                    if (!World.W.ValidCoord(nx, ny) || (nx == x && ny == y) || Gen.THIS.map[nx + ny * Gen.height].Item2 == -1)
                    {
                        continue;
                    }
                    if (Gen.THIS.map[nx + ny * Gen.height].Item1 > (r.Next(20,50) * 0.01f) && Gen.THIS.map[nx + ny * Gen.height].Item2 == 0 && r.Next(0,100) < nearbyally(x,y))
                    {
                        father.update.Enqueue(new VulcCell(nx, ny, father, Gen.THIS.map[nx + ny * Gen.height].Item1));
                    }
                    Gen.HeatUp(nx, ny, Gen.THIS.map[x + y * Gen.height].Item1 * (r.Next(2, 5) * 0.01f),father.id);
                }
            }
        }
        public void Diffusion()
        {
            var r = new Random();
            for (int px = -1; px <= 1; px++)
            {
                for (int py = -1; py <= 1; py++)
                {
                    var nx = x + px;
                    var ny = y + py;
                    if (!World.W.ValidCoord(nx, ny) || (nx == x && ny == y) || Gen.THIS.map[nx + ny * Gen.height].Item2 == -1)
                    {
                        continue;
                    }
                    if (Gen.THIS.map[nx + ny * Gen.height].Item2 != 0 && Gen.THIS.map[nx + ny * Gen.height].Item2 != father.id && nearbyally(nx,ny) > nearbyenemy(nx,ny) && r.Next(0,100) > 60)
                    {
                        var vulc = Gen.THIS.vulcs[Gen.THIS.map[nx + ny * Gen.height].Item2 - 1];
                        Ext.Remove(vulc.update, vulc.update.FirstOrDefault(c => c.x == nx && c.y == ny));
                        father.update.Enqueue(new VulcCell(nx, ny, father, Gen.THIS.map[nx + ny * Gen.height].Item1));
                        if (World.GetProp(World.W.GetCell(nx, ny)).is_destructible)
                        {
                            World.W.SetCell(nx, ny, 90);
                        }
                    }
                }
            }
        }
        public int nearbyally(int cx,int cy)
        {
            var c = 0;
            for (int px = -1; px <= 1; px++)
            {
                for (int py = -1; py <= 1; py++)
                {
                    var nx = cx + px;
                    var ny = cy + py;
                    if (!World.W.ValidCoord(nx, ny) || (nx == cx && ny == cy) || Gen.THIS.map[nx + ny * Gen.height].Item2 == -1)
                    {
                        continue;
                    }
                    if (Gen.THIS.map[nx + ny * Gen.height].Item2 == father.id && Gen.THIS.map[nx + ny * Gen.height].Item1 > 0.2f)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
        public int nearbyenemy(int cx, int cy)
        {
            var c = 0;
            for (int px = -1; px <= 1; px++)
            {
                for (int py = -1; py <= 1; py++)
                {
                    var nx = cx + px;
                    var ny = cy + py;
                    if (!World.W.ValidCoord(nx, ny) || (nx == cx && ny == cy) || Gen.THIS.map[nx + ny * Gen.height].Item2 == -1)
                    {
                        continue;
                    }
                    if (Gen.THIS.map[nx + ny * Gen.height].Item2 != 0 && Gen.THIS.map[nx + ny * Gen.height].Item2 != father.id && Gen.THIS.map[nx + ny * Gen.height].Item1 > 0.2f)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
        public bool CanBeWall
        {
            get
            {
                var c = 0;
                var another = 0;
                for (int px = -1; px <= 1; px++)
                {
                    for (int py = -1; py <= 1; py++)
                    {
                        var nx = x + px;
                        var ny = y + py;
                        if (!World.W.ValidCoord(nx, ny) || (nx == x && ny == y))
                        {
                            continue;
                        }
                        if (Gen.THIS.map[nx + ny * Gen.height].Item2 != 0)
                        {
                            c++;
                        }
                        if (Gen.THIS.map[nx + ny * Gen.height].Item2 != father.id && Gen.THIS.map[nx + ny * Gen.height].Item2 != -1)
                        {
                            another++;
                        }
                    }
                }
                return c == 8 && another > 2;
            }
        }
        public bool Closed
        {
            get
            {
                var c = 0;
                for (int px = -1; px <= 1; px++)
                {
                    for (int py = -1; py <= 1; py++)
                    {
                        var nx = x + px;
                        var ny = y + py;
                        if (World.W.ValidCoord(nx,ny))
                        {
                            if(Gen.THIS.map[nx + ny * Gen.height].Item2 != 0 && Gen.THIS.map[nx + ny * Gen.height].Item2 == father.id && (nx != x && ny != y))
                            {
                                c++;
                                continue;
                            }
                            else if (Gen.THIS.map[nx + ny * Gen.height].Item2 == -1)
                            {
                                c++;
                                continue;
                            }
                            continue;
                        }
                        c++;
                    }
                }
                return c >= 8;
            }
        }
        public int x;public int y;public Heart father;
    }
}
