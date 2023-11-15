using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Skills
{
    public class Skill
    {
        public int lvl;
        public int slot;
        public int exp;
        public float GetEffect()
        {
            return effectfunc(lvl);
        }
        public float GetExp()
        {
            return expfunc(lvl);
        }
        public float GetCost()
        {
            return costfunc(lvl);
        }
        public bool isUpReady()
        {
            return exp >= expfunc(lvl);
        }
        public SkillEffectType effecttype;
        private Func<int, float> expfunc;
        private Func<int, float> effectfunc;
        private Func<int, float> costfunc;
    }
}
