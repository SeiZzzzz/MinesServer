using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator
{
    public struct ProgAction
    {
        public string label;
        public ActionType type;
        public int delay;
        public ProgAction(ActionType type)
        {
            this.type = type;
        }
        public ProgAction(ActionType type,string text)
        {
            this.type = type;
            label = text;
        }
        public void Execute(Player p)
        {
            switch(type)
            {
                case ActionType.MoveDown:
                    p.Move(p.x, p.y + 1);
                    return;
                case ActionType.None:
                    break;
            }
        }
    }
}
