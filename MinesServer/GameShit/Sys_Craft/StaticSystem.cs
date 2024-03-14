using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.SysMarket;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Sys_Craft
{
    public static class StaticSystem
    {
        public static string[] crysnames = { "зель", "синь", "крась", "фиоль", "бель", "голь" };
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
        public static void OpenRecipie(Player p,int result_id)
        {
            var recipie = RDes.recipies.FirstOrDefault(i => i.result.id == result_id);
            var text = recipie.costcrys?.Select(i => $"{crysnames[i.id]} x{i.num}").Aggregate("", (str, obj) => str + obj.ToString() + "\n");
            text += recipie.costres?.Select(i => $"{MarketSystem.PackName(i.id)} x{i.num}").Aggregate("", (str, obj) => str + obj.ToString() + "\n");
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"recipie {MarketSystem.PackName(result_id)}",
                Card = new Card(CardImageType.Item, result_id.ToString(), $" {MarketSystem.PackName(recipie.result.id)} x{recipie.result.num}\n Время сборки:{recipie.time} сек."),
                Text = $"@@\n\nНужно для сборки четатам\n\n{text}\n\n",
                Input = new InputConfig($"num", null, false),
                Buttons = [new Button("craft", $"craft:{ActionMacros.Input}", (a) => {if (int.TryParse(a.Input,out var num)) Craft(p,recipie,num); })],
            });
        }
        public static void Craft(Player p,Recipie r,int num)
        {
            if (World.ContainsPack(p.x,p.y,out var craft))
            {
                var c = (craft as Crafter);
                using var db = new DataBase();
                db.crafts.Attach(c);
                c.currentcraft = new CraftEntry(r.result.id,num,DateTime.Now + (TimeSpan.FromSeconds(r.time) * num));
                db.SaveChanges();
            }
        }
        public static IPage? FilledPage(Player p,Crafter c)
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
                    Buttons = [new Button("claim", "claim", (a) => { })]
                };
            }
            return new Page()
            {
                Title = "Крафтер",
                Text = $"@@\nprogress {progress}% {bar}\n\n{remain}",
                Buttons = [],
            };
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            var oninventory = (int type) => {  OpenRecipie(p, type); };
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
