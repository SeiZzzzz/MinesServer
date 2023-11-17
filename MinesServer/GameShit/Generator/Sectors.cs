using RcherNZ.AccidentalNoise;

namespace MinesServer.GameShit.Generator
{
    public class Sectors
    {
        public Sectors(int seed, (int, int) size)
        {
            this.size = size;
            this.seed = seed;
            r = new Random(seed);
        }
        public Sectors((int, int) size)
        {
            this.size = size;
            seed = Environment.TickCount;
            r = new Random(seed);
        }
        public List<Sector> DetectSectors()
        {
            List<Sector> sectors = new List<Sector>();
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            var ce = new List<SectorCell>();
            var que = new Queue<SectorCell>();
            for (int y = 0; y < size.Item2; y++)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x + y * size.Item2].sector == -1 && map[x + y * size.Item2].value == 0) //если клетка не принадлежит сектору и пустая, подзалупный алгоритм начинает работать
                    {
                        var swidth = 0;
                        var sheight = 0;
                        var depth = map[x + y * size.Item2].pos.Item2;
                        que.Enqueue(map[x + y * size.Item2]);
                        while (que.Count > 0)
                        {
                            var cell = que.Dequeue();
                            depth = depth > cell.pos.Item2 ? cell.pos.Item2 : depth;
                            swidth = swidth > (cell.pos.Item1 - map[x + y * size.Item2].pos.Item1) ? swidth : (map[x + y * size.Item2].pos.Item1 - map[x + y * size.Item2].pos.Item2);
                            sheight = sheight > (cell.pos.Item2 - map[x + y * size.Item2].pos.Item2) ? sheight : (cell.pos.Item1 - map[x + y * size.Item2].pos.Item2);
                            ce.Add(cell);
                            cell.sector = sectors.Count; //тут заполнение сектора клетки
                            foreach (var i in dirs)
                            {
                                var nx = cell.pos.Item1 + i.Item1; var ny = cell.pos.Item2 + i.Item2;
                                if (v(nx, ny))
                                {
                                    var ncell = map[nx + ny * size.Item2];
                                    if (ncell.sector == -1 && ncell.value == 0) // если клетка пустая, она становится частью сектора
                                    {
                                        ncell.sector = sectors.Count; //заполнение сектора клетки
                                        que.Enqueue(ncell);
                                    }
                                }
                            }
                        }
                        var s = new Sector() { seccells = ce, width = swidth, height = sheight, depth = depth };
                        sectors.Add(s);
                        ce = new List<SectorCell>();
                    }
                }
            }
            return sectors;

        }
        private float chs(int y)
        {
            return 30f - ((float)y * 0.0028f);
        }
        private void CleanCs(int j, bool b = false)
        {
            Console.WriteLine("filling cs to chs");
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            for (int y = (j % 2 == 0 ? 0 : size.Item2 - 1); (j % 2 == 0 ? y < size.Item2 : y >= 0);)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x + y * size.Item2].value == 1)
                    {
                        var c = 0; var ch = 0; var e = 0;
                        for (int xx = -2; xx <= 2; xx++)
                        {
                            for (int yy = -2; yy <= 2; yy++)
                            {
                                var nx = x + xx; var ny = y + yy;
                                if (v(nx, ny))
                                {
                                    if (map[nx + ny * size.Item2].value == 1)
                                    {
                                        c++;
                                    }
                                    else if (map[nx + ny * size.Item2].value == 2)
                                    {
                                        ch++;
                                    }
                                    else if (map[nx + ny * size.Item2].value == 0)
                                    {
                                        e++;
                                    }
                                }
                            }
                        }
                        if ((3 < ch && r.Next(1, 101) > 60) || (e > 1))
                        {
                            map[x + y * size.Item2].value = 2;
                            if (r.Next(1, 101) > 95 && b)
                            {
                                Boom(x, y);
                            }
                        }
                    }
                }
                if (j % 2 == 0)
                {
                    y++;
                    continue;
                }
                y--;
            }
        }
        public void GenerateENoise(double freq = 25, double lac = 1, InterpolationType t = InterpolationType.Cubic, float res = .45f)
        {
            fr = new ImplicitFractal(FractalType.RidgedMulti, BasisType.GradientValue, t)
            {
                Octaves = 1,
                Frequency = freq,
                Lacunarity = lac,
                Seed = seed
            };
            Console.WriteLine(fr.Type);
            map = new SectorCell[size.Item1 * size.Item2];
            max = (float)fr.Get(0, 0);
            min = (float)fr.Get(0, 0);
            var time = DateTime.Now;
            var counter = 0;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    var v = (float)fr.Get((float)(x / (float)size.Item1), (float)(y / (float)size.Item1));
                    max = max < v ? v : max;
                    min = min < v ? min : v;
                    map[x + y * size.Item2] = new SectorCell() { value = v, pos = (x, y), sector = -1 };
                    counter++;
                }
                Console.Write($"\r{counter}/{map.Length} setting base map");
            }
            Console.WriteLine("");
            Console.WriteLine($"{(DateTime.Now - time)} base set");
            Console.WriteLine(max);
            Console.WriteLine(min);
            mid = 0f;
            counter = 0;
            time = DateTime.Now;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    map[x + y * size.Item2].value = (float)((map[x + y * size.Item2].value - min) / (max - min));
                    mid += map[x + y * size.Item2].value;
                    counter++;
                }
                Console.Write($"\r{counter}/{map.Length} sampling map");
            }
            Console.WriteLine("");
            Console.WriteLine($"{(DateTime.Now - time)} sampling");
            mid /= map.Length;
            Console.WriteLine(mid);
            resample(res);
        }
        private void Clean()
        {
            Console.WriteLine("adding empty space");
            var c = 0;
            for (int y = 0; y < size.Item2; y++)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x + y * size.Item2].value == 2 && r.Next(1, 101) > 90)
                    {
                        map[x + y * size.Item2].value = 0;
                    }
                    else if (map[x + y * size.Item2].value == 1 && r.Next(1, 101) > 95)
                    {
                        map[x + y * size.Item2].value = 0;
                    }
                    c++;
                }
                Console.Write($"\r{c}/{map.Length} empty space");
            }
            Console.Write($"");
        }
        public void End()
        {
            Console.WriteLine("ending");
            Add();
            Clean();
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    map[x + y * size.Item2].type = map[x + y * size.Item2].value == 2 ? CellType.NiggerRock : (map[x + y * size.Item2].value == 1 ? CellType.RedRock : CellType.Empty);
                }
            }
            Console.WriteLine("end");
        }
        public void AddW(double freq = 25, double lac = 1, InterpolationType t = InterpolationType.Cubic, float res = .45f)
        {
            var temp = map;
            GenerateENoise(freq, lac, t, res);
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    temp[x + y * size.Item2].value = temp[x + y * size.Item2].value == 0 ? map[x + y * size.Item2].value : temp[x + y * size.Item2].value;
                }
            }
            map = temp;
        }
        private void Add()
        {
            CleanCs(0, true);
            for (int i = 1; i < 6; i++)
            {
                CleanCs(i);
            }
            Console.WriteLine("adding NIGGERrock");
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            var counter = 0;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    counter++;
                    if (map[x + y * size.Item2].value == 1)
                    {
                        if (r.Next(1, 101) < chs(y))
                        {
                            map[x + y * size.Item2].value = 2;
                        }
                    }
                }
                Console.Write($"\r{counter}/{map.Length} nigger rock");
            }
            Console.WriteLine("");
        }
        private void Boom(int x, int y)
        {
            var b = r.Next(3, 7);
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            for (int xx = -b; xx <= b; xx++)
            {
                for (int yy = -b; yy <= b; yy++)
                {
                    var nx = x + xx; var ny = y + yy;
                    if (v(nx, ny) && ((map[nx + ny * size.Item2].value == 0 && r.Next(1, 101) > 60) || (map[nx + ny * size.Item2].value == 1 && r.Next(1, 101) < chs(y))))
                    {
                        map[nx + ny * size.Item2].value = 2;
                    }
                }
            }
        }
        private void resample(float res = .45f)
        {
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    if (map[x + y * size.Item2].value < mid + res)
                    {
                        map[x + y * size.Item2].value = 0;
                    }
                    else if (map[x + y * size.Item2].value >= mid + res)
                    {
                        map[x + y * size.Item2].value = 1;
                    }
                }
            }
        }
        private (int, int)[] dirs = { (0, 1), (0, -1), (-1, 0), (1, 0) };
        public float min, mid, max;
        public SectorCell[] map { get; private set; }
        private ImplicitFractal fr;
        public (int, int) size { get; private set; }
        public Random r = new Random();
        public int seed { private set; get; }
    }
}
