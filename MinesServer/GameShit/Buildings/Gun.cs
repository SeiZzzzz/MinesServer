using MinesServer.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.Skills;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Numerics;
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
        public Gun(int x, int y, int ownerid, int cid) : base(x, y, ownerid, PackType.Gun)
        {
            this.cid = cid;
            hp = 1000;
            charge = 1000;
            maxcharge = 10000;
        }
        private Gun()
        {

        }
        #region affectworld
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
        protected override void ClearBuilding()
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
        #endregion
        public void Fill(Player p, long val)
        {
            if (charge == maxcharge)
            {
                return;
            }
            using var db = new DataBase();
            db.Attach(this);
            if (p.crys[CrystalType.Cyan] < val)
            {
                val = p.crys[CrystalType.Cyan];
            }
            if (p.crys.RemoveCrys((int)CrystalType.Cyan, val))
            {
                charge += (int)val;
                World.W.GetChunk(x, y).ResendPacks();
            }
            db.SaveChanges();
            p.win = GUIWin(p);
            p.SendWindow();
        }
        public override Window? GUIWin(Player p)
        {
            Button[] fillbuttons = [p.crys[CrystalType.Cyan] >= 100 ? new Button("+100", "fill:100", (args) => Fill(p, 100)) : new Button("+100", "fill:100"),
                p.crys[CrystalType.Cyan] >= 1000 ? new Button("+1000", "fill:1000", (args) => Fill(p, 1000)) : new Button("+1000", "fill:1000"),
                p.crys[CrystalType.Cyan] >= 0 ? new Button("max", "fill:max", (args) => Fill(p, (long)(maxcharge - charge))) : new Button("max", "fill:max")
               ];
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "gun",
                    Label = "хуй",
                    Title = "Пушка",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = [RichListEntry.Fill("заряд", (int)charge, (int)maxcharge, CrystalType.Cyan, fillbuttons[0], fillbuttons[1], fillbuttons[2])]
                        },
                        Buttons = []
                    }
                }]
            };
        }
        public override void Update()
        {
            if (charge == 0)
            {
                return;
            }
            for (int chx = -21; chx <= 21; chx++)
            {
                for (int chy = -21; chy <= 21; chy++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(x + chx, y + chy)) <= 20f)
                    {
                        if (World.W.ValidCoord(x + chx, y + chy))
                        {
                            foreach (var player in World.W.GetPlayersFromPos(x + chx, y + chy))
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
                                            basecrys *= (c.Effect / 100);
                                        }
                                    }
                                }
                                if (charge - basecrys > 0)
                                {
                                    charge -= basecrys;
                                    continue;
                                }
                                charge = 0;
                                World.W.GetChunk(x, y).ResendPack(this);
                            }
                        }
                    }
                }
            }
        }
    }
}
