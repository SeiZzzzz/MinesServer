using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class PackHeart : Cell
    {
        public PackHeart(int x, int y,Pack father) : base(x,y,37)
        {
            this.father = father;
        }
        public Pack father;
    }
}
