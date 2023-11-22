using MinesServer.GameShit.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Up : Pack
    {
        public Up(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Up) { }
        public override Window? GUIWin(Player p)
        {
            return null;
        }
    }
}
