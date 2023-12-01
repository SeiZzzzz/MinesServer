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
            return (CellType)cell switch
            {
                CellType.AliveViol => AliveViol(x, y),
                CellType.AliveRainbow => AliveRainbow(x, y),
                CellType.AliveBlue => AliveBlue(x, y),
                CellType.AliveRed => AliveRed(x, y),
                CellType.AliveCyan => AliveCyan(x, y),
                CellType.AliveNigger => AliveNigger(x, y),
                CellType.AliveWhite => AliveWhite(x, y),
                _ => false
            };
        }
        private static (int, int)[] baseddirs = [(1, 0), (0, 1), (-1, 0), (0, -1)];
        public static bool AliveBlue(int x, int y)
        {
            foreach (var i in baseddirs)
            {
                if (r.Next(1, 101) < 20 && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0 && World.IsEmpty(x + i.Item1, y + i.Item2))
                {
                    World.MoveCell(x, y, i.Item1, i.Item2);
                    World.SetCell(x, y, 109);
                    World.SetDurability(x, y, 20);
                    return true;
                }
            }
            return false;
        }
        public static bool AliveWhite(int x, int y)
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
        public static bool AliveNigger(int x, int y)
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
                        }
                        else
                        {
                            World.SetCell(x + -i.Item1, y + -i.Item2, (byte)CellType.Cyan);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool AliveCyan(int x, int y)
        {
            var c = 0;
            foreach (var i in baseddirs)
            {
                if (World.IsEmpty(x + i.Item1, y + i.Item2) && World.W.GetPlayersFromPos(x + i.Item1, y + i.Item2).Count == 0)
                {
                    World.SetCell(x + i.Item1, y + i.Item2, (byte)CellType.Cyan);
                    World.SetDurability(x + i.Item1, y + i.Item2, 2);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        public static bool AliveRainbow(int x, int y)
        {
            return false;
        }
        public static bool AliveRed(int x, int y)
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
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        public static bool AliveViol(int x, int y)
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
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
    }
}
