using MinesServer.Enums;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit
{
    public class PlayerSkills
    {
        [Key]
        public int id { get; set; }
        public string ser { get; set; } = "";
        public void LoadSkills()
        {
            if (skills.Count < 1)
            {
                skills = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Skill?>>(ser);
            }
        }
        [NotMapped]
        public int selectedslot = -1;
        public void DeleteSkill(Player p)
        {
            if (!skills.ContainsKey(selectedslot))
            {
                return;
            }
            skills[selectedslot] = null;
            p.SendLvl();
            Save();
        }
        public void InstallSkill(string type, int slot, Player p)
        {
            if ((skills.ContainsKey(slot) && skills[slot] != null) || slot > slots || slot < 0)
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
            using var db = new DataBase();
            db.Attach(this);
            ser = Newtonsoft.Json.JsonConvert.SerializeObject(skills, Newtonsoft.Json.Formatting.None);
            db.SaveChanges();
        }
        public Dictionary<SkillType, bool> SkillToInstall()
        {
            Dictionary<SkillType, bool> d = new();
            foreach (var sk in skillz)
            {
                if (skills.FirstOrDefault(skill => skill.Value?.type == sk.type).Value == null)
                {
                    d.Add(sk.type, true);
                }
            }
            return d;
        }
        public int lvlsummary() => skills.Sum(i => i.Value?.lvl ?? 0);
        public UpSkill[] GetSkills()
        {
            List<UpSkill> ski = new();
            LoadSkills();
            foreach (var i in skills)
            {
                if (i.Value != null)
                {
                    ski.Add(new UpSkill(i.Key, i.Value.lvl, i.Value.isUpReady(), i.Value.type));
                }
            }
            return ski.ToArray();
        }
        public int slots { get; set; } = 20;
        [NotMapped]
        public Dictionary<int, Skill?> skills = new();
        [NotMapped]
        public static List<Skill> skillz = new List<Skill>()
        {
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 100f + x * 10,
                    expfunc = (int x) => 1,
                    type = SkillType.Digging, // dick
                    effecttype = SkillEffectType.OnDig
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 5f - x * 0.2f < 0 ? 1f : 5f - x * 0.2f,
                    expfunc = (int x) => 1f,
                    type = SkillType.BuildRoad,
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f + x * 2,
                    expfunc = (int x) => 1f,
                    type = SkillType.BuildGreen,
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f + x * 2,
                    expfunc = (int x) => 1f,
                    type = SkillType.BuildYellow,
                    effecttype = SkillEffectType.OnBld
                },
                 new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f + x * 2f,
                    expfunc = (int x) => 1f,
                    type = SkillType.BuildRed,
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.BuildStructure,
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.Fridge, // охлад
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 0f,
                    effectfunc = (int x) => 90.00f - x * 0.05f,
                    expfunc = (int x) => 1f,
                    type = SkillType.Movement, // движение,
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.RoadMovement, // по дорогам
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.Packing, // упаковка
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => x * 3f,
                    expfunc = (int x) => 1f,
                    type = SkillType.Health, // хп
                    effecttype = SkillEffectType.OnHealth
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingBlue, // упаковка синь
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingCyan, // упаковка голь
                    effecttype = SkillEffectType.OnMove
                },
                 new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingGreen, // упаковка зель
                    effecttype = SkillEffectType.OnMove
                },
                  new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingRed, // упаковка крась
                    effecttype = SkillEffectType.OnMove
                },
                    new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingViolet, // упаковка фиоль
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 1f,
                    expfunc = (int x) => 1f,
                    type = SkillType.PackingWhite, // упаковка бель
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 0.08f + (float)(Math.Log10(x) * (Math.Pow(x, 0.5) / 4)),
                    expfunc = (int x) => 1f,
                    type = SkillType.MineGeneral, // доба
                    effecttype = SkillEffectType.OnDigCrys
                },
                new Skill()
                {
                    costfunc = (int x) => 1f,
                    effectfunc = (int x) => 100f + x * 0.2f,
                    expfunc = (int x) => 1f,
                    type = SkillType.Induction, // инда
                    effecttype = SkillEffectType.OnHurt
                },
                new Skill()
                {
                    costfunc = (int x) =>1f,
                    effectfunc = (int x) =>  (float)Math.Round(1f+(x-((float)Math.Log10(x)*((float)Math.Pow(x,0.9))/2f)-x*0.098f)) >= 92 ? 92 : (float)Math.Round(1f+(x-((float)Math.Log10(x)*((float)Math.Pow(x,0.9))/2f)-x*0.098f)),
                    expfunc = (int x) => 0,
                    type = SkillType.AntiGun, // антипуфка
                    effecttype = SkillEffectType.OnHurt
                }

        };
    }
}
