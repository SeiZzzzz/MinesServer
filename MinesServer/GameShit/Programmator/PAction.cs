using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public PFunction father;
        public PAction(ActionType t)
        {
            type = t;
        }
        public PAction(ActionType t, string label)
        {
            this.label = label; type = t;
        }
        public void SetFather(PFunction f) => father = f;
        public int delay;
        public string label;
        public ActionType type;
        private void Check(Player p,Func<int,int,bool> func)
        {
            if (father.state == null)
            {
                father.state = func(p.x,p.y);
                return;
            }
            father.state = father.laststateaction switch
            {
                null => func(p.x,p.y),
                ActionType.Or => (bool)father.state || func(p.x, p.y),
                ActionType.And => (bool)father.state && func(p.x,p.y)
            };
        }
        public object? Execute(Player p, ref bool? template)
        {
            switch (type)
            {
                case ActionType.MoveDown:
                    delay = p.pause / 100;
                    if (p.Move(p.x, p.y + 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveUp:
                    delay = p.pause / 100;
                    if (p.Move(p.x, p.y - 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveRight:
                    delay = p.pause / 100;
                    if (p.Move(p.x + 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveLeft:
                    delay = p.pause / 100;
                    if (p.Move(p.x - 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveForward:
                    delay = p.pause / 100;
                    if (p.Move((int)p.GetDirCord().X, (int)p.GetDirCord().Y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.RotateDown:
                    delay = p.pause / 100;
                    p.Move(p.x, p.y, 0);
                    break;
                case ActionType.RotateUp:
                    delay = p.pause / 100;
                    p.Move(p.x, p.y, 2);
                    break;
                case ActionType.RotateLeft:
                    delay = p.pause / 100;
                    p.Move(p.x, p.y, 3);
                    break;
                case ActionType.RotateRight:
                    delay = p.pause / 100;
                    p.Move(p.x, p.y, 1);
                    break;
                case ActionType.RotateLeftRelative:
                    delay = p.pause / 100;
                    var dirl = p.dir switch
                    {
                        0 => 3,
                        1 => 0,
                        2 => 1,
                        3 => 2
                    };
                    p.Move(p.x, p.y, dirl);
                    break;
                case ActionType.RotateRightRelative:
                    delay = p.pause / 100;
                    var dirr = p.dir switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 0
                    };
                    p.Move(p.x, p.y, dirr);
                    break;
                case ActionType.RotateRandom:
                    delay = p.pause / 100;
                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    p.Move(p.x, p.y, rand.Next(4));
                    break;
                case ActionType.Dig:
                    delay = 250;
                    p.Bz();
                    break;
                case ActionType.BuildBlock:
                    break;
                case ActionType.BuildPillar:
                    break;
                case ActionType.BuildRoad:
                    break;
                case ActionType.BuildMilitaryBlock:
                    break;
                case ActionType.Geology:
                    delay = 250;
                    p.Geo();
                    break;
                case ActionType.Heal:
                    p.health.Heal();
                    break;
                case ActionType.IsHpLower100:
                    Check(p, (x, y) => p.health.HP < p.health.MaxHP);
                    break;
                case ActionType.IsHpLower50:
                    Check(p, (x, y) => p.health.HP < p.health.MaxHP / 2);
                    break;
                case ActionType.IsEmpty:
                    break;
                case ActionType.IsNotEmpty:
                    break;
                case ActionType.IsGreenBlock:
                    break;
                case ActionType.RunSub:
                    return label;
                case ActionType.ReturnState:
                    return father.state;
                case ActionType.RunIfTrue or ActionType.RunIfFalse:
                    var resultif = template;
                    template = null;
                    return resultif;
                case ActionType.Or or ActionType.And:
                    father.laststateaction = type;
                    break;
                case ActionType.GoTo:
                    return label;
            }
            return null;
        }
    }
}
