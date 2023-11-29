using MinesServer.GameShit.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.Server;
using MinesServer.GameShit.Marketext;
namespace MinesServer.GameShit.Buildings
{
    public class Market : Pack
    {
        public Market() { }
        public Market(int ownerid,int x,int y) : base(ownerid,x,y,PackType.Market)
        {
            using var db = new DataBase();
            db.markets.Add(this);
            db.SaveChanges();
        }
        public long moneyinside { get; set; }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            for(int xx = -2; xx < 3;xx++)
            {
                for (int yy = -2; yy < 3; yy++)
                {
                    int px = x + xx, py = y + yy;
                    if (px == x || py == y)
                    {
                        World.SetCell(px, py, 37, true);
                        continue;
                    }
                    World.SetCell(px, py, 106, true);
                }
            }
            World.SetCell(x + 2, y + 2, 38, true);
            World.SetCell(x - 2, y + 2, 38, true);
            World.SetCell(x + 2, y - 2, 38, true);
            World.SetCell(x - 2, y - 2, 38, true);
            base.Build();
        }
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                Tabs = [new Tab()
                {
                    InitialPage = MarketSystem.GlobalFirstPage(p)!,
                    Action = "sa",
                    Label = "penis"
                }]
            };
        }
    }
}
