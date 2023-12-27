using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator
{
    public class Program
    {
        public int id { get; set; }
        public string context { get; set; }
        public string title { get; set; }
        public Player owner { get; set; }
    }
}
