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
        public List<VulcanoCell> HeatTerritory()
        {
            var r = new Random();
            var j = new List<VulcanoCell>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = this.x + x;
                    var ty = this.y + y;
                    if (!World.W.ValidCoord(tx, ty) || Gen.heatmap[0, tx + ty * Gen.height] == -1)
                    {
                        continue;
                    }
                    if (Gen.GetHeat(tx, ty) > 0.4f)
                    {
                        if (!AlreadyCell(tx, ty) && r.Next(0, 100) < HCount(tx, ty) && World.GetProp(World.W.GetCell(tx, ty)).is_destructible)
                        {
                            j.Add(new VulcanoCell(tx, ty, father, Gen.GetHeat(tx, ty)));
                        }
                        continue;
                    }
                    Gen.UpdateHeat(tx, ty, (float)(HCount(tx, ty) * r.NextDouble()), father.id);

                }
            }
            return j;
        }
        public bool containsAnother(int x, int y)
        {
            if (!World.W.ValidCoord(x, y))
            {
                return false;
            }
            return Gen.heatmap[0, x + y * Gen.height] != father.id;
        }
        private bool AlreadyCell(int x, int y)
        {
            var j = father.cells.FirstOrDefault(cell => cell.x == x && cell.y == y) != default(VulcanoCell);
            return j;
        }
        public int CountAnother(int cx, int cy)
        {
            int c = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = cx + x;
                    var ty = cy + y;
                    if (containsAnother(tx, ty))
                    {
                        c++;
                    }
                }
            }
            return c;
        }
        private int HCount(int cx, int cy)
        {
            var c = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = cx + x;
                    var ty = cy + y;
                    if (Gen.GetHeat(tx, ty) > 0.2f)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
    }
}
