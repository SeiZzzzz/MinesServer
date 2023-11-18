using MinesServer.Network.GUI;

namespace MinesServer.GameShit.Skills
{
    public class Skill 
    {
        public int lvl = 1;
        public float exp = 0;
        public string name;
        public float GetEffect()
        {
            effectfunc ??= PlayerSkills.skillz.First(i => i.name == name).effectfunc;
            return effectfunc(lvl,this);
        }
        public float GetExp()
        {
            expfunc ??= PlayerSkills.skillz.First(i => i.name == name).expfunc;
            return expfunc(lvl, this);
        }
        public float GetCost()
        {
            costfunc ??= PlayerSkills.skillz.First(i => i.name == name).costfunc;
            return costfunc(lvl, this);
        }
        public Skill Clone()
        {
            return MemberwiseClone() as Skill;
        }
        public void Up()
        {
            if (isUpReady())
            {
                lastexp = GetExp();
                lasteff = GetEffect();
                lastcost = GetCost();
                lvl += 1;
                exp -= GetExp();
            }
        }
        public void AddExp(Player p)
        {
            Dictionary<string, int> v = new();
            exp += 1;
            v.Add(this.name, (int)((exp * 100f) / GetExp()));
            p.connection.SendU(new SkillsPacket(v));
        }
        public bool UseSkill(SkillEffectType e,Player p)
        {
            if (e == effecttype)
            {
                AddExp(p);
                p.skillslist.Save();
                return true; 
            }
            return false; 
        }
        public bool isUpReady()
        {
            return exp >= GetExp();
        }
        public SkillEffectType EffectType()
        {
            return PlayerSkills.skillz.First(i => i.name == name).effecttype;
        }
        public float lastexp;
        public float lasteff;
        public float lastcost;
        [NonSerialized]
        public SkillEffectType effecttype;
        [NonSerialized]
        public Func<int,Skill, float> expfunc;
        [NonSerialized]
        public Func<int, Skill, float> effectfunc;
        [NonSerialized]
        public Func<int, Skill, float> costfunc;
    }
}
