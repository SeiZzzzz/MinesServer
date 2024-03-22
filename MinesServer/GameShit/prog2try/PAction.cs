using MinesServer.GameShit.Programmator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.prog2try
{
    public struct PAction
    {
        public PAction(ActionType t)
        {
            type = t;
        }
        public PAction(ActionType t,string label)
        {
            this.label = label;type = t;
        }
        public string label;
        public ActionType type;
        public void Execute(Player p)
        {

        }
    }
}
