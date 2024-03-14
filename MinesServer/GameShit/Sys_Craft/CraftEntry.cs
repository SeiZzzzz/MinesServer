using MinesServer.GameShit.Buildings;
using RT.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Sys_Craft
{
    public class CraftEntry
    {
        public CraftEntry()
        {
            
        }
        public CraftEntry(int res_id,int num,DateTime end)
        {
           starttime = DateTime.Now;
           endtime = end;
           result_id = res_id; this.num = num;
        }
        [NotMapped]
        public double progress { get => Math.Round((((endtime - starttime) - (endtime - DateTime.Now)) / (endtime - starttime)) * 100,2); }
        public int id { get; set; }
        public int result_id { get; set; }
        public int num { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }
}
