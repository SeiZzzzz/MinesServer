using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MinesServer.GameShit.Marketext
{
    public static class MarketSystem
    {
        public static string PackName(this int i)
        {
            string[] names =
            {
                "TP","Resp","UP","Market"
            };
            return names[i];
        }
        public static void CreateOrder(Player p,int type,int num,int cost)
        {
            if (p.inventory.items[type] < num)
            {
                p.win = null;
                return;
            }
            p.inventory.items[type] -= num;
            using var db = new DataBase();
            var order = new Order()
            {
                initiatorid = p.Id,
                cost = cost,
                num = num,
                itemid = type
            };
            db.orders.Add(order);
            db.SaveChanges();
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ok",
                Text = "u just created order u can cancel it within five mins after first bet",
                Buttons = []
            });
        }
        public static void OpenOrder(Player p,int orderid)
        {
            var db = new DataBase();
            var o = db.orders.First(i => i.id == orderid);
            Player? buyer = null;
            if (o.buyerid > 0)
            {
                buyer = db.players.First(p => p.Id == o.buyerid);
            }
            var cost = buyer == null ? o.cost : o.cost + (o.cost * 0.01f);
            var timer = o.buyerid > 0 ? $"(time till ends {TimeSpan.FromMinutes(5) - (DateTime.Now - o.bettime):mm\\:ss})" : "";
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order of player {p.name} {timer}",
                Text = buyer == null ? null : $"last bet by: {buyer.name}",
                Input = new InputConfig($"minimal bet is <color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", null, false),
                Buttons = [new Button("bet", $"bet:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var bet)) { var db = new DataBase(); db.Attach(o); o.Bet(p, bet); db.SaveChanges(); } OpenOrder(p, orderid); p.SendWindow(); })],
                Card = new Card(CardImageType.Item, o.itemid.ToString(), $"{o.itemid.PackName()} x{o.num} costs <color=#aaeeaa>{o.cost}$</color>"),
            });

        }
        public static ListEntry[] GetItems(Player p,int type)
        {
            ListEntry[] re = [];
            using var db = new DataBase();
            foreach (var i in db.orders.Where(o => o.itemid == type))
            {
                var cost = i.buyerid == 0 ? i.cost : i.cost + (i.cost * 0.01f);
                re = re.Append(new ListEntry($"{i.itemid.PackName()} x{i.num}", new Button($"<color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", $"openorder:{i.id}", (args) => { OpenOrder(p, i.id); p.SendWindow(); }))).ToArray();
            }
            return re;
        }
        public static void OpenOrdersGui(Player p,int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ItemNameOrders",
                Buttons = [],
                Card = new Card(CardImageType.Item, itemtype.ToString(), itemtype.PackName()),
                List = GetItems(p,itemtype)
            });
        }
        public static void OpenOrderCreation(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {itemtype.PackName()}",
                Text = "Enter cost",
                Input = new InputConfig("cost", null, false),
                Buttons = [new Button("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) OrderCreationNum(p,itemtype, res); else p.win = null; p.SendWindow();  })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), itemtype.PackName()),
            });
        }
        public static void OrderCreationNum(Player p,int itemtype,int cost)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {itemtype.PackName()}",
                Text = $"{itemtype.PackName()} to sell count",
                Input = new InputConfig("num", null, false),
                Buttons = [new Button("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) CreateOrder(p, itemtype, res, cost); else p.win = null; p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), itemtype.PackName()),
            });
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            return new Page() {
                Title = "Auc " + 2.PackName(),
                Buttons = [new Button("Продать кри", "sell", (args) => { OpenOrderCreation(p, 2); })],
                List = GetItems(p,2)
            };
        }
    }
}
