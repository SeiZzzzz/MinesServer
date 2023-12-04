using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.ClanSystem
{
    public class Rank
    {
        public int id { get; set; }
        public int priority { get; set; }
        public string colorhex { get; set; }
        public string name { get; set; }
        public Clan owner { get; set; }
    }
}
