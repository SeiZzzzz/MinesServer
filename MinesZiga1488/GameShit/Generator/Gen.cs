namespace MinesServer.GameShit.Generator
{
    public class Gen
    {
        public System.Timers.Timer t;
        public List<(int, int)> spawns;
        public (float, int, bool)[] map;
        public Gen(int width, int height)
        {
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
        public void StartGeneration()
        {
            var x = new Random();
            var vcount = (width * height * 0.2 / World.W.chunksCountW * 0.01) * 10;
        }
        public void Update()
        {
        }
    }
}
