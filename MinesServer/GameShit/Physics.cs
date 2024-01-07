namespace MinesServer.GameShit
{
    public static class Physics
    {
        public static Random r = new Random();
        public static bool Boulder(int x, int y)
        {
            if (World.GetProp(World.GetCell(x, y + 1)).isEmpty)
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }
            if (World.GetProp(World.GetCell(x, y + 1)).isBoulder || World.GetProp(World.GetCell(x, y + 1)).isSand)
            {
                if (r.Next(1, 101) > 50 && World.GetProp(World.GetCell(x + 1, y + 1)).isEmpty && World.GetProp(World.GetCell(x + 1, y)).isEmpty)
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }
                else if (World.GetProp(World.GetCell(x - 1, y + 1)).isEmpty && World.GetProp(World.GetCell(x - 1, y)).isEmpty)
                {
                    World.MoveCell(x, y, -1, 1);
                    return true;
                }
            }
            return false;
        }
        public static bool Sand(int x, int y)
        {
            if (World.IsEmpty(x, y + 1))
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }
            else if (World.GetProp(World.GetCell(x, y + 1)).isSand || World.GetProp(World.GetCell(x, y + 1)).isBoulder)
            {
                if (r.Next(1, 101) > 50 && World.IsEmpty(x + 1, y + 1))
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }
                else if (World.IsEmpty(x - 1, y + 1))
                {
                    World.MoveCell(x, y, -1, 1);
                    return true;
                }
            }
            return false;
        }
        public static bool Alive(int x, int y)
        {
            var cell = World.GetCell(x, y);
            var mod = 1;
            foreach (var dir in baseddirs)
            {
                if (World.GetCell(x + dir.Item1, y + dir.Item2) == 119)
                {
                    mod += 2;
                }
            }
            if (mod > 1)
            {
                mod -= 1;
            }
            return (CellType)cell switch
            {
                CellType.AliveViol => AliveViol(x, y, mod),
                CellType.AliveRainbow => AliveRainbow(x, y, mod),
                CellType.AliveBlue => AliveBlue(x, y, mod),
                CellType.AliveRed => AliveRed(x, y, mod),
                CellType.AliveCyan => AliveCyan(x, y, mod),
                CellType.AliveNigger => AliveNigger(x, y, mod),
                CellType.AliveWhite => AliveWhite(x, y, mod),
                _ => false
            };
        }
        private static (int, int)[] baseddirs = [(1, 0), (0, 1), (-1, 0), (0, -1)];
        public static bool AliveBlue(int x, int y, int mod)
        {
            foreach (var i in baseddirs)
            {
                if (r.Next(1, 101) < 20 && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0 && World.IsEmpty(x + i.Item1, y + i.Item2))
                {
                    World.MoveCell(x, y, i.Item1, i.Item2);
                    World.SetCell(x, y, 109);
                    World.SetDurability(x, y, 20 * mod);
                    return true;
                }
            }
            return false;
        }
        public static bool AliveWhite(int x, int y, int mod)
        {
            if (World.GetProp(x, y - 1).isSand)
            {
                for (int wx = -1; wx <= 1; wx++)
                {
                    for (int wy = -1; wy <= 1; wy++)
                    {
                        if (World.IsEmpty(x + wx, y + wy) && World.W.GetPlayersFromPos(x + wx, y + wy).Count == 0)
                        {
                            World.SetCell(x + wx, y + wy, (byte)CellType.White);
                            World.SetDurability(x + wx, y + wy, 9 * mod);
                        }
                    }
                }
                if (r.Next(1, 101) < 20)
                {
                    World.Destroy(x, y - 1);
                }
                return true;
            }
            return true;
        }
        public static bool AliveNigger(int x, int y, int mod)
        {
            var c = 0;
            for (int ax = -1; ax <= 1; ax++)
            {
                for (int ay = -1; ay <= 1; ay++)
                {
                    var cell = World.GetCell(x + ax, y + ay);
                    if (cell == (byte)CellType.AliveNigger)
                    {
                        c++;
                    }
                }
            }
            if (c >= 6)
            {
                World.SetCell(x, y, 114);
                return true;
            }
            if (c > 0)
            {
                foreach (var i in baseddirs)
                {
                    var cell = World.GetCell(x + i.Item1, y + i.Item2);
                    if (cell == (byte)CellType.AliveNigger && World.IsEmpty(x + -i.Item1, y + -i.Item2) && World.W.GetPlayersFromPos(x + -i.Item1, y + -i.Item2).Count == 0)
                    {
                        if (r.Next(1, 101) > 50)
                        {
                            World.SetCell(x + -i.Item1, y + -i.Item2, (byte)CellType.Red);
                            World.SetDurability(x + i.Item1, y + i.Item2, 3 * mod);
                        }
                        else
                        {
                            World.SetCell(x + -i.Item1, y + -i.Item2, (byte)CellType.Cyan);
                            World.SetDurability(x + i.Item1, y + i.Item2, 2 * mod);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool AliveCyan(int x, int y, int mod)
        {
            var c = 0;
            foreach (var i in baseddirs)
            {
                if (World.IsEmpty(x + i.Item1, y + i.Item2) && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0)
                {
                    World.SetCell(x + i.Item1, y + i.Item2, (byte)CellType.Cyan);
                    World.SetDurability(x + i.Item1, y + i.Item2, 2 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        public static bool AliveRainbow(int x, int y, int mod)
        {
            var c = 0;
            foreach (var dir in baseddirs)
            {
                if (World.IsEmpty(x + dir.Item1, y + dir.Item2) && World.W.GetPlayersFromPos(x + dir.Item1, y + dir.Item2).Count == 0 && !World.isAlive(World.GetCell(x + -dir.Item1, y + -dir.Item2)) && !World.GetProp(x + -dir.Item1, y + -dir.Item2).isEmpty && World.GetProp(x + -dir.Item1, y + -dir.Item2).is_diggable && World.GetProp(x + -dir.Item1, y + -dir.Item2).is_destructible)
                {
                    World.SetCell(x + dir.Item1, y + dir.Item2, World.GetCell(x + -dir.Item1, y + -dir.Item2));
                    World.SetDurability(x + dir.Item1, y + dir.Item2, World.GetProp(x + dir.Item1, y + dir.Item2).durability * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        public static bool AliveRed(int x, int y, int mod)
        {
            var c = 0;
            var chs = 0;
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.NiggerRock)
                    {
                        chs++;
                    }
                }
            }
            if (chs < 1)
            {
                return false;
            }
            foreach (var i in baseddirs)
            {
                if (World.IsEmpty(x + i.Item1, y + i.Item2) && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0)
                {
                    World.SetCell(x + i.Item1, y + i.Item2, (byte)CellType.Red);
                    World.SetDurability(x + i.Item1, y + i.Item2, 3 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        public static bool AliveViol(int x, int y, int mod)
        {
            var c = 0;
            var chs = 0;
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.NiggerRock)
                    {
                        chs++;
                    }
                }
            }
            if (chs < 1)
            {
                return false;
            }
            foreach (var i in baseddirs)
            {
                if (World.IsEmpty(x + i.Item1, y + i.Item2) && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0)
                {
                    World.SetCell(x + i.Item1, y + i.Item2, (byte)CellType.Violet);
                    World.SetDurability(x + i.Item1, y + i.Item2, 2 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
    }
}
