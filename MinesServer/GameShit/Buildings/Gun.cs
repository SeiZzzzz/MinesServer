using MinesServer.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Skills;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Numerics;
using System.Windows.Forms.VisualStyles;
namespace MinesServer.GameShit.Buildings
{
    public class Gun : Pack, IDamagable
    {
        #region fields
        public int hp { get; set; }
        public float charge { get; set; }
        public float maxcharge { get; set; }
        public override int cid { get; set; }
        public override int off { get { return charge > 0 ? 1 : 0; } }
        public DateTime brokentimer { get; set; }
        #endregion
        public Gun(int x, int y, int ownerid,int cid) : base(x, y, ownerid, PackType.Gun)
        {
            this.cid = cid;
            hp = 1000;
            charge = 1000;
            maxcharge = 10000;
        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.guns.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[1]++;
            }
        }
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                Tabs = []
            };
        }
        public override void Build()
        {
            World.SetCell(x, y, 32, true);
            World.SetCell(x + 1, y, 35, true);
            World.SetCell(x - 1, y, 35, true);
            World.SetCell(x, y - 1, 35, true);
            World.SetCell(x, y + 1, 35, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            base.Build();
        }
        private void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 35, false);
            World.SetCell(x - 1, y, 35, false);
            World.SetCell(x, y - 1, 35, false);
            World.SetCell(x, y + 1, 35, false);
            World.SetCell(x + 1, y + 1, 106, false);
            World.SetCell(x - 1, y + 1, 106, false);
            World.SetCell(x + 1, y - 1, 106, false);
            World.SetCell(x - 1, y - 1, 106, false);
        }
        public override void Update()
        {
            if (charge == 0)
            {
                return;
            }
            for(int chx = -21;chx <= 21;chx++)
            {
                for (int chy = -21; chy <= 21; chy++)
                {
                    if (Vector2.Distance(new Vector2(x,y),new Vector2(x + chx,y + chy)) <= 20f)
                    {
                        if (World.W.ValidCoord(x + chx,y + chy))
                        {
                            foreach(var player in World.W.GetPlayersFromPos(x + chx, y + chy))
                            {
                                if (player.cid == cid)
                                {
                                    continue;
                                }
                                player.health.Hurt(60, DamageType.Gun);
                                player.SendDFToBots(7, x, y, player.Id, 1);
                                var basecrys = 0.5f;
                                foreach (var c in player.skillslist.skills.Values)
                                {
                                    if (c != null && c.UseSkill(SkillEffectType.OnHurt, player))
                                    {
                                        if (c.type == SkillType.Induction)
                                        {
                                            basecrys *= (c.GetEffect() / 100);
                                        }
                                    }
                                }
                                if (charge - basecrys > 0)
                                {
                                    charge -= basecrys;
                                    continue;
                                }
                                charge = 0;
                                World.W.GetChunk(x, y).ResendPacks();
                            }
                        }
                    }
                }
            }
        }
    }
}
