using Microsoft.EntityFrameworkCore;
using MinesServer.Enums;
using MinesServer.GameShit.Skills;
using System.Numerics;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public PlayerSkills skillslist { get; set; }
        public PFunction father { get; set; }
        public PAction(ActionType t)
        {
            type = t;
        }
        public PAction(ActionType t, string label)
        {
            this.label = label; type = t;
        }
        public int delay = 0;
        public string label;
        public ActionType type;
        private void Check(Player p,Func<int,int,bool> func)
        {
            var x = p.x;
            var y = p.y;
            if (father.startoffset != default)
            {
                x += father.startoffset.x;
                y += father.startoffset.y;
            }
            else
            {
                x += p.programsData.shiftX + p.programsData.checkX;
                y += p.programsData.shiftY + p.programsData.checkY;
            }
            p.programsData.checkX = 0;p.programsData.checkY = 0;p.programsData.shiftX = 0;p.programsData.shiftY = 0;
            if (World.W.ValidCoord(x, y))
            {
                if (father.state == null)
                {
                    father.state = func(x, y);
                    return;
                }
                father.state = father.laststateaction switch
                {
                    null => func(x, y),
                    ActionType.Or => (bool)father.state || func(x, y),
                    ActionType.And => (bool)father.state && func(x, y)
                };
            }
        }
        public int moveskop(Player p)
        {
            var speed = 150;
            var todoor = 0;
            foreach (var c in p.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnMove, p))
                {
                    if (c.type == SkillType.Movement)
                    {
                        speed -= (int)c.GetEffect() / 2;
                    }
                    if (c.type == SkillType.RoadMovement)
                    {
                        todoor = (int)c.GetEffect() / 10;
                    }
                }
            }
            return speed - todoor;
        }
        public object? Execute(Player p, ref bool? template)
        {
            switch (type)
            {
                case ActionType.MoveDown:
                    delay = moveskop(p);
                    if (p.Move(p.x, p.y + 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveUp:
                    delay = moveskop(p);
                    if (p.Move(p.x, p.y - 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveRight:
                    delay = moveskop(p);
                    if (p.Move(p.x + 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveLeft:
                    delay = moveskop(p);
                    if (p.Move(p.x - 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveForward:
                    delay = moveskop(p);
                    if (p.Move((int)p.GetDirCord().X, (int)p.GetDirCord().Y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.RotateDown:
                    delay = moveskop(p);
                    p.Move(p.x, p.y, 0);
                    break;
                case ActionType.RotateUp:
                    delay = moveskop(p);
                    p.Move(p.x, p.y, 2);
                    break;
                case ActionType.RotateLeft:
                    delay = moveskop(p);
                    p.Move(p.x, p.y, 3);
                    break;
                case ActionType.RotateRight:
                    delay = moveskop(p);
                    p.Move(p.x, p.y, 1);
                    break;
                case ActionType.RotateLeftRelative:
                    delay = moveskop(p);
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
                    delay = moveskop(p);
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
                    delay = moveskop(p);
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
                case ActionType.ShiftUp:
                    p.programsData.shiftY--;
                    break;
                case ActionType.ShiftDown:
                    p.programsData.shiftY++;
                    break;
                case ActionType.ShiftRight:
                    p.programsData.shiftX++;
                    break;
                case ActionType.ShiftLeft:
                    p.programsData.shiftX--;
                    break;
                case ActionType.ShiftForward:
                    p.programsData.shiftX += p.dir switch
                    {
                        1 => 1,
                        3 => -1,
                        _ => 0
                    };
                    p.programsData.shiftY += p.dir switch
                    {
                        0 => -1,
                        2 => 1,
                        _ => 0
                    };
                    break;
                case ActionType.CheckRightRelative:
                    p.programsData.checkX = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    break;
                case ActionType.CheckLeftRelative:
                    p.programsData.checkX = p.dir switch
                    {
                        0 => -1,
                        2 => 1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        1 => 1,
                        3 => -1,
                        _ => 0
                    };
                    break;
                case ActionType.CheckForward:
                    p.programsData.checkX = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;
                case ActionType.CheckUp:
                    p.programsData.checkX = 0;
                    p.programsData.checkY = -1;
                    break;
                case ActionType.CheckDown:
                    p.programsData.checkX = 0;
                    p.programsData.checkY = 1;
                    break;
                case ActionType.CheckRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = 0;
                    break;
                case ActionType.CheckLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = 0;
                    break;
                case ActionType.CheckUpLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = -1;
                    break;
                case ActionType.CheckUpRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = -1;
                    break;
                case ActionType.CheckDownLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = 1;
                    break;
                case ActionType.CheckDownRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = 1;
                    break;
                case ActionType.IsHpLower100:
                    Check(p, (x, y) => p.health.HP < p.health.MaxHP);
                    break;
                case ActionType.IsHpLower50:
                    Check(p, (x, y) => p.health.HP < p.health.MaxHP / 2);
                    break;
                case ActionType.IsCrystal:
                    Check(p, (x, y) => World.isCry(World.GetCell(x, y)));
                    break;
                case ActionType.IsEmpty:
                    Check(p, (x, y) => World.GetProp(x, y).isEmpty);
                    break;
                case ActionType.IsNotEmpty:
                    Check(p, (x, y) => !World.GetProp(x, y).isEmpty);
                    break;
                case ActionType.IsGreenBlock:
                    break;
                case ActionType.RunSub or ActionType.RunState or ActionType.RunFunction:
                    return label;
                case ActionType.ReturnFunction:
                    return father.state;
                case ActionType.RunIfTrue:
                    if (father.state.HasValue && !father.state.Value)
                    {
                        label = "";
                    }
                    father.state = null;
                    return label;
                case ActionType.RunIfFalse:
                    if (father.state.HasValue && father.state.Value)
                    {
                        label = "";
                    }
                    father.state = null;
                    return label;
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
