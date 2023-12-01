using MinesServer.GameShit.GUI;
using System.Runtime.CompilerServices;

namespace MinesServer.GameShit.Buildings
{
    public class Storage : Pack
    {
        public int hp { get; set; }
        public long this[int index]
        {
            get => crysinside[index];
            set => crysinside[index] = value;
        }

        public long[] crysinside = new long[6];
        public Storage()
        {

        }
        public Storage(int x,int y,int ownerid) : base(x,y,ownerid,PackType.Storage)
        {

        }

        public override Window? GUIWin(Player p)
        {
            return null;
        }
    }
}
