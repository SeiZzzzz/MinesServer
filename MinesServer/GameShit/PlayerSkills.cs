using MinesServer.Enums;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit
{
    public class PlayerSkills
    {
        public int Id { get; set; }
        public string ser { get; set; }
        public void LoadSkills()
        {
            if (!string.IsNullOrWhiteSpace(ser))
            {
                skills = Newtonsoft.Json.JsonConvert.DeserializeObject<Skill[]>(ser);
            }
            else
            {
                ser = "";
            }
        }
        public void SelectSlot(Player p)
        {
            
        }
        [NotMapped]
        public int selectedslot = -1;
        public void DeletetSkill(Player p)
        {
            skills[selectedslot] = null;
            p.SendLvl();
            Save();
        }
        public void InstallSkill(string type, int slot,Player p)
        {
            if (skills.FirstOrDefault(i => i?.type.GetCode() == type) != null)
             {
                return;
            }
            var s = new Skill();
            skills[slot] = skillz.First(i => i.type.GetCode() == type).Clone();
            p.SendLvl();
            Save();
        }
        public void Save()
        {
            ser = Newtonsoft.Json.JsonConvert.SerializeObject(skills, Newtonsoft.Json.Formatting.None);
            using var db = new DataBase();
            db.SaveChanges();
        }
        public Dictionary<SkillType,bool> SkillToInstall()
        {
            Dictionary<SkillType, bool> d = new();
            foreach (var sk in skillz)
            {
                d.Add(sk.type, true);
            }
            return d;
        }
        public int lvlsummary() => skills.Sum(i => i?.lvl ?? 0);
        public UpSkill[] GetSkills()
        {
            List<UpSkill> ski = new();
            for(int i = 0; i < skills.Length;i++)
            {
                if (skills[i] != null)
                {
                    ski.Add(new UpSkill(i, skills[i].lvl, skills[i].isUpReady(), skills[i].type));
                    continue;
                }
            }
            return ski.ToArray();
        }
        [NotMapped]
        public Skill?[] skills = new Skill[35];
        [NotMapped]
        public static List<Skill> skillz = new List<Skill>()
        {
                new Skill()
                {
                    lastexp = 25,
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 100f + x * 10,
                    expfunc = (int x,Skill b) => b.lastexp + ((int)(b.lastexp * 10) / 100),
                    type = SkillType.Digging, // dick
                    effecttype = SkillEffectType.OnDig
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.BuildGreen, // стройка
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.Fridge, // охлад
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.Movement, // движение,
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.RoadMovement, // по дорогам
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.Packing, // упаковка
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => x * 3f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.Health, // хп
                    effecttype = SkillEffectType.OnHealth
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingBlue, // упаковка синь
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingCyan, // упаковка голь
                    effecttype = SkillEffectType.OnMove
                },
                 new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingGreen, // упаковка зель
                    effecttype = SkillEffectType.OnMove
                },
                  new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingRed, // упаковка крась
                    effecttype = SkillEffectType.OnMove
                },
                    new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingViolet, // упаковка фиоль
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    type = SkillType.PackingWhite, // упаковка бель
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    lastexp = 700,
                    lastcost = 1100,
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 0.08f + (float)(Math.Log10(x) * (Math.Pow(x, 0.5) / 4)),
                    expfunc = (int x,Skill b) => 0,
                    type = SkillType.MineGeneral, // доба
                    effecttype = SkillEffectType.OnDigCrys
                }

        };
    }
}
