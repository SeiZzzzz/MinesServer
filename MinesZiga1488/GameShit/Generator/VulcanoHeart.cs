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
            World.W.SetCell(x + 1, y, 91);
        }
        public override void Update()
        {

        }
        
    }
}
