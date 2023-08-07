using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public struct SectorCell
    {
        public SectorCell(int x,int y)
        {
            this.x = x;this.y = y;
        }
        public static bool operator ==(SectorCell left, SectorCell right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(SectorCell left, SectorCell right)
        {
            return !left.Equals(right);
        }
        public int x;
        public int y;
    }
}
