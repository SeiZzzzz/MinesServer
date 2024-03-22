using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.prog2try
{
    public class PFunction
    {
        public int current { get; set; } = 0;
        public PAction Next
        {
            get
            {
                var c = actions[current];
                current++;
                if (actions.Count > current)
                {
                    current = 0;
                }
                return c;
            }
        }
        public static PFunction operator +(PFunction a, PAction b)
        {
            a.actions.Add(b);
            return a;
        }
        public List<PAction> actions { get; init; } = new();
    }
}
