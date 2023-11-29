using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.Server;
using MoreLinq;
using RT.Util.ExtensionMethods;
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
                "TP","Resp","UP","Market","Clans","boom","prot","raz","Cred","Rembot","geopack","CyanAlive","RedAlive","VioletAlive","BlackNigger","WhiteAlive",
                "BlueAlive","VulcRadar","AliveRadar","BotRadar","TPR","Konstr Bot","Boy gay","Zalupa Zalupa","Crafter","BoomShop","Gun","Gate","Dizz","Storage",
                "PackRadar","x3 up","freeup","mine x4","Gypno","poli","nano bot","accum","transgender","Comp","c190","Fed","NiggerRock","RedRock","AntiMage","EMO",
                "RainbowAlive","spot","NC","Money","Оперативные Порно Покемоны."
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
            var list = db.orders.Where(o => o.itemid == type);
            foreach (var i in list.OrderBy(it => it.cost))
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
        private static InventoryItem[] Items()
        {
            using var db = new DataBase();
            InventoryItem[] items = [];
            for(int i = 0;i < 51;i++)
            {
                if (i == 49)
                    continue;
                var c = db.orders.Where(z => z.itemid == i).OrderBy(i => i.cost).FirstOrDefault()?.cost.ToString();
                var count = db.orders.Where(order => order.itemid == i).Count();
                items = items.Append(InventoryItem.Item(i, (count > 0 ? count.ToString() : ""), (string.IsNullOrWhiteSpace(c) ? "" : c + "$"), false, InventoryTextColor.Default, InventoryTextColor.Green)).ToArray();
            }
            return items;
        }
        public static void OpenItemAuc(Player p,int item)
        {
            p.win.CurrentTab.Open(new Page()
            {
                Title = "Auc " + item.PackName(),
                Buttons = [new Button("Создать Ордер", "createorder", (args) => { OpenOrderCreation(p, item); p.SendWindow(); })],
                List = GetItems(p, item)
            });
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            var oninventory = (int type) => { OpenItemAuc(p, type);p.SendWindow(); };
            return new Page() {
                OnInventory = oninventory,
                Inventory = Items(),
                Title = "МАРКЕТ",
                Buttons = [new Button("Продать кри", "sell", (args) => { Console.WriteLine("sell"); })],
            };
        }
    }
}
