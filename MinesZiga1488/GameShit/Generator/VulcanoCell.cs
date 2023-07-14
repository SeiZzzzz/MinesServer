namespace MinesServer.GameShit.Generator
{
    public class VulcanoCell : Cell
    {
        public VulcanoCell(int x, int y, VulcanoHeart father, float heat = 0) : base(x, y, 91)
        {
            World.W.SetCell(x, y, 91);
            this.father = father;
            if (heat != 0)
            {
                Gen.UpdateHeat(x, y, heat, father.id);
            }
        }
        public VulcanoHeart father;
        public List<VulcanoCell> HeatTerritory(List<VulcanoCell> l)
        {
            var r = new Random();
            if (!World.W.ValidCoord(x,y))
            {
                l.Remove(l.First(cell => cell.x == x && cell.y == y));
                return l;
            }
            if (Gen.heatmap[0, this.x + this.y * Gen.height] == -1)
            {
                l.Remove(l.First(cell => cell.x == x && cell.y == y));
                return l;
            }
            if (Gen.heatmap[1,this.x + this.y * Gen.height] > 1 && CountAnother(x, y,father.id) > 1)
            {
                World.W.SetCell(this.x, this.y, 117);
                Gen.heatmap[0, this.x + this.y * Gen.height] = -1;
                l.Remove(l.First(cell => cell.x == x && cell.y == y));
                return l;
            }
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = this.x + x;
                    var ty = this.y + y;
                    if (!World.W.ValidCoord(tx, ty) || (tx == this.x && ty == this.y))
                    {
                        continue;
                    }
                    if (Gen.heatmap[0, this.x + this.y * Gen.height] == -1)
                    {
                        continue;
                    }
                    if (CountAnother(tx,ty,father.id) > 9 && containsAnother(tx,ty,father.id))
                    {
                        l.Add(new VulcanoCell(tx, ty, father, Gen.GetHeat(tx, ty)));
                        Gen.UpdateHeat(tx, ty, (float)(HCount(tx, ty) * r.NextDouble()), father.id);
                        continue;
                    }
                    if (Gen.GetHeat(tx, ty) > 0f)
                    {
                        if (!AlreadyCell(tx, ty) && r.Next(0, 100) < HCount(tx, ty) && World.GetProp(World.W.GetCell(tx, ty)).is_destructible && World.W.GetCell(tx, ty) != 91)
                        {
                            l.Add(new VulcanoCell(tx, ty, father, Gen.GetHeat(tx, ty)));
                        }
                        continue;
                    }
                    if (Gen.heatmap[0, this.x + this.y * Gen.height] != -1 && !containsAnother(tx,ty, father.id))
                    {
                        Gen.UpdateHeat(tx, ty, (float)(HCount(tx, ty) * r.NextDouble()), father.id);
                    }

                }
            }
            return l;
        }
        public static bool containsAnother(int x, int y,float id)
        {
            if (!World.W.ValidCoord(x, y))
            {
                return false;
            }
            if (Gen.heatmap[0, x + y * Gen.height] == 0)
            {
                return false;
            }
            if (Gen.heatmap[0, x + y * Gen.height] == -1)
            {
                return false;
            }
            return Gen.heatmap[0, x + y * Gen.height] != id;
        }
        private bool AlreadyCell(int x, int y)
        {
            var j = father.cells.FirstOrDefault(cell => cell.x == x && cell.y == y) != default(VulcanoCell);
            return j;
        }
        public static int CountAnother(int cx, int cy, float id)
        {
            int c = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = cx + x;
                    var ty = cy + y;
                    if (containsAnother(tx, ty, id))
                    {
                        c++;
                    }
                }
            }
            return c;
        }
        public static int HCount(int cx, int cy)
        {
            var c = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = cx + x;
                    var ty = cy + y;
                    if (Gen.GetHeat(tx,ty) > 0f)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
    }
}
