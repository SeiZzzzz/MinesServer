using System.Drawing;

namespace MinesServer.GameShit.Generator
{
    public class Gen
    {
        public System.Timers.Timer t;
        public List<(int, int)> spawns;
        public static Gen THIS;
        public Gen(int width, int height)
        {
            THIS = this;
            Gen.height = height;
            Gen.width = width;
            spawns = new List<(int, int)>();

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
                        World.W.SetCell(x + xs, y + ys, (byte)CellType.FedRoad);
                    }
                }

            }
        }
        public void StartGeneration()
        {
            Console.WriteLine("Generating sectors");
            var sec = new Sectors((width, height));
            sec.GenerateENoise(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Cubic);
            sec.AddW(25, 1, RcherNZ.AccidentalNoise.InterpolationType.Linear, .7f);
            sec.End();
            var map = sec.map;
            var s = sec.DetectSectors();
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].seccells.Count < 50)
                {
                    continue;
                }
                Console.WriteLine($"fill sectors {s[i].seccells.Count} {i}/{s.Count}");
                var inside = new SectorFiller();
                if (s[i].seccells.Count > 80000)
                {
                    inside.CreateFillForCells(s[i], false, s[i].GenerateInsides());
                }
                else if (s[i].seccells.Count <= 80000)
                {
                    inside.CreateFillForCells(s[i], true, s[i].GenerateInsides());
                }
                foreach (var c in s[i].seccells)
                {
                    map[c.pos.Item1 + c.pos.Item2 * sec.size.Item2].type = c.type;
                }
            }
            Console.WriteLine("final map");
            var co = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    
                    World.W.map.mapmesh[1][x + y * height] = map[x + y * height].type == CellType.Empty ? (byte)0 : (byte)map[x + y * height].type;
                    co++;
                }
                Console.Write($"\r{co}/{map.Length}");
            }
            Console.WriteLine("");
                var xx = 0;
                while (xx < width)
                {
                    World.W.SetCell(xx, 0, (byte)CellType.FedRoad);
                    xx++;
                }
            Console.WriteLine("END END");
        }
        public void Update()
        {
        }
    }
}
