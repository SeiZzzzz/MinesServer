using MinesServer.GameShit.Skills;
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
        public void Up(int slot)
        {
            skills[slot].Up();
        }
        public void InstallSkill(string type, int slot)
        {
            var s = new Skill();
            skills[slot] = skillz.First(i => i.name == type).Clone();
            Save();
        }
        public void Save()
        {
            ser = Newtonsoft.Json.JsonConvert.SerializeObject(skills, Newtonsoft.Json.Formatting.None);
        }
        [NotMapped]
        public Player owner;
        [NotMapped]
        public Skill[] skills = new Skill[35];
        [NotMapped]
        public static List<Skill> skillz = new List<Skill>()
        {
                new Skill()
                {
                    lastexp = 25,
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 100f + x * 10,
                    expfunc = (int x,Skill b) => b.lastexp + ((int)(b.lastexp * 10) / 100),
                    name = "d", // dick
                    effecttype = SkillEffectType.OnDig
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "L", // стройка
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "F", // охлад
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "M", // движение,
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "t", // по дорогам
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "p", // упаковка
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "l", // хп
                    effecttype = SkillEffectType.OnHurt
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "b", // упаковка синь
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "c", // упаковка голь
                    effecttype = SkillEffectType.OnMove
                },
                 new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "g", // упаковка зель
                    effecttype = SkillEffectType.OnMove
                },
                  new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "r", // упаковка крась
                    effecttype = SkillEffectType.OnMove
                },
                    new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "v", // упаковка фиоль
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "w", // упаковка бель
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x,Skill b) => 1f,
                    effectfunc = (int x,Skill b) => 1f,
                    expfunc = (int x,Skill b) => 1f,
                    name = "m", // доба
                    effecttype = SkillEffectType.OnDig
                }

        };
    }
}
