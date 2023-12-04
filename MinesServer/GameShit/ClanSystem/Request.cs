using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.ClanSystem
{
    public class Request
    {
        public int id { get; set; }
        public Player player { get; set; }
        public Clan clan { get; set; }
        public DateTime reqtime { get; set; }
    }
}
