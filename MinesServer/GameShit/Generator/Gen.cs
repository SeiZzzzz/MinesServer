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
                

            }
        }
        public void StartGeneration()
        {
            Console.WriteLine("Generating sectors");
            var sec = new Sectors((width, height));
            sec.GenerateENoise(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Cubic);
            sec.AddW(25, 1, RcherNZ.AccidentalNoise.InterpolationType.Linear, .55f);
            sec.End();
            var map = sec.map;
            var rc = 0;
            for (int x = 0; x < width; x+= 32)
            {
                for (int y = 0; y < height; y += 32)
                {
                    for(int chx = 0; chx < 32; chx++)
                    {
                        for (int chy = 0; chy < 32; chy++)
                        {
                            var t = map[(x + chx) + (y + chy) * height].value == 2 ? (byte)CellType.NiggerRock : map[(x + chx) + (y + chy) * height].value == 1 ? (byte)CellType.RedRock : (byte)0;
                            if (t != 0)
                            {
                                World.W.GetChunk((x + chx), (y + chy)).wcells[chx + chy * 32] = t;
                            }
                            World.W.GetChunk((x + chx), (y + chy)).rcells[chx + chy * 32] = 32;
                            rc++;
                        }

                    }    
                }
                Console.Write($"\r{rc}/{map.Length} saving rocks");
            }
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
                Console.WriteLine("saving sector " + s[i].seccells.Count);
                foreach (var c in s[i].seccells)
                {
                    var ty = c.type == CellType.Empty ? (byte)0 : (byte)c.type;
                    var ch = World.W.GetChunk(c.pos.Item1, c.pos.Item2);
                    var xx = c.pos.Item1 - ch.pos.Item1 * 32;var yy = c.pos.Item2 - ch.pos.Item2 * 32;
                    if (ty != 0)
                    {
                        ch.wcells[xx + yy * 32] = ty;
                    }
                    ch.rcells[xx + yy * 32] = 32;
                }
            }
            World.W.map.SaveAllChunks();
            Console.WriteLine("END END");
        }
        public void Update()
        {
        }
    }
}
