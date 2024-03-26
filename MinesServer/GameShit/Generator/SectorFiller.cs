using Microsoft.EntityFrameworkCore.Migrations.Operations;
using RcherNZ.AccidentalNoise;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private Dictionary<CellType, (float, float)> RandomSizedParts(params CellType[] args)
        {
            var dick = new Dictionary<CellType, (float, float)>();
            foreach (var d in args)
            {
                double start = rand.NextDouble();
                double end = start + rand.NextDouble();
                while (dick.Values.Any(segment => segment.Item1 <= end && segment.Item2 >= start))
                {
                    start = rand.NextDouble();
                    end = start + rand.NextDouble();
                    continue;
                }
                dick[d] = ((float)start, (float)end);
            }
            return dick;
        }
        private (float min,float max) FillNoiseToSector(Sector s)
        {
            var fr = NotTypedNoise();
            float max = (float)fr.Get(0, 0);
            float min = (float)fr.Get(0, 0);
            double localoffsetx = rand.NextDouble();
            double localoffsety = rand.NextDouble();
            foreach (var c in s.seccells)
            {
                var x = c.pos.Item1 == 0 ? 100 : c.pos.Item1;
                var y = c.pos.Item2 == 0 ? 100 : c.pos.Item2;
                var widthx = (s.width) == 0 ? 100 : (s.width);
                var heighty = (s.height) == 0 ? 100 : (s.height);
                var v = (float)fr.Get((float)((float)x / (float)widthx), (float)((float)y / (float)heighty));
                while (v == double.NaN || v == 0)
                {
                    localoffsetx += rand.NextDouble();
                    localoffsety += rand.NextDouble();
                    if (localoffsetx > x)
                        localoffsetx = 0;
                    if (localoffsety > y)
                        localoffsety = 0;
                    v = (float)fr.Get((float)(x + localoffsetx) / (float)widthx, (float)(y + localoffsety) / (float)heighty);
                }
                max = max < v ? v : max;
                min = min < v ? min : v;
                c.value = v;
            }
            return (min, max);
        }
        private Dictionary<CellType, int> SampleAndFindTypes(Sector s, Dictionary<CellType, (float, float)> parts, (float minvalue,float maxvalue) data)
        {
            var fr = NotTypedNoise();
            var typesresult = new Dictionary<CellType, int>();
            foreach (var c in s.seccells)
            {
                c.value = ((c.value - data.minvalue) / (data.maxvalue - data.minvalue));
                for (int i = 0; i < parts.Count; i++)
                {
                    c.type = c.value >= parts.ElementAt(i).Value.Item1 && c.value <= parts.ElementAt(i).Value.Item2 ? parts.ElementAt(i).Key : c.type;
                    if (!typesresult.ContainsKey(c.type))
                        typesresult[c.type] = 1;
                    else
                    {
                        typesresult[c.type]++;
                    }
                }
            }
            return typesresult;
        }
        //TODO: если значение не попадает в существующие отрезки перегенирировать
        public void CreateFillForCells(Sector s, bool gig = false, params CellType[] args)
        {
            var segmentsmall = 0;
            var notenouthparts = 0;
            var empty = 0;
            restart:
            var parts = RandomSizedParts(args);
            while(parts.Count < args.Length)
            {
                parts = RandomSizedParts(args);
            }
        refillnoise:
            var data = FillNoiseToSector(s);
            var result = SampleAndFindTypes(s, parts, data);
            if (result.Count < parts.Count)
            {
                notenouthparts++;
                if (notenouthparts > 2)
                {
                    notenouthparts = 0;
                    Console.WriteLine("restarted");
                    goto restart;
                }
                Console.WriteLine("to small result");
                goto refillnoise;
            }
            if (result.ContainsKey(CellType.Empty) && s.seccells.Count * 0.4 < result[CellType.Empty])
            {
                empty++;
                if (empty > 4)
                {
                    Console.WriteLine("too empty");
                    empty = 0;
                    goto restart;
                }
                goto refillnoise;
            }
            foreach (var i in result)
            {
                var check = (s.seccells.Count / parts.Count) * 0.4 > i.Value;
                if (check)
                {
                    segmentsmall++;
                    if (segmentsmall > 2)
                    {
                        segmentsmall = 0;
                        Console.WriteLine("OneOfsegmentstosmall restart");
                        goto restart;
                    }
                    Console.WriteLine($"OneOfsegmentstosmall resample {segmentsmall}");
                    goto refillnoise;
                }
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
