using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.Server;
using MoreLinq;

namespace MinesServer.GameShit.SysMarket
{
    public static class MarketSystem
    {
        public static string PackName(int i)
        {
            string[] names =
            {
                "","Респаун","UP","Маркет","Хз что это","Плазма","Прото","Разрядка","Кредит","Рембот",
                "Геопак","Геопак с голубой живкой","Геопак с красной живкой","Геопак с фиолетовой живкой","Геопак с чёрной живкой","Геопак с белой живкой","Геопак с синей живкой",
                "","","","TPR","Конст бот","Боевой заряд","Заряд защиты","Крафт","(На рассмотрении)","Пушка","Ворота","Диззамблер","Склад",
                "Сканер","","","","Геопак с гипноскалом","Полимер","Нанобот","Аккумулятор","Транслятор","Компаратор","c190","Фед",
                "Геопак с чёрной скалой","Геопак с красной скалой","Автоматизатор","ЭМИ",
                "Геопак с радужной живкой","Спот","Научный центр"
            };
            if (i >= 0 && names.Length > i)
            {
                return names[i];
            }
            return "";
        }
        public static void Buy(long[] sliders, Player p, Market m)
        {
            if (sliders == null)
            {
                return;
            }
            long money = 0;
            var db = new DataBase();
            for (int i = 0; i < 6; i++)
            {
                if ((p.money + money) - (sliders[i] * (long)(World.GetCrysCost(i) * 10)) > 0 || p.money > -(money - (sliders[i] * (long)(World.GetCrysCost(i) * 10))))
                {
                    if (sliders[i] < (long)1000000000000)
                    {
                        money -= sliders[i] * (World.GetCrysCost(i) * 10);
                        p.crys.AddCrys(i, sliders[i]);
                    }
                    else
                    {
                        money -= sliders[i] * (World.GetCrysCost(i) * 10);
                        p.crys.AddCrys(i, sliders[i]);
                    }
                }
            }
            var page = new Page()
            {
                OnAdmin = (p.Id != m.ownerid ? null : () => m.onadmn(p, m)),
                CrystalConfig = new CrystalConfig(" ", "ЦЕНА   ", [
                            new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(0) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(1) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(2) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(3) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(4) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(5) * 10)), 0)

                            ], true),
                Text = $"Купленно кристаллов на <color=#aaeeaa>{-money}$</color>",
                Buttons = [new Button("КУПИТЬ", $"buy:{ActionMacros.CrystalSliders}", (args) => Buy(args.CrystalSliders, p, m))]
            };
            p.win.CurrentTab.Replace(page);
            p.money += money;
            p.SendWindow();
            db.SaveChanges();
            p.SendMoney();
        }
        public static void Sell(long[] sliders, Player p, Market m)
        {
            long money = 0;
            if (sliders != null)
            {
                var db = new DataBase();
                for (int i = 0; i < 6; i++)
                {
                    if (p.crys.cry[i] > 0)
                    {
                        if (p.crys.cry[i] >= sliders[i])
                        {
                            money += sliders[i] * World.GetCrysCost(i);
                            p.crys.RemoveCrys(i, sliders[i]);
                        }
                    }
                }
                var page = new Page()
                {
                    OnAdmin = (p.Id != m.ownerid ? null : () => m.onadmn(p, m)),
                    CrystalConfig = new CrystalConfig(" ", "ЦЕНА   ", [
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0)}$</color>", 0, 0, p.crys[Enums.CrystalType.Green], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1)}$</color>", 0, 0, p.crys[Enums.CrystalType.Blue], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2)}$</color>", 0, 0, p.crys[Enums.CrystalType.Red], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3)}$</color>", 0, 0, p.crys[Enums.CrystalType.Violet], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4)}$</color>", 0, 0, p.crys[Enums.CrystalType.White], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5)}$</color>", 0, 0, p.crys[Enums.CrystalType.Cyan], 0)]),
                    Text = $"Продано кристаллов на <color=#aaeeaa>{money}$</color>",
                    Buttons = [new Button("ПРОДАТЬ ВСЕ", $"sellall", (args) => Sell(p.crys.cry, p, m)),
                        new Button("ПРОДАТЬ", $"sell:{ActionMacros.CrystalSliders}", (args) => Sell(args.CrystalSliders, p, m))]
                };
                p.win.CurrentTab.Replace(page);
                m.moneyinside += (long)(money * 0.1);
                p.money += money;
                db.SaveChanges();
                p.SendWindow(); 
                p.SendMoney();
            }

        }
        public static void CreateOrder(Player p, int type, int num, int cost)
        {
            if (p.inventory[type] < num | cost < 1 | num < 1)
            {
                p.win = null;
                return;
            }
            p.inventory[type] -= num;
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
                Title = "Аукцион",
                Text = "Лот был успешно создан.",
                Buttons = []
            });
        }
        public static void OpenOrder(Player p, int orderid)
        {
            var db = new DataBase();
            var o = db.orders.First(i => i.id == orderid);
            Player? buyer = null;
            if (o.buyerid > 0)
            {
                buyer = db.players.First(p => p.Id == o.buyerid);
            }
            var cost = buyer == null ? o.cost : o.cost + (o.cost * 0.01f);
            var timer = o.buyerid > 0 ? $"(Осталось {TimeSpan.FromMinutes(5) - (DateTime.Now - o.bettime):mm\\:ss})" : "";
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Лот игрока {p.name} {timer}",
                Text = buyer == null ? null : $"Последняя ставка: {buyer.name}",
                Input = new InputConfig($"Минимум от <color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", null, false),
                Buttons = [new Button("bet", $"bet:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var bet)) { var db = new DataBase(); db.Attach(o); o.Bet(p, bet, orderid); db.SaveChanges(); } OpenOrder(p, orderid); p.SendWindow(); })],
                Card = new Card(CardImageType.Item, o.itemid.ToString(), $"{PackName(o.itemid)} x{o.num} Стоимость <color=#aaeeaa>{o.cost}$</color>"),
            });

        }
        public static ListEntry[] GetItems(Player p, int type)
        {
            ListEntry[] re = [];
            using var db = new DataBase();
            var list = db.orders.Where(o => o.itemid == type);
            foreach (var i in list.OrderBy(it => it.cost))
            {
                if (i.cost != 0)
                {
                    var cost = i.buyerid == 0 ? i.cost : i.cost + (i.cost * 0.01f);
                    re = re.Append(new ListEntry($"{PackName(i.itemid)} x{i.num}", new Button($"<color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", $"openorder:{i.id}", (args) => { OpenOrder(p, i.id); p.SendWindow(); }))).ToArray();
                }
            }
            return re;
        }
        public static void OpenOrdersGui(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ItemNameOrders",
                Buttons = [],
                Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
                List = GetItems(p, itemtype)
            });
        }
        public static void OpenOrderCreation(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Создание лота {PackName(itemtype)}",
                Text = "Enter cost",
                Input = new InputConfig("cost", null, false),
                Buttons = [new Button("Создать ордер", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res) | args.Input != "" | args.Input != " " || args.Input != "" | args.Input != " " | int.Parse(args.Input) > 0) OrderCreationNum(p, itemtype, res); else { p.win = null; } p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
            });
        }
        public static void OrderCreationNum(Player p, int itemtype, int cost)
        {
            if (cost <= 0)
            {
                OpenOrderCreation(p, itemtype);
            }
            else
            {
                p.win?.CurrentTab.Open(new Page()
                {
                    Title = $"Создание лота {PackName(itemtype)}",
                    Text = $"{PackName(itemtype)} to sell count",
                    Input = new InputConfig("num", null, false),
                    Buttons = [new Button("Создать ордер", $"createorder:{ActionMacros.Input}", (args) => {
                    if (int.TryParse(args.Input, out var res)| args.Input != ""| args.Input != " " || int.Parse(args.Input) > 0 | args.Input != "" | args.Input != " " )
                    {
                        CreateOrder(p, itemtype, res, cost);
                    }
                    else
                    {
                        p.win = null; } p.SendWindow();
                    })],
                    Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
                });
            }
        }
        private static InventoryItem[] Items()
        {
            using var db = new DataBase();
            InventoryItem[] items = [];
            for (int i = 1; i < 49; i++)
            {
                if (i == 49| i ==31 | i == 32 | i==33 | i==19 | i==18 | i==17)
                    continue;
                var c = db.orders.Where(z => z.itemid == i).OrderBy(i => i.cost).FirstOrDefault()?.cost.ToString();
                var count = db.orders.Where(order => order.itemid == i).Count();
                if (c == "0") { items = items.Append(InventoryItem.Item(i, "", "", false, InventoryTextColor.Default, InventoryTextColor.Green)).ToArray(); }
                else { items = items.Append(InventoryItem.Item(i, (count > 0 ? count.ToString() : ""), (string.IsNullOrWhiteSpace(c) ? "" : c + "$"), false, InventoryTextColor.Default, InventoryTextColor.Green)).ToArray(); }
            }
            return items;
        }
        public static void OpenItemAuc(Player p, int item)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "Auc " + PackName(item),
                Buttons = [new Button("Создать ордер", "createorder", (args) => { OpenOrderCreation(p, item); p.SendWindow(); })],
                List = GetItems(p, item)
            });
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            var oninventory = (int type) => { OpenItemAuc(p, type); };
            return new Page()
            {
                OnInventory = oninventory,
                Inventory = Items(),
                Title = "МАРКЕТ",
                Buttons = [new Button("ВЫХОД", "exit", (args) => {  })],
            };
        }
    }
}
