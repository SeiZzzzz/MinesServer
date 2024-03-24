using MinesServer.Enums;
using MinesServer.Network.GUI;

namespace MinesServer.GameShit.Skills
{
    public class Skill
    {
        public int lvl = 1;
        public float exp = 0;
        public float count = 0;
        public SkillType type;
        public float GetEffect()
        {
            effectfunc ??= PlayerSkills.skillz.First(i => i.type == type).effectfunc;
            return (float)Math.Round(effectfunc(lvl), 2);
        }
        public float GetExp()
        {
            expfunc ??= PlayerSkills.skillz.First(i => i.type == type).expfunc;
            return expfunc(lvl);
        }
        public float GetDop()
        {
            expfunc ??= PlayerSkills.skillz.First(i => i.type == type).dopfunc;
            return dopfunc(lvl);
        }
        public float GetCost()
        {
            costfunc ??= PlayerSkills.skillz.First(i => i.type == type).costfunc;
            return costfunc(lvl);
        }
        public Skill Clone()
        {
            return MemberwiseClone() as Skill;
        }
        public void Up(Player p)
        {
            if (isUpReady())
            {
                if (p.money - (long)GetCost() < (long)100000000000000)
                { if (p.money - (long)GetCost() > (long)0)
                    {
                        if (exp - GetExp() >= 0)
                        {
                            Dictionary<string, int> v = new();
                            exp = exp - GetExp();
                            lvl += 1;

                            p.money -= (long)GetCost();
                            v.Add(type.GetCode(), (int)((exp * 100f) / GetExp()));


                            p.skillslist.Save(); 
                            p.connection?.SendU(new SkillsPacket(v));
                            p.SendLvl();
                            p.SendMoney();
                            p.SendSpeed();
                            p.health.SendHp();
                        }
                    }
                }
            }
        }
        public void AddExp(Player p, float expv = 1)
        {
            Dictionary<string, int> v = new();
            foreach (var i in p.skillslist.skills.Values)
            {
                if (UseSkill(SkillEffectType.OnExp, p))
                {
                    if (i.type == SkillType.Upgrade)
                    {
                        expv *= i.GetEffect();
                    }
                }
            }
            exp += expv;
            v.Add(type.GetCode(), (int)((exp * 100f) / GetExp()));
            p.connection?.SendU(new SkillsPacket(v));
            p.skillslist.Save();
        }
        public bool UseSkill(SkillEffectType e, Player p)
        {
            if (e == EffectType())
            {
                return true;
            }
            return false;
        }
        public string Description()
        {
            return $"lvl:{lvl} effect:{GetEffect()} cost:{GetCost()} exp:{exp}/{GetExp()}";
        }
        public bool isUpReady()
        {
            return exp >= GetExp();
        }
        public SkillEffectType EffectType()
        {
            return PlayerSkills.skillz.First(i => i.type == type).effecttype;
        }
        [NonSerialized]
        public SkillEffectType effecttype;
        [NonSerialized]
        public Func<int, float> expfunc;
        [NonSerialized]
        public Func<int, float> effectfunc;
        [NonSerialized]
        public Func<int, float> costfunc;
        [NonSerialized]
        public Func<int, float> dopfunc = null;
    }
}
