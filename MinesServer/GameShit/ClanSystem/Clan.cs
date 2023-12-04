using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Marketext;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace MinesServer.GameShit.ClanSystem
{
    public class Clan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int id { get; set; }
        public virtual ICollection<Request> reqs { get; set; } = new List<Request>();
        public virtual ICollection<Player> members { get; set; } = new List<Player>();
        public virtual ICollection<Rank> ranks { get; set; } = new List<Rank>();
        public int ownerid { get; set; }
        public string name { get; set; }
        public string abr { get; set; }
        public Clan()
        {
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
        public void OpenClanWin(Player p)
        {
            Button[] buttons = [new Button("leave", "leave",(args) => LeaveClan(p))];
            if (p.Id == ownerid && members.Count > 1)
            {
                buttons = [];
            }
            Tab[] tabs = [new Tab()
            {
                Action = "view",
                Label = "Обзор",
                InitialPage = new Page()
                {
                    Card = new Card(CardImageType.Clan, id.ToString(), $"<color=white>{name}[{abr}]</color>\nУчастники: <color=white>{members.Count}</color>"),
                    Buttons = []
                }
            },new Tab()
                {
                    Action = "list",
                    Label = "Список",
                    InitialPage = new Page()
                    {
                        ClanList = BuildClanlist(p),
                        Buttons = buttons
                    }
            }];

            if (reqs.Count > 0)
            {
                tabs = tabs.Append(new Tab()
                {
                    Action = "reqs",
                    Label = "Заявки",
                    InitialPage = new Page()
                    { Buttons = [],
                    List = Reqs(p)
                    }
                    
                }).ToArray();
            }
            p.win = new Window()
            {
                Title = name,
                ShowTabs = true,
                Tabs = tabs
            };
            p.SendWindow();
        }
        public void AddReq(int id)
        {
            using var db = new DataBase();
            db.Attach(this);
            var p = MServer.GetPlayer(id);
            if (GetRequests().FirstOrDefault(i => i.player?.Id == id) == null)
            {
                var req = new Request() { clan = this, player = p, reqtime = DateTime.Now };
                reqs.Add(req);
                p?.ClanReqs.Add(req);
            }
            db.SaveChanges();
            p.win.CurrentTab.Open(new Page()
            {
                Text = "Заявка подана",
                Title = "КЛАНЫ",
                Card = new Card(CardImageType.Clan, (200 + id).ToString(), $"<color=white>{name}</color>\nУчастники: <color=white>{members.Count}</color>"),
                Buttons = []
            });
            p.SendWindow();
        }
        public ListEntry[] Reqs(Player p)
        {
            List<ListEntry> rq = new();
            var c = 1;
            foreach (var request in GetRequests())
            {
                rq.Add(new ListEntry($"{c}.<color=white>{request.player?.name}</color>", new Button("...", $"openreq:{request.player?.Id}",(args) => OpenReq(p,request))));
                c++;
            }
            return rq.ToArray();
        }
        public void OpenReq(Player p,Request target)
        {
            using var db = new DataBase();

            p.win = new Window()
            {
                Title = "Заявка в клан",
                Tabs = [new Tab() {
                    Action = "req", Label = "req",
                    InitialPage = new Page()
                    {
                        Text = $"@@Заявка на прием в клан:\n\n\nИмя: <color=white>{target.player?.name}</color>\nID <color=white>{target.player?.Id}</color>\nИстекает через:" +
                        $" {string.Format("{0:hh}ч.{0:mm} мин.", (TimeSpan.FromDays(1) - (DateTime.Now - target.reqtime)))}",
                        Buttons = [new Button("Принять", "accept", (args) => { AddMember(target.player, target); OpenClanWin(p); }), new Button("Откланить", "decline", (args) => { DeclineReq(target); OpenClanWin(p); }), new Button("Прокачка", "openskills", (args) => OpenPlayerSkills(p, target.player))]
                    }
                }]
            };
            p.SendWindow();
        }
        public void DeclineReq(Request target)
        {
            using var db = new DataBase(); 
            db.Attach(target.player);
            reqs.Remove(target);
            target.player.ClanReqs.Remove(target); 
            db.SaveChanges();

        }
        public void AddMember(Player p,Request q)
        {
            using var db = new DataBase();
            db.Attach(this);
            var player = MServer.GetPlayer(p.Id);
            player.clan = this;
            members.Add(player);
            player.clanrank = ranks.OrderBy(r => r.priority).First();
            player.SendClan();
            reqs.Remove(q);
            player.ClanReqs.Clear();
            db.SaveChanges();
        }
        private ClanListEntry[] BuildClanlist(Player p)
        {
            List<ClanListEntry> list = new();
            if (members != null)
            {
                foreach (var player in members)
                {
                    list.Add(new ClanListEntry(new Button($"<color={player?.clanrank.colorhex}>{player?.name}</color> - {player?.clanrank.name}", $"listrow:{player?.Id}", (args) => OpenPlayerPrew(p,player)), 0, "онлайн?"));
                }
            }
            return list.ToArray();
        }
        public void OpenPlayerPrew(Player p,Player target)
        {
            Button[] buttons = [new Button("Прокачка","skills",(args) => OpenPlayerSkills(p,target))];
            if (p.Id == ownerid && target.Id != p.Id)
            {
                buttons = buttons.Append(new Button("kick", "kick", (args) => { })).ToArray();
            }
            p.win.CurrentTab.Open(new Page()
            {
                Text = $"@@ПРОФИЛЬ СОКЛАНА\n\nИмя: <color={target.clanrank?.colorhex}>{target.name}</color>\nЗвание: {target.clanrank.name}\nID:  <color=white>{target.Id}</color>",
                Buttons = buttons
            });
        }
        public void OpenPlayerSkills(Player p,Player target)
        {
            target = MServer.GetPlayer(target.Id);
            p.win.CurrentTab.Open(new UpPage()
            {
                Skills = target.skillslist.GetSkills(),
                SlotAmount = target.skillslist.slots,
                SkillsToInstall = [],
                Text = $"Просмотр скиллов игрока\n\n<color=white>{target.name}</color>\nID <color=white>{target.Id}</color>\nОбщий уровень: <color=white>{target.skillslist.lvlsummary()}</color>"
            });
            p.SendWindow();
        }
        public void LeaveClan(Player p)
        {
            using var db = new DataBase();
            db.Attach(this);
            if (p.Id == ownerid)
            {
                db.Remove(this);
            }
            p.win = null;
            p.clan = null;
            p.SendClan();
            p.SendMyMove();
            p.SendWindow();
            db.SaveChanges();
        }
        #region creating
        public static void CreateClan(Player p,int icon,string name,string abr)
        {
            using var db = new DataBase();
            db.Attach(p);
            if (db.clans.FirstOrDefault(i => i.id == icon || i.name == name) == null)
            {
                var c = new Clan() { ownerid = p.Id, id = icon, abr = abr, name = name };
                       c.ranks = new List<Rank>()
            {
                new Rank() { name = "хуесос",priority = 0,colorhex = "#00FF00",owner = c },
                new Rank() { name = "уже смешарик",priority = 20,colorhex = "#ff0000",owner = c },
                new Rank() { name = "Создатель",priority = 100,colorhex = "#006400",owner = c }
                };
                db.Add(c);
                c.members.Add(p);
                p.clan = c;
                p.clanrank = c.ranks.First(i => i.priority == 100);
                p.SendClan();
            }
            db.SaveChanges();
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "КЛАН СОЗДАН",
                Buttons = []
            });
            p.SendWindow();
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
        #endregion
        #region clans
        public static void OpenClanList(Player p)
        {
            List<ClanListEntry> clans = new();
            using var db = new DataBase();
            foreach (var clan in db.clans.Include(i => i.members).Include(i => i.reqs))
            {
                clans.Add(new ClanListEntry(new Button($"<color=white>{clan.name}</color> [{clan.abr}]", $"clan{clan.id}", (args) => clan.OpenPreview(p)), (byte)clan.id, $"прием аткрыт"));
            }
            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "clanlist",
                    Label = "СПИСОК КЛАНОВ",
                    InitialPage = new Page()
                    {
                        Title = "КЛАНЫ",
                        ClanList = clans.ToArray(),
                        Buttons = []
                    }

                }
            ]
            };
            p.SendWindow();
        }
        public List<Request> GetRequests()
        {
            using var db = new DataBase();
            return db.reqs.Where(i => i.clan == this).Include(p => p.player).ToList();
        }
        public void OpenPreview(Player p)
        {
            var text = "";
            Button[] buttons = [new Button("Подать заявку", "reqin", (args) => AddReq(p.Id))];
            if (members.Contains(p) || ownerid == p.Id)
            {
                buttons = [];
            }
            using var db = new DataBase();
            if (GetRequests().FirstOrDefault(i => i.player.Id == id) != null)
            {
                text += "\n Заявка уже подана";
            }
            p.win.CurrentTab.Open(new Page()
            {
                Text = text,
                Title = "КЛАНЫ",
                Card = new Card(CardImageType.Clan, id.ToString(), $"<color=white>{name}</color>\nУчастники: <color=white>{members.Count}</color>"),
                Buttons = buttons
            });
            p.SendWindow();
        }
        #endregion
    }
}
