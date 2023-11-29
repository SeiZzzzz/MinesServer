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
        }
        public virtual int id { get; set; }
        public virtual int x { get; set; }
        public virtual int y { get; set; }
        public virtual int cid { get; set; }
        [NotMapped]
        public virtual int off { get; set; }
        public PackType type { get; set; }
        public int ownerid { get; set; }
        public abstract Window? GUIWin(Player p);
        public virtual void Build()
        {
            World.AddPack(x, y, this);
        }
        public virtual void Update()
        {

        }
    }
}
