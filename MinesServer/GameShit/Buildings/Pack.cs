using MinesServer.GameShit.GUI;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Buildings
{
    public abstract class Pack
    {
        public Pack() { }
        public Pack(int x, int y, int ownerid, PackType type)
        {
            this.x = x; this.y = y; this.ownerid = ownerid; this.type = type;
            World.
        }
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int cid { get; set; }
        [NotMapped]
        public int off { get; set; }
        public PackType type { get; set; }
        public int ownerid { get; set; }
        public abstract Window? GUIWin(Player p);
        public virtual void Update()
        {

        }
    }
}
