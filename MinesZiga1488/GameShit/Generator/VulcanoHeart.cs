using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Generator
{
    public class VulcanoHeart : Cell
    {
        public VulcanoHeart(int x,int y) : base(x,y,31)
        {
            cells = new List<VulcanoCell>();
            cells.Add(new VulcanoCell(x + 1, y,this,1));
            cells.Add(new VulcanoCell(x - 1, y, this,1));
            cells.Add(new VulcanoCell(x, y + 1, this,1));
            cells.Add(new VulcanoCell(x, y - 1, this,1));

        }
        public override void Update()
        {
               for (int i = 0;i < cells.Count;i++)
            {
                cells[i].Update();
            }
        }
        public List<VulcanoCell> cells;
        
    }
}
