using MinesServer.GameShit.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Crafter : Pack, IDamagable
    {
        public DateTime brokentimer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float charge { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int hp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Destroy(Player p)
        {
            throw new NotImplementedException();
        }

        public override Window? GUIWin(Player p)
        {
            throw new NotImplementedException();
        }
    }
}
