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
        public void StartGeneration()
        {
            Console.WriteLine("Generating sectors");
            var sec = new Sectors((width, height));
            sec.GenerateENoise(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Cubic);
            sec.AddW(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(25, 5, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(35, 20, RcherNZ.AccidentalNoise.InterpolationType.Quintic);
            sec.End();
            var map = sec.map;
            var rc = 0;
            for (int x = 0; x < width; x += 32)
            {
                for (int y = 0; y < height; y += 32)
                {
                    for (int chx = 0; chx < 32; chx++)
                    {
                        for (int chy = 0; chy < 32; chy++)
                        {
                            var t = map[(x + chx) * height + (y + chy)].value == 2 ? (byte)CellType.NiggerRock : map[(x + chx) * height + (y + chy)].value == 1 ? (byte)CellType.RedRock : (byte)0;
                            if (t != 0)
                            {
                                World.SetCell((x + chx), (y + chy), t);
                            }
                            else
                            {
                                World.SetCell((x + chx), (y + chy), 32);
                            }
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
                if (s[i].seccells.Count > 40000)
                {
                    inside.CreateFillForCells(s[i], false, s[i].GenerateInsides());
                }
                else if (s[i].seccells.Count <= 40000)
                {
                    inside.CreateFillForCells(s[i], true, s[i].GenerateInsides());
                }
                Console.WriteLine("saving sector " + s[i].seccells.Count);
                foreach (var c in s[i].seccells)
                {
                    var ty = c.type == CellType.Empty ? (byte)0 : (byte)c.type;
                    if (ty != 0)
                    {
                        World.SetCell(c.pos.Item1, c.pos.Item2, ty);
                    }
                    else
                    {
                        World.SetCell(c.pos.Item1, c.pos.Item2, 32);
                    }
                }
            }
            World.W.cells.Commit();
            World.W.road.Commit();
            World.W.durability.Commit();

            Console.WriteLine("END END");
        }
    }
}
