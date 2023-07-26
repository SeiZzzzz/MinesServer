namespace MinesServer.GameShit.Generator
{
    public class Gen
    {
        public System.Timers.Timer t;
        public List<(int, int)> spawns;
        public (float, int, bool)[] map;
        public List<Heart> vulcs;
        public static Gen THIS;
        public Gen(int width, int height)
        {
            THIS = this;
            vulcs = new List<Heart>();
            Gen.height = height;
            Gen.width = width;
            spawns = new List<(int, int)>();
            map = new (float, int, bool)[width * height];
            Task.Run(() =>
            {
                var x = 0;
                while (true)
                {
                    World.W.SetCell(x, 0, 36);
                    x++;
                    Update();
                }
            });
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    UpdMap();
                }
            });
        }
        public static int height;
        public static int width;
        public void GenerateSpawn(int count)
        {
            var r = new Random();
            for (int i = 0; i < count; i++)
            {
                var x = r.Next(width);
                var y = 0;
                spawns.Add((x, y));
                for (int xs = 0; xs < 24; xs++)
                {
                    for (int ys = 0; ys < 24; ys++)
                    {
                        World.W.SetCell(x + xs, y + ys, 36);
                    }
                }

            }
        }
        public void StartGeneration()
        {
            var r = new Random();
            var vcount = 1 + (width * height * 0.2 / World.W.chunksCountW * 0.01) * 10;
            for (int i = 1; i < vcount;i++)
            {
                var x = r.Next(width);
                var y = r.Next(height);
                vulcs.Add(new Heart(x,y,i));
            }
        }
        public void Update()
        {
            for (int i = 0; i < vulcs.Count; i++)
            {
                vulcs[i].Update();
            }
        }
        public void UpdMap()
        {
            for (int x = 0;x < width;x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x + y * height].Item2 != 0)
                    {
                        if (World.GetProp(World.W.GetCell(x, y)).is_destructible)
                        {
                            World.W.SetCell(x, y, 91);
                        }
                    }
                }
            }
        }
        public static void HeatUp(int x, int y, float h,int id)
        {
            if (THIS.map[x + y * height].Item2 == 0 || THIS.map[x + y * height].Item2 == id)
            {
                THIS.map[x + y * height] = (THIS.map[x + y * height].Item1 + h, THIS.map[x + y * height].Item2, THIS.map[x + y * height].Item3);
            }
        }
    }
}
