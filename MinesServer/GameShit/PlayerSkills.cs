using MinesServer.GameShit.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public class PlayerSkills
    {
        public int Id { get; set; }
        public string ser { get; set; }
        [NotMapped]
        public List<Skill> skills { get {
                skills = new List<Skill>();
                return skills;
            }
            set { skills = value; } }
    }
}
