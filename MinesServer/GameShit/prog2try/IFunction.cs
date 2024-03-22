using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.prog2try
{
    public interface IFunction
    {
        public int current { get; set; }
        public PAction Next { get {
                var c = actions[current];
                current++;
                if (actions.Count > current)
                {
                    current = 0;
                }
                return c;
            } }
        public static IFunction operator +(IFunction a, PAction b)
        {
            a.actions.Add(b);
            return a;
        }
        public List<PAction> actions { get; init; }
    }
}
