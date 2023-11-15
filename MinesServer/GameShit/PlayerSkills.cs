using MinesServer.GameShit.Skills;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit
{
    public class PlayerSkills
    {
        public int Id { get; set; }
        public string ser { get; set; }
        public void InstallSkill(string type, int slot)
        {
            skills[slot] = new Skill();
        }
        [NotMapped]
        public List<Skill> skills
        {
            get
            {
                skills = new List<Skill>();
                return skills;
            }
            set { skills = value; }
        }
        public static Dictionary<string, (Func<int, float>, Func<int, float>, Func<int, float>)> skillz = new Dictionary<string, (Func<int, float>, Func<int, float>, Func<int, float>)>()
        {
            //{type,{exp effect cost}}
            {"d",( // dig
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"L",( // bld
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"F",( //depth
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"M",( //move
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"t",( //moveroad
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"p",( //pack
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"l",( //live
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"b",( //packb
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"c",( //packc
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            },
            {"g",( //packg
                float (int x)=> 1f,
                float (int x)=> 1f,
                float (int x)=> 1f
                )
            }

        };
    }
}
