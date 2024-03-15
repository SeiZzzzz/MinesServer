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
                double start = rand.NextDouble();
                double end = start + rand.NextDouble();
                while(dick.Values.Any(segment => segment.Item1 <= end && segment.Item2 >= start))
                {
                    start = rand.NextDouble();
                    end = start + rand.NextDouble();
                    continue;
                }
                dick[d] = ((float)start, (float)end);
            }
        reg:
            var fr = NotTypedNoise();
            var max = (float)fr.Get(0, 0);
            var min = (float)fr.Get(0, 0);
            var offsetx = 1;
            var offsety = 1;
            foreach (var c in s.seccells)
            {
                var v = (float)fr.Get((float)(c.pos.Item1 + offsetx / (float)s.width + offsetx), (float)(c.pos.Item2 + offsety / (float)s.height + offsety));
                max = max < v ? v : max;
                min = min < v ? min : v;
                c.value = v;
            }
            var error = 0;
            var types = new Dictionary<CellType, int>();
            foreach (var c in s.seccells)
            {
                c.value = ((c.value - min) / (max - min));
                for (int i = 0; i < dick.Count; i++)
                {
                    c.type = c.value >= dick.ElementAt(i).Value.Item1 && c.value <= dick.ElementAt(i).Value.Item2 ? dick.ElementAt(i).Key : c.type;
                    if (!types.ContainsKey(c.type))
                        types[c.type] = 1;
                    else
                    {
                        types[c.type]++;
                    }
                }
                if (c.type == CellType.Empty)
                {
                    error++;
                }
            }
            if (types.Count < args.Length)
            {
                gte++;
                Console.Write($"\rnot enouth types {gte}");
                if (gte > 3)
                {
                    Console.Write($"\rtypes restart");
                    goto tme;
                }
                goto reg;
            }
            foreach(var i in types)
            {
                var check = s.seccells.Count / dick.Count * 0.4 > i.Value;
                if (check)
                {
                    gte++;
                    Console.Write($"\rto small segment {gte}");
                    if (gte > 3)
                    {
                        Console.Write($"\ra lots of errors restart segmentation");
                        goto tme;
                    }
                    goto reg;
                }
            }
            if (error > ((s.seccells.Count) * 0.6f))
            {
                gte++;
                Console.Write($"\rgoto toosmall {gte}");
                if (gte > 3)
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
