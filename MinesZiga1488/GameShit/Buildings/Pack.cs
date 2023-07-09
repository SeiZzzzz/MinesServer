using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Pack
    {
        public Pack()
        {

        }
        public Pack(int x,int y,int ownerid,Packs type)
        {
            this.x = x;this.y = y;this.ownerid = ownerid; this.type = type;
        }
        public int x { get; set; }
        public int y { get; set; }
        public Packs type { get; set; }
        public int ownerid { get; set; }
        public virtual void Update()
        {

        }
    }
}
