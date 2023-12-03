using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.ClanSystem
{
    public class Clan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int id { get; set; }
        [NotMapped]
        private List<int> memberlist = null;
        public string membersbd { get; set; } = "";
        public int ownerid { get; set; }
        public string name { get; set; }
        public string abr { get; set; }
        public Clan()
        {
        }
        public List<int> GetMemberlist()
        {
            if (memberlist == null)
            {
                memberlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(membersbd);
            }
            return memberlist;
        }
        public static InventoryItem[] ClanIcons()
        {
            using var db = new DataBase(); List<InventoryItem> l = new();
            for (int i = 0;i < 8;i++)
            {
                var r = Physics.r.Next(1, 219);
                if (db.clans.FirstOrDefault(i => i.id == r) == null && l.FirstOrDefault(i => i.Id == r) == default)
                {
                    l.Add(InventoryItem.Clan((byte)r, 0));
                }
            }
            return l.ToArray();
        }
        public static void CreateClan(Player p,int icon,string name,string abr)
        {
            using var db = new DataBase();
            if (db.clans.FirstOrDefault(i => i.id == icon) == null)
            {
                db.Add(new Clan() { ownerid = p.Id,id = icon, abr = abr, name = name });
                p.clanid = icon;
                p.SendClan();
            }
            db.SaveChanges();
        }
        public static void ChooseIcon(Player p)
        {
            var goingtoend = (Player p, int icon, string name,string abr) =>
            {
                p.win?.CurrentTab.Open(new Page()
                {
                    Text = "@@\nВсе готово для создания клана.Остался последний этап.\n\n <color=#ff8888ff>Условия:</color>\n1. При создании спишется залог 1000 кредитов.\n2. При удалении клана 90% залога возвращается.\n3. При неактивности игроков в течение 2 месяцев клан удаляется.\n4. Мультоводство в игре запрещено. Использование нескольких\nаккаунтов одним человеком может повлечь штраф и санкции вплоть\nдо бана аккаунтов и удаления клана.\n",
                    Title = "ЗАВЕРШЕНИЕ СОЗДАНИЯ КЛАНА",
                    Card = new Card(CardImageType.Clan, icon.ToString(), $"<color=white>{name}[{abr}]</color>\n"),
                    Buttons = [new Button("<color=#ff8888ff>ПРИНИМАЮ УСЛОВИЯ</color>", $"complete", (args) => CreateClan(p,icon,name,abr))]
                });
                p.SendWindow();
            };
            var abrchoose = (Player p,int icon,string name) =>
            {
                p.win?.CurrentTab.Open(new Page()
                {
                    Text = "@@\nВыберите краткое имя клана, заглавными латинскими буквами.\n1-3 буквы. Оно используется в списках, командах консоли и пр.\n\nНапример, Хр@нители - HRA, Герои Меча - GRM\nВыберите сокращение, по которому легко узнать ваш клан.\n",
                    Title = "СОЗДАНИЕ АББРЕВИАТУРЫ",
                    Card = new Card(CardImageType.Clan, icon.ToString(), $"<color=white>{name}</color>\n"),
                    Input = new InputConfig()
                    {
                        IsConsole = false,
                        Placeholder = "XXX",
                        MaxLength = 3
                    },
                    Buttons = [new Button("Далее", $"next:{ActionMacros.Input}", (args) => goingtoend(p, icon, name, args.Input))]
                });
                p.SendWindow();
            };
            var namechoose = (Player p,int iconid) =>
            {
                p.win?.CurrentTab.Open(new Page()
                {
                    Title = "ВЫБОР НАЗВАНИЯ КЛАНА",
                    Text = "@@\nВыберите название клана.\nВ игре есть модерация, оскорбительные кланы могут быть удалены!\n\nВнимание! Название клана нельзя будет изменить после создания.\n",
                    Input = new InputConfig()
                    {
                        IsConsole = false,
                        Placeholder = "clanname"
                    },
                    Buttons = [new Button("Продолжить", $"namechoose:{ActionMacros.Input}", (args) => abrchoose(p, iconid, args.Input))]
                });
                p.SendWindow();
            };
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ВЫБОР ЗНАЧКА КЛАНА",
                Text = "@@Выберите значок клана. Всего значков больше сотни. Для удобства мы\nпоказываем их небольшими порциями. Нажмите ДРУГИЕ, чтобы посмотреть еще.\nДля выбора значка - кликните на него.\n\nВнимание! Значок клана нельзя будет изменить после создания.\n",
                Buttons = [new Button("Другие", "nexticons", (args) => ChooseIcon(p))],
                Inventory = ClanIcons(),
                OnInventory = (i) => namechoose(p,i - 200)
            });
            p.SendWindow();
        }
        public static void OpenCreateWindow(Player p)
        {
            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Label = "",
                    Action = "clancreate:1",
                    InitialPage = new Page()
                    {
                        Text = "@@\nУра! Вы собираетесь создать новый клан. После создания клана вы сможете\nвыполнять клановые квесты, создавать свои фермы, вести войны с другими\nкланами, защищать и отбивать территории, и многое другое.\n\nСоздание клана - ответственное действие, значок и название клана нельзя\nбудет изменить позже. Поэтому внимательно подумайте над тем, как будет\nзвучать и выглядеть ваш клан в игре.\n\nСоздание клана требует залога в 1000 кредитов.\n",
                        Buttons = [new Button("ВЫБРАТЬ ЗНАЧОК КЛАНА", "chooseicon", (args) => ChooseIcon(p))],
                        
                    }
                }]
            };
            p.SendWindow();
        }
    }
}
