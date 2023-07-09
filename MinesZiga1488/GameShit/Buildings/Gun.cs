using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Gun : Pack
    {
        public Gun(int x, int y, int ownerid, Packs type) : base(x, y, ownerid, type)
        {

        }
        public override void Update()
        {
            base.Update();
        }
    }
}
