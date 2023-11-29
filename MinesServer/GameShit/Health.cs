﻿using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Skills;
using MinesServer.Network.BotInfo;
using MinesServer.Server;
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
        public void Death()
        {
            if (player.crys.AllCry > 0)
            {
                Box.BuildBox(player.x, player.y, player.crys.cry, player);
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
            player.MoveToChunk(player.ChunkX, player.ChunkY);
            player.SendMap();
            player.tp(player.x, player.y);
        }
        public void Hurt(int d, DamageType t = DamageType.Pure)
        {
            foreach (var c in player.skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, player))
                {
                    if (c.type == Enums.SkillType.Health)
                    {
                        c.AddExp(player);
                    }
                }
                else if (c != null && c.UseSkill(SkillEffectType.OnHurt, player))
                {
                     c.AddExp(player);
                }
            }
            if (HP - d > 0)
            {
                HP -= d;
                player.SendDFToBots(6, 0, 0, 0, 0);
            }
            else
            {
                Death();
            }
            SendHp();
        }
    }
}
