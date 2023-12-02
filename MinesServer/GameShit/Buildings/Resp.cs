using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit.Buildings
{
    public class Resp : Pack, IDamagable
    {
        #region fields
        public int charge { get; set; }
        public int maxcharge { get; set; }
        public int cost { get; set; }
        public override int cid { get; set; }
        public long moneyinside { get; set; }
        public int hp { get; set; }
        public DateTime brokentimer { get; set; }
        #endregion
        public Resp()
        {

        }
        public Resp(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Resp)
        {
            cost = 1000;
            charge = 100;
            maxcharge = 1000;
            hp = 100;
            using var db = new DataBase();
            db.resps.Add(this);
            db.SaveChanges();
        }
        public void OnRespawn(Player p)
        {
            using var db = new DataBase();
            db.Attach(this);
            if (ownerid > 0)
            {
                if (p.money > cost)
                {
                    p.money -= cost;
                    moneyinside += cost;
                }
                else
                {
                    p.RandomResp();
                    p.GetCurrentResp()?.OnRespawn(p);
                }
                if (charge > 0) charge--;
                else
                {
                    p.RandomResp();
                    p.GetCurrentResp()?.OnRespawn(p);
                }
                p.SendMoney();
                World.W.GetChunk(x, y).ResendPacks();
            }
            db.SaveChanges();
        }
        [NotMapped]
        public override int off
        {
            get => (charge > 0 ? 1 : 0);
        }
        public (int, int) GetRandompoint()
        {
            var r = new Random();
            return (r.Next(x + 2, x + 5), r.Next(y - 1, y + 3));
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 37, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x + 1, y + 2, 106, true);
            World.SetCell(x - 1, y + 2, 106, true);
            World.SetCell(x, y + 2, 37,true);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, true);
                }
            }
            base.Build();
        }
        public void Fill(Player p, long num)
        {
            using var db = new DataBase();
            if (p.crys[Enums.CrystalType.Blue] < num)
            {
                num = p.crys[Enums.CrystalType.Blue];
            }
            db.Attach(this);
            if (p.crys.RemoveCrys((int)Enums.CrystalType.Blue, num))
            {
                charge += (int)num;
            }
            p.win?.CurrentTab.Replace(AdmnPage(p));
            p.SendWindow();
            db.SaveChanges();
        }
        public void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, false);
                }
            }
        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.resps.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[1]++;
            }
        }
        public void AdminSaveChanges(Player p,Dictionary<string, string> d)
        {
            if (bool.TryParse(d["clan"], out var clan))
            {
                if (MServer.GetPlayer(ownerid) != null)
                {
                    cid = clan ? MServer.GetPlayer(ownerid).clanid : 0;
                }
            }
            if (int.TryParse(d["cost"], out var costs))
            {
                cost = costs;
            }
            if (int.TryParse(d["clanzone"],out var clanz))
            {
                clanzone = clanz;
            }
        }
        public int clanzone { get; set; }
        private IPage AdmnPage(Player p)
        {
            Button[] fillbuttons = [p.crys[Enums.CrystalType.Blue] >= 100 ? new Button("+100", "fill:100", (args) => Fill(p, 100)) : new Button("+100", "fill:100"),
                p.crys[Enums.CrystalType.Blue] >= 1000 ? new Button("+1000", "fill:1000", (args) => Fill(p, 1000)) : new Button("+1000", "fill:1000"),
                p.crys[Enums.CrystalType.Blue] >= 0 ? new Button("max", "fill:max", (args) => Fill(p, maxcharge - charge)) : new Button("max", "fill:max")
               ];
            return new Page()
            {
                Text = " ",
                RichList = new RichListConfig()
                {
                    Entries = [RichListEntry.Fill("заряд", charge, maxcharge, Enums.CrystalType.Blue, fillbuttons[0], fillbuttons[1], fillbuttons[2]),
                        RichListEntry.Text("hp"),
                        RichListEntry.UInt32("cost", "cost", (uint)cost),
                        RichListEntry.ButtonLine($"прибыль {moneyinside}$", moneyinside == 0 ? new Button() : new Button("Получить", "getprofit", (args) => { using var db = new DataBase(); p.money += moneyinside; moneyinside = 0; p.SendMoney(); db.SaveChanges();p.win?.CurrentTab.Replace(AdmnPage(p)); p.SendWindow(); })),
                        RichListEntry.Bool("Клановый респ", "clan", cid > 0),
                        RichListEntry.UInt32("clanzone", "clanzone", (uint)clanzone)
                            ]
                },
                Buttons = [new Button("СОХРАНИТЬ", $"save:{ActionMacros.RichList}", (args) => { AdminSaveChanges(p,args.RichList); })]
            };
        }
        public override Window? GUIWin(Player p)
        {
            Action adminaction = (p.Id != ownerid && p.clanid != cid ? null : () =>
            {
                if (p.Id == ownerid)
                {
                    p.win?.CurrentTab.Open(AdmnPage(p));
                }
            })!;
            Page page = p.respid != id ? new Page()
            {
                OnAdmin = adminaction,

                Text = $"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\nЦена восстановления: <color=green>${cost}</color>\n\n<color=#f88>Привязать робота к респу?</color>",
                Buttons = [new Button("ПРИВЯЗАТЬ", "bind", (args) =>
                {
                    p.SetResp(this);
                    p.win = GUIWin(p)!;
                })]
            } : new Page()
            {
                OnAdmin = adminaction,
                Text = $"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\nЦена восстановления: <color=green>${cost}</color>\n\n<color=#8f8>Вы привязаны к этому респу.</color>",
                Buttons = []
            };

            return new Window()
            {
                Title = "РЕСП",
                Tabs = [
                    new Tab()
                    {
                        Label = "РЕСП",
                        Action = "resp",
                        InitialPage = page
                    }
                ]
            };
        }
    }
}
