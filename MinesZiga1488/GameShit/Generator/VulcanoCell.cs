using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Generator
{
    public class VulcanoCell : Cell
    {
        public VulcanoCell(int x, int y,VulcanoHeart father,float heat = 0) : base(x, y, 91)
        {
            World.W.SetCell(x, y, 91);
            this.father = father;
            if (heat != 0)
            {
                Gen.UpdateHeat(x, y, heat);
            }
        }
        public VulcanoHeart father;
        public override void Update()
        {
            HeatTerritory();
        }
        private void HeatTerritory()
        {
            for (int x = -1;x <= 1;x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = this.x + x;
                    var ty = this.y + y;
                    if (Gen.GetHeat(tx,ty) > 0.4f)
                    {
                        father.cells.Add(new VulcanoCell(tx, ty,father, Gen.GetHeat(tx,ty)));
                        continue;
                    }
                    Gen.UpdateHeat(tx, ty, HCount(tx,ty));
                }
            }
        }
        private int HCount(int cx,int cy)
        {
            var c = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var tx = cx + x;
                    var ty = cy + y;
                    if (Gen.GetHeat(tx,ty) > 0.2f)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
    }
}
