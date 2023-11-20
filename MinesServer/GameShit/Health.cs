using MinesServer.GameShit.Skills;
using MinesServer.Network.BotInfo;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit
{
    public class Health
    {
        public int Id { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        [NotMapped]
        private Player player;
        public void LoadHealth(Player p)
        {
            MaxHP = 100;
            player = p;
            foreach (var c in p.skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, p))
                {
                    if (c.name == "l")
                    {
                        MaxHP += (int)c.GetEffect();
                    }
                }
            }
            HP = HP <= 0 ? MaxHP : HP;
        }
        public void SendHp()
        {
            player.connection.SendU(new LivePacket(HP, MaxHP));
        }
        public void Hurt(int d, DamageType t = DamageType.Pure)
        {
            if (HP - d > 0)
            {
                HP -= d;
                //hurtandresend
            }
        }
    }
}
