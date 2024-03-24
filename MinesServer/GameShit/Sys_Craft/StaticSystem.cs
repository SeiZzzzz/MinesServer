using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.SysMarket;
using MinesServer.Network.GUI;
using MinesServer.Server;

namespace MinesServer.GameShit.Sys_Craft
{
    public static class StaticSystem
    {
        public static string[] crysnames = { "<color=#00e600>зель</color>", "<color=#2929ff>синь</color>", "<color=#ff3333>крась</color>", "фиоль", "бель", "голь" };
        private static InventoryItem[] Items()
        {
            InventoryItem[] items = [];
            for (int i = 0; i < RDes.recipies.Count; i++)
            {
                var cancraft = 1;
                items = items.Append(InventoryItem.Item(RDes.recipies[i].result.id, (cancraft > 0 ? cancraft.ToString() : ""), "", false, InventoryTextColor.Default, InventoryTextColor.Green)).ToArray();
            }
            return items;
        }
        public static void OpenRecipie(Player p, int id)
        {
            var recipie = RDes.recipies.FirstOrDefault(i => i.id == id);
            var text = recipie.costcrys?.Select(i => $"{crysnames[i.id]} x{i.num}").Aggregate("", (str, obj) => str + obj.ToString() + "\n");
            text += recipie.costres?.Select(i => $"{MarketSystem.PackName(i.id)} x{i.num}").Aggregate("", (str, obj) => str + obj.ToString() + "\n");
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"recipie {MarketSystem.PackName(recipie.result.id)}",
                Card = new Card(CardImageType.Item, recipie.result.id.ToString(), $" {MarketSystem.PackName(recipie.result.id)} x{recipie.result.num}\n Время сборки:{recipie.time} сек."),
                Text = $"@@\n\nНужно для сборки четатам\n\n{text}\n\n",
                Input = new InputConfig($"num", null, false),
                Buttons = [new Button("craft", $"craft:{ActionMacros.Input}", (a) => { if (int.TryParse(a.Input, out var num)) Craft(p, recipie, num); })],
            });
        }
        public static void Craft(Player p, Recipie r, int num)
        {
            if (World.ContainsPack(p.x, p.y, out var craft) && MeetReqs(p, r, num))
            {
                DeleteReqs(p, r, num);
                var c = (craft as Crafter);
                using var db = new DataBase();
                db.crafts.Attach(c);
                c.currentcraft = new CraftEntry(r.id, num, DateTime.Now + (TimeSpan.FromSeconds(r.time) * num));
                db.SaveChanges();
                p.win?.CurrentTab.Open(FilledPage(p, c));
                World.W.GetChunk(c.x, c.y).ResendPack(c);
                return;
            }
            p.connection?.SendU(new OKPacket("Недостаточно ресов", "..."));
        }
        public static void Claim(Player p, Crafter c)
        {
            var recipie = c.currentcraft.GetRecipie();
            using var db = new DataBase();
            db.crafts.Attach(c);
            db.players.Attach(p);
            p.inventory[recipie.result.id] += c.currentcraft.num * recipie.result.num;
            db.craftentries.Remove(c.currentcraft);
            c.currentcraft = null;
            db.SaveChanges();
            p.SendInventory();
            World.W.GetChunk(c.x, c.y).ResendPack(c);
            p.win = c.GUIWin(p);
        }
        public static bool MeetReqs(Player p, Recipie r, int num) => (r.costcrys != null ? !r.costcrys.Select(i => { return p.crys.cry[i.id] >= (i.num * num); }).Contains(false) : true) && (r.costres != null ? !r.costres.Select(i => { return p.inventory[i.id] >= (i.num * num); }).Contains(false) : true);
        public static void DeleteReqs(Player p, Recipie r, int num)
        {
            if (r.costcrys != null)
                foreach (var i in r.costcrys)
                    p.crys.cry[i.id] -= i.num * num;
            if (r.costres != null)
                foreach (var i in r.costres)
                    p.inventory[i.id] -= i.num * num;
            p.SendInventory();
            p.crys.SendBasket();
        }
        public static IPage? FilledPage(Player p, Crafter c)
        {
            var progress = c.currentcraft?.progress <= 100 ? c.currentcraft?.progress : 100;
            string bar = "<color=#aaeeaa>" + new string('|', (int)(progress / 2)) + "</color>" + new string('-', 50 - (int)(progress / 2));
            bar += progress == 100 ? " ГОТОВА" : "";
            string remain = progress != 100 ? $"осталось {(c.currentcraft.endtime - DateTime.Now)}" : "осталось нихуя";
            if (c.currentcraft?.progress >= 100)
            {
                return new Page()
                {
                    Title = "Крафтер",
                    Text = $"@@\nprogress {progress}% {bar}\n\n{remain}",
                    Buttons = [new Button("claim", "claim", (a) => Claim(p, c))]
                };
            }
            return new Page()
            {
                Title = "Крафтер",
                Text = $"@@\nprogress {progress}% {bar}\n\n{remain}",
                Buttons = [],
            };
        }
        public static void SecondPage(Player p,int type)
        {
            var lol = RDes.recipies.Where(i => i.result.id == type);
            p.win?.CurrentTab.Open(new Page()
            {
                List = lol.Select(r => new ListEntry(r.result.id.ToString(), new Button("open", $"openrecipie:{r.id}", (arg) => OpenRecipie(p, r.id)))).ToArray(),
                Title = "Крафтер",
                Buttons = [],
            });
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            var oninventory = (int type) => {SecondPage(p, type); };
            return new Page()
            {
                OnInventory = oninventory,
                Inventory = Items(),
                Title = "Крафтер",
                Buttons = [],
            };
        }
    }
}
