using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator
{
    public class ProgsData
    {
        public ProgsData(Player p)
        {
            this.p = p;
        }
        Player p;
        public int checkX;
        public int checkY;
        public bool ProgRunning { get; set; } = false;
        public Program? current { get; set; }
        public DateTime delay;
        public void Run()
        {
            current?.Decode();
            delay = DateTime.Now;
            ProgRunning = true;
        }
        public void IncreaseDelay(int ms) => delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        public void Step()
        {
            if (current == null || DateTime.Now < delay)
            { 
                return;
            }
            var result = current.prog.Next();
            result.Execute(p);
        }
    }
}
    