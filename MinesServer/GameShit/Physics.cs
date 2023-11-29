using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public static class Physics
    {
        static Random r = new Random();
        public static bool Boulder(int x,int y)
        {
            return false;
        }
        public static bool Sand(int x,int y)
        {
            return false;
            if (World.GetProp(World.GetCell(x + 1,y + 1)).isEmpty)
            {
                
            }
        }
        public static bool Alive(int x,int y)
        {
            return false;
        }
    }
}
