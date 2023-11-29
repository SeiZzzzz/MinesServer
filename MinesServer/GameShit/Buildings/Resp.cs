using MinesServer.Server;
using MinesServer.GameShit.GUI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit.Buildings
{
    public class Resp : Pack
    {
        public Resp()
        {

        }
        public Resp(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Resp) {
            cost = 1000;
            charge = 100;
            maxcharge = 1000;
            using var db = new DataBase();
            db.resps.Add(this);
            db.SaveChanges();
        }
        public void OnRespawn(Player p)
        {
            if (ownerid > 0)
            {
                if (p.money > cost) p.money -= cost;
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
                using var db = new DataBase();
                db.SaveChanges();
                World.W.GetChunk(x, y).ResendPacks();
            }
        }
        [NotMapped]
        public override int off
        {
            get => (charge > 0 ? 1 : 0);
        }
        public (int,int) GetRandompoint()
        {
            var r = new Random();
            return (r.Next(x + 2, x + 5), r.Next(y - 1, y + 3));
        }
        public override void Build()
        {
            World.SetCell(x, y, 37,true);
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
            World.SetCell(x, y + 2, 37);
            for(int xx = x + 2;xx < x + 6;xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, true);
                }
            }
            base.Build();
        }
        public int charge { get; set; }
        public int maxcharge { get; set; }
        public int cost { get; set; }
        public int cid { get; set; }
        public long moneyinside { get; set; }

        public override Window? GUIWin(Player p)
        {
            Action adminaction = (p.Id != ownerid ? null : () =>
            {
                if (p.Id == ownerid)
                {
                    p.win.CurrentTab.Replace(new Page()
                    {
                        Text = " ",
                        RichList = new RichListConfig()
                        {
                            Entries = [new RichListEntry(RichListEntryType.Fill, "Заряд", $"{(charge * 100) / maxcharge}#{charge}/{maxcharge}#1#fill:b_100#fill:b_1000#fill:b_max", "", "fuck", [new Button("h", "kill", (args) => { return; })])
                            ]
                        },
                        Buttons = [new Button("СОХРАНИТЬ", $"save:{ActionMacros.RichList}", (args) => { Console.WriteLine(args.RichList); })]
                    });
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
