using MinesServer.GameShit.SysCraft;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Sys_Craft
{
    public class CraftEntry
    {
        public CraftEntry()
        {

        }
        public CraftEntry(int rec_id, int num, DateTime end)
        {
            starttime = DateTime.Now;
            endtime = end;
            recipie_id = rec_id; this.num = num;
        }
        public Recipie GetRecipie()
        {
            return RDes.recipies.FirstOrDefault(i => i.id == recipie_id);
        }
        [NotMapped]
        public double progress { get
            {
                var value = Math.Round((((endtime - starttime) - (endtime - DateTime.Now)) / (endtime - starttime)) * 100, 2);
                value = value < 0 ? 0 : value < 101 ? value : 100;
                return value;
            }
        }
        public int id { get; set; }
        public int recipie_id { get; set; }
        public int num { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }
}
