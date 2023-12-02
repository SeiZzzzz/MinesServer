using MinesServer.GameShit.GUI;

namespace MinesServer.GameShit.Buildings
{
    public class Storage : Pack,IDamagable
    {
        public long this[int index]
        {
            get => crysinside[index];
            set => crysinside[index] = value;
        }
        public DateTime brokentimer { get; set; }
        public int hp { get; set; }

        public long[] crysinside = new long[6];
        public Storage()
        {

        }
        public Storage(int x,int y,int ownerid) : base(x,y,ownerid,PackType.Storage)
        {

        }
        public void Destroy(Player p)
        {

        }
        public override Window? GUIWin(Player p)
        {
            return null;
        }
    }
}
