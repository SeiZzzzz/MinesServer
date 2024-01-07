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
            foreach (var c in p.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, p))
                {
                    if (c.type == Enums.SkillType.Health)
                    {
                        MaxHP += (int)c.GetEffect();
                    }
                }
            }
            HP = HP <= 0 ? MaxHP : HP;
        }
        public void SendHp()
        {
            LoadHealth(player);
            player.connection?.SendU(new LivePacket(HP, MaxHP));
        }
        public (int, int) FindEmptyForBox(int x, int y)
        {
            var dirs = new (int, int)[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
            var q = new Queue<(int, int)>();
            if (!World.IsEmpty(x, y))
            {
                q.Enqueue((x, y));
            }
            while (q.Count > 0)
            {
                var b = q.Dequeue();
                foreach (var dir in dirs)
                {
                    if (!World.IsEmpty(b.Item1 + dir.Item1, b.Item2 + dir.Item2))
                    {
                        q.Enqueue((b.Item1 + dir.Item1, b.Item2 + dir.Item2));
                    }
                    else
                    {
                        return (b.Item1 + dir.Item1, b.Item2 + dir.Item2);
                    }
                }
            }
            return (x, y);
        }
        public void Death()
        {
            if (player.crys.AllCry > 0)
            {
                var c = FindEmptyForBox(player.x, player.y);
                Box.BuildBox(c.Item1, c.Item2, player.crys.cry, player);
                player.crys.ClearCrys();
            }
            player.win = null;
            player.SendWindow();
            player.SendFXoBots(2, player.x, player.y);
            HP = MaxHP;
            var r = player.GetCurrentResp()!;
            r.OnRespawn(player);
            r = player.GetCurrentResp()!;
            var newpos = r.GetRandompoint();
            player.x = newpos.Item1; player.y = newpos.Item2;
            player.tp(player.x, player.y);
            player.SendMap();
            SendHp();
        }
        public void Hurt(int d, DamageType t = DamageType.Pure)
        {
            if (player is null) return;
            foreach (var c in player.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, player))
                {
                    if (c.type == Enums.SkillType.Health)
                    {
                        c.AddExp(player);
                    }
                }
                if (c != null && c.UseSkill(SkillEffectType.OnHurt, player) && t == DamageType.Gun)
                {
                    if (c.type == Enums.SkillType.Induction)
                    {
                        c.AddExp(player);
                    }
                    if (c.type == Enums.SkillType.AntiGun)
                    {
                        c.AddExp(player);
                        var eff = (int)(d * (c.GetEffect() / 100));
                        if (d - eff >= 0)
                        {
                            d -= eff;
                        }
                        else
                        {
                            d = 0;
                        }
                    }
                }
            }
            if (HP - d > 0)
            {
                HP -= d;
                player.SendDFToBots(6, 0, 0, player.Id, 0);
            }
            else
            {
                Death();
            }
            SendHp();
        }
    }
}
