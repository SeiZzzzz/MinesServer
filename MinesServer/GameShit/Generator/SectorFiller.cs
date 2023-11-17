using RcherNZ.AccidentalNoise;

namespace MinesServer.GameShit.Generator
{
    public class SectorFiller
    {
        public CellType[] typedmap;
        public float[] map { get; private set; }
        private Random rand = new Random();
        public string blyat;
        private ImplicitFractal NotTypedNoise()
        {
            var type = (FractalType)rand.Next(0, 5);
            var basis = (BasisType)rand.Next(0, 4);
            var interpol = (InterpolationType)rand.Next(0, 4);
            blyat = $"{type} {basis} {interpol}";
            return new ImplicitFractal(type, basis, interpol)
            {
                Octaves = rand.Next(4, 20),
                Frequency = rand.Next(4, 20),
                Lacunarity = rand.Next(1, 20)

            };
        }
        public void CreateFillForCells(Sector s, bool gig = false, params CellType[] args)
        {
        tme:
            Console.WriteLine("");
            var dick = new Dictionary<CellType, (float, float)>();
            var gt = 0;
            var gt1 = 0;
            var gte = 0;
            foreach (var d in args)
            {
            GC:
                int f = rand.Next(1, 1000001);
                int sec = rand.Next(f, 1000001);
                var dis = (float)((sec - f) / 1000000f);
                if (dis > 0.2f || dis < 0.05f)
                {
                    gt++;
                    Console.Write($"\rgoto {gt}");
                    if (gt > 100000)
                    {
                        goto tme;
                    }
                    goto GC;
                }
                for (int j = 0; j < dick.Count; j++)
                {
                    if ((dick.ElementAt(j).Value.Item1 * 1000000f) < sec && f < (dick.ElementAt(j).Value.Item2 * 1000000f))
                    {
                        gt1++;
                        Console.Write($"\rgoto err1 {gt1}");
                        if (gt1 > 100000)
                        {
                            goto tme;
                        }
                        goto GC;
                    }
                }
                dick[d] = (f / 1000000f, sec / 1000000f);
            }
        reg:
            var fr = NotTypedNoise();
            var max = (float)fr.Get(0, 0);
            var min = (float)fr.Get(0, 0);
            foreach (var c in s.seccells)
            {
                var v = (float)fr.Get((float)(c.pos.Item1 / (float)s.width), (float)(c.pos.Item2 / (float)s.height));
                max = max < v ? v : max;
                min = min < v ? min : v;
                c.value = v;
            }
            var error = 0;
            foreach (var c in s.seccells)
            {
                c.value = (float)((c.value - min) / (max - min));
                for (int i = 0; i < dick.Count; i++)
                {
                    c.type = c.value >= dick.ElementAt(i).Value.Item1 && c.value <= dick.ElementAt(i).Value.Item2 ? dick.ElementAt(i).Key : c.type;
                }
                if (c.type == CellType.Empty)
                {
                    error++;
                }
            }
            if (error > ((s.seccells.Count) * 0.6f))
            {
                gte++;
                Console.Write($"\rgoto toosmall {gte}");
                if (gte > 10)
                {
                    Console.Write($"\ra lots of errors restart");
                    goto tme;
                }
                goto reg;
            }
            if (gig)
            {
                var ft = args[rand.Next(0, args.Length - 1)];
                foreach (var c in s.seccells)
                {
                    if (c.type == CellType.Empty)
                    {
                        c.type = ft;
                    }
                    if (alive(s.seccells.Count) > rand.Next(1, 101))
                    {
                        //c.type = randalive
                    }
                }
            }
            Console.WriteLine("");
        }
        public static int alive(int x)
        {
            return 40 + (((85 - 40) * (x - 50000)) / (5000 - 50000));
        }
    }

}
