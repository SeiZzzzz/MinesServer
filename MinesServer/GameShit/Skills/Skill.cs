namespace MinesServer.GameShit.Skills
{
    public class Skill 
    {
        public int lvl = 1;
        public int exp = 0;
        public string name;
        public float GetEffect()
        {
            effectfunc ??= PlayerSkills.skillz.First(i => i.name == name).effectfunc;
            return effectfunc(lvl);
        }
        public float GetExp()
        {
            expfunc ??= PlayerSkills.skillz.First(i => i.name == name).expfunc;
            return expfunc(lvl);
        }
        public float GetCost()
        {
            costfunc ??= PlayerSkills.skillz.First(i => i.name == name).costfunc;
            return costfunc(lvl);
        }
        public Skill Clone()
        {
            return MemberwiseClone() as Skill;
        }
        public void Up()
        {
            if (isUpReady())
            {
                lvl += 1;
                exp = 0;
            }
        }
        public void AddExp()
        {
            exp += 1;
        }
        public bool UseSkill(SkillEffectType e,Player p)
        {
            if (e == effecttype)
            {
                AddExp();
                p.skillslist.Save();
                return true;
            }
            return true;
        }
        public bool isUpReady()
        {
            return exp >= GetExp();
        }
        public SkillEffectType EffectType()
        {
            return PlayerSkills.skillz.First(i => i.name == name).effecttype;
        }
        [NonSerialized]
        public SkillEffectType effecttype;
        [NonSerialized]
        public Func<int, float> expfunc;
        [NonSerialized]
        public Func<int, float> effectfunc;
        [NonSerialized]
        public Func<int, float> costfunc;
    }
}
