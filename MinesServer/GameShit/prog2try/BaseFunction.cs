using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.prog2try
{
    public class BaseFunction : IFunction
    {
        public int current { get; set; } = 0;
        public List<PAction> actions { get; init; } = new();
    }
}
