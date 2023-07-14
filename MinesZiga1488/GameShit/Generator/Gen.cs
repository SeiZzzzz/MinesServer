namespace MinesServer.GameShit.Generator
{
    public class Gen
    {
        public System.Timers.Timer t;
        public static List<VulcanoHeart> hearts;
        public List<(int, int)> spawns;
        public static float[,] heatmap;
        public Gen(int width, int height)
        {
            Gen.height = height;
            Gen.width = width;
            hearts = new List<VulcanoHeart>();
            spawns = new List<(int, int)>();
            heatmap = new float[2, width * height];
            Task.Run(() =>
            {
                var x = 0;
                while (true)
                {
                    World.W.SetCell(x, 0, 36);
                    x++;
                    Update();
                    Thread.Sleep(1);
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
        public static float GetHeat(int x, int y)
        {
            if (!World.W.ValidCoord(x, y))
            {
                return 0;
            }
            return heatmap[1, x + y * height];
        }
        public static void UpdateHeat(int x, int y, float value, float vid)
        {
            if (World.W.ValidCoord(x, y))
            {
                heatmap[1, x + y * height] += value;
                heatmap[0, x + y * height] = vid;
            }
        }
        public void StartGeneration()
        {
            var x = new Random();
            var vcount = (width * height * 0.2 / World.W.chunksCountW * 0.01);
            for (int i = 1; i < vcount; i++)
            {
                var vx = x.Next(width);
                var vy = x.Next(height);
                World.W.SetCell(vx, vy, 31);
                hearts.Add(new VulcanoHeart(vx, vy, i));
            }
        }
        public void Update()
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].Update();
            }
        }
    }
}
