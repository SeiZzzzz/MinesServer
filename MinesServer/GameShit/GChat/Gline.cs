using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.GChat
{
    public class GLine
    {
        [NotMapped]
        public int time = (int)(DateTime.Now.Ticks / 10000L / 60000L);
        public int id { get; set; }
        public Player? player { get; set; }
        public string message { get; set; }
        public Chat owner { get; set; }
    }
}
