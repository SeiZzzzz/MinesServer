using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public class Fallable : Cell
    {
        public Fallable(int x, int y,byte type) : base(x, y,type)
        {

        }
        public override void Update()
        {
            base.Update();
        }
    }
}
