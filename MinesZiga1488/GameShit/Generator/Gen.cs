using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MinesServer.GameShit.Generator
{
    public class Gen
    {
        public System.Timers.Timer t;
        public List<VulcanoHeart> hearts;
        public List<(int, int)> spawns;
        public Gen(int width, int height)
        {
            this.height = height;
            this.width = width;
            t = new System.Timers.Timer(15);
            t.Elapsed += (e, a) => { Update(); };
            t.Start();
            hearts = new List<VulcanoHeart>();
            spawns = new List<(int, int)>();
        }
        private int height;
        private int width;
        public void GenerateSpawn(int count)
        {
            var r = new Random();
            for (int i = 0;i < count;i++)
            {
                var x = r.Next(width);
                var y = 0;
                spawns.Add((x, y));
                for (int xs = 0;xs < 24;xs++)
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
            var vcount = (width * height) / 2560;
            for (int i = 0; i < vcount;i++)
            {
                var vx = x.Next(width);
                var vy = x.Next(height);
                World.W.SetCell(vx,vy,31);
                hearts.Add(new VulcanoHeart(vx,vy));
            }
        }
        public void Update()
        {
            for (int i = 0;i < hearts.Count;i++)
            {
                hearts[i].Update();
            }
        }
    }
}
