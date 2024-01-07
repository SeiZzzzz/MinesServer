namespace MinesServer.GameShit.Programmator
{
    public static class Interpreter
    {
        const int lines = 16;
        const int rows = 16;
        const int pages = 16;
        public static void Step(Player player)
        {
            if (DateTime.Now > player.ProgData.NextRun)
            {
                return;
            }
            if (player.ProgData.X == lines)
            {
                player.ProgData.X = player.ProgData.startx;
                player.ProgData.Y = player.ProgData.starty;
            }
            var action = player.ProgData.ActionMatrix[player.ProgData.Y, player.ProgData.X];
            while (action == null && player.ProgData.X < 15)
            {
                player.ProgData.X++;
                action = player.ProgData.ActionMatrix[player.ProgData.Y, player.ProgData.X];
            }
            if (action != null)
            {
                (int X, int Y) labelCoords;
                switch (action.Type)
                {
                    case ActionType.MoveUp:
                        player.Move(player.x, player.y - 1, 2);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.MoveLeft:
                        player.Move(player.x - 1, player.y, 1);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.MoveDown:
                        player.Move(player.x, player.y + 1, 0);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.MoveRight:
                        player.Move(player.x + 1, player.y, 3);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.MoveForward:
                        player.Move((int)player.GetDirCord().X, (int)player.GetDirCord().Y, player.dir);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateUp:
                        player.Move(player.x, player.y, 2);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateLeft:
                        player.Move(player.x, player.y, 1);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateDown:
                        player.Move(player.x, player.y, 0);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateRight:
                        player.Move(player.x, player.y, 3);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateLeftRelative:
                        player.dir = player.dir switch
                        {
                            0 => 3,
                            1 => 0,
                            2 => 1,
                            3 => 2
                        };
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateRightRelative:
                        player.dir = player.dir switch
                        {
                            0 => 1,
                            1 => 2,
                            2 => 3,
                            3 => 0
                        };
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.RotateRandom:
                        var rand = new Random(Guid.NewGuid().GetHashCode());
                        player.dir = rand.Next(4);
                        player.ProgData.AddDelay(50);
                        break;
                    case ActionType.Dig:
                        var x = (int)player.GetDirCord().X;
                        var y = (int)player.GetDirCord().Y;
                        if (World.W.ValidCoord(x, y))
                        {
                            player.Bz();
                        }

                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.BuildBlock:
                        player.Build("G");
                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.BuildPillar:
                        player.Build("O");
                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.BuildRoad:
                        player.Build("R");
                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.BuildMilitaryBlock:
                        player.Build("V");
                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.Geology:
                        player.Geo();
                        player.ProgData.AddDelay(400);
                        break;
                    case ActionType.Heal:
                        player.Heal();
                        player.ProgData.AddDelay(160);
                        break;
                    case ActionType.Label:
                        player.Heal();
                        player.ProgData.AddDelay(160);
                        break;
                    case ActionType.And:
                        player.ProgData.ConditionMode = ActionType.And;
                        break;
                    case ActionType.Or:
                        player.ProgData.ConditionMode = ActionType.Or;
                        break;
                    case ActionType.IsHpLower100:
                        player.ProgData.SetCondition(player.health.HP < player.health.MaxHP);
                        break;
                    case ActionType.IsHpLower50:
                        player.ProgData.SetCondition(player.health.HP < player.health.MaxHP / 2);
                        break;
                    case ActionType.IsGreenBlock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.GreenBlock);
                        break;
                    case ActionType.IsYellowBlock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.YellowBlock);
                        break;
                    case ActionType.IsRedBlock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.RedBlock);
                        break;
                    case ActionType.IsSupport:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.Support);
                        break;
                    case ActionType.IsQuadBlock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.QuadBlock);
                        break;
                    case ActionType.IsRoad:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.Road);
                        break;
                    case ActionType.IsBox:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.Box);
                        break;
                    case ActionType.IsEmpty:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetProp(x, y).isEmpty);
                        break;
                    case ActionType.IsNotEmpty:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => !World.GetProp(x, y).isEmpty);
                        break;
                    case ActionType.IsFalling:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetProp(x, y).isSand || World.GetProp(x, y).isBoulder);
                        break;
                    case ActionType.IsCrystal:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.isCry(World.GetCell(x, y)));
                        break;
                    case ActionType.IsLivingCrystal:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.isAlive(World.GetCell(x, y)));
                        break;
                    case ActionType.IsBoulder:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetProp(x, y).isBoulder);
                        break;
                    case ActionType.IsSand:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetProp(x, y).isSand);
                        break;
                    case ActionType.IsBreakableRock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetProp(x, y).is_diggable);
                        break;
                    case ActionType.IsUnbreakable:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => !World.GetProp(x, y).is_diggable);
                        break;
                    case ActionType.IsAcid:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => false);
                        break;
                    case ActionType.IsRedRock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.RedRock);
                        break;
                    case ActionType.IsBlackRock:
                        player.ProgData.SetCheckCondition(player, bool (int x, int y) => World.GetCell(x, y) == (byte)CellType.NiggerRock);
                        break;
                    case ActionType.CheckUp:
                        player.ProgData.CheckX = 0;
                        player.ProgData.CheckY = -1;
                        break;
                    case ActionType.CheckLeft:
                        player.ProgData.CheckX = -1;
                        player.ProgData.CheckY = 0;
                        break;
                    case ActionType.CheckDown:
                        player.ProgData.CheckX = 0;
                        player.ProgData.CheckY = 1;
                        break;
                    case ActionType.CheckRight:
                        player.ProgData.CheckX = 1;
                        player.ProgData.CheckY = 0;
                        break;
                    case ActionType.CheckUpLeft:
                        player.ProgData.CheckX = 1;
                        player.ProgData.CheckY = -1;
                        break;
                    case ActionType.CheckUpRight:
                        player.ProgData.CheckX = -1;
                        player.ProgData.CheckY = -1;
                        break;
                    case ActionType.CheckDownLeft:
                        player.ProgData.CheckX = 1;
                        player.ProgData.CheckY = 1;
                        break;
                    case ActionType.CheckDownRight:
                        player.ProgData.CheckX = -1;
                        player.ProgData.CheckY = 1;
                        break;
                    case ActionType.CheckForward:
                        player.ProgData.CheckX = player.dir switch
                        {
                            1 => -1,
                            3 => 1,
                            _ => 0
                        };
                        player.ProgData.CheckY = player.dir switch
                        {
                            0 => 1,
                            2 => -1,
                            _ => 0
                        };
                        break;
                    case ActionType.CheckLeftRelative:
                        player.ProgData.CheckX = player.dir switch
                        {
                            0 => -1,
                            2 => 1,
                            _ => 0
                        };
                        player.ProgData.CheckY = player.dir switch
                        {
                            1 => 1,
                            3 => -1,
                            _ => 0
                        };
                        break;
                    case ActionType.CheckRightRelative:
                        player.ProgData.CheckX = player.dir switch
                        {
                            0 => 1,
                            2 => -1,
                            _ => 0
                        };
                        player.ProgData.CheckY = player.dir switch
                        {
                            1 => -1,
                            3 => 1,
                            _ => 0
                        };
                        break;
                    case ActionType.ShiftUp:
                        player.ProgData.CheckY--;
                        break;
                    case ActionType.ShiftLeft:
                        player.ProgData.CheckX--;
                        break;
                    case ActionType.ShiftDown:
                        player.ProgData.CheckY++;
                        break;
                    case ActionType.ShiftRight:
                        player.ProgData.CheckX++;
                        break;
                    case ActionType.ShiftForward:
                        player.ProgData.CheckX += player.dir switch
                        {
                            1 => -1,
                            3 => 1,
                            _ => 0
                        };
                        player.ProgData.CheckY += player.dir switch
                        {
                            0 => 1,
                            2 => -1,
                            _ => 0
                        };
                        break;
                    case ActionType.Flip:
                        player.ProgData.CheckX *= -1;
                        player.ProgData.CheckY *= -1;
                        break;
                    case ActionType.RunIfFalse:
                        labelCoords = player.ProgData.IndexOf(action.Label);
                        if (!player.ProgData.Condition)
                        {
                            player.ProgData.X = labelCoords.X;
                            player.ProgData.Y = labelCoords.Y;
                            return;
                        }

                        break;
                    case ActionType.RunIfTrue:
                        labelCoords = player.ProgData.IndexOf(action.Label);
                        if (player.ProgData.Condition)
                        {
                            player.ProgData.X = labelCoords.X;
                            player.ProgData.Y = labelCoords.Y;
                            return;
                        }

                        break;
                    case ActionType.GoTo:
                        labelCoords = player.ProgData.IndexOf(action.Label);
                        player.ProgData.X = labelCoords.X;
                        player.ProgData.Y = labelCoords.Y;
                        return;
                    case ActionType.RunSub:
                        labelCoords = player.ProgData.IndexOf(action.Label);
                        player.ProgData.ReturnX = player.ProgData.X + 1;
                        player.ProgData.ReturnY = player.ProgData.Y;
                        player.ProgData.X = labelCoords.X;
                        player.ProgData.Y = labelCoords.Y;
                        return;
                    case ActionType.Return:
                        player.ProgData.X = player.ProgData.ReturnX;
                        player.ProgData.Y = player.ProgData.ReturnY;
                        return;
                    case ActionType.NextRow:
                        player.ProgData.X = 0;
                        player.ProgData.Y++;
                        return;
                    case ActionType.Start:
                        player.ProgData.startx = player.ProgData.X;
                        player.ProgData.starty = player.ProgData.Y;
                        return;
                    case ActionType.Stop:
                        player.SwitchProg();
                        break;
                    case ActionType.Beep:
                        player.Beep();
                        break;
                }
            }
        }
        public static void Update()
        {

        }
    }
}
