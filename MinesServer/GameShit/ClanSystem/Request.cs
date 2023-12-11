using Microsoft.EntityFrameworkCore;
using MinesServer.Server;
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
        public DateTime reqtime { get; set; }
        public static Request LoadReq(Request q)
            {
            using var db = new DataBase();
            return db.reqs.Include(p => p.player).First(w => w == q);
        }
}
}
