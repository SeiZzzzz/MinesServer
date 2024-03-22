using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.Network.Programmator;
using MinesServer.Server;

namespace MinesServer.GameShit.Programmator
{
    public static class StaticGUI
    {

        public static void NewProg(Player p, string name)
        {
            using var db = new DataBase();
            db.players.Attach(p);
            var prog = new Program(p, name, "");
            p.programs.Add(prog);
            db.SaveChanges();
            p.connection?.SendU(new OpenProgrammatorPacket(prog.id, name, ""));
            p.connection?.SendU(new ProgrammatorPacket());
            p.win = null;
        }
        public static ListEntry[] LoadProgs(Player p)
        {
            var db = new DataBase();
            var progs = db.progs.Where(i => i.owner == p).ToList();
            if (progs.Count == 0)
                return [];
            return progs.Select(i => new ListEntry(i.name, new Button("open", "openprog", (a) => OpenProg(p, i)))).ToArray();
        }
        public static void OpenProg(Player p, Program prog)
        {
            p.connection?.SendU(new OpenProgrammatorPacket(prog.id, prog.name, ""));
            p.win = null;
        }
        public static void Rename(Player p,int id)
        {
            var rename =  (ActionArgs args) =>
            {
                if (Default.def.IsMatch(args.Input))
                {
                    using var db = new DataBase();
                    var prog = db.progs.FirstOrDefault(p => p.id == id);
                    prog.name = args.Input;
                    db.SaveChanges();
                    p.connection.SendU(new UpdateProgrammatorPacket(prog.id, prog.name, prog.data));
                }
                p.win = null;
            };
            p.win = new Window()
            {
                Title = "RenameProg",
                Tabs = [new Tab()
                {
                    Action = "pren1",
                    InitialPage = new Page(){
                        Text = "Rename\n",
                Input = new InputConfig()
                {
                    Placeholder = "Название программы..."
                },
                Style = new Style()
                {
                    FixScrollTag = "prg"
                },
                Buttons = [new Button("Ok", $"rename{ActionMacros.Input}", rename)]
                    },
                    Label = "pren2"
                }]
            };
            p.SendWindow();
        }
        public static void StartedProg(Player p, (int id, string source) data)
        {
            using var db = new DataBase();
            var programm = db.progs.FirstOrDefault(i => i.id == data.id);
            if (programm != default)
            {
                programm.data = data.source;
                db.SaveChanges();
                p.RunProgramm(programm);
                p.connection?.SendU(new OpenProgrammatorPacket(-1,programm.name,programm.data));
            }
        }
        public static void OpenCreateProg(Player p)
        {
            p.win.CurrentTab.Open(new Page()
            {
                Text = "Введите название вашей программы\n",
                Input = new InputConfig()
                {
                    Placeholder = "Название программы..."
                },
                Style = new Style()
                {
                    FixScrollTag = "prg"
                },
                Buttons = [new Button("Создать", $"create2{ActionMacros.Input}", (args) => NewProg(p, args.Input))]
            });
            p.SendWindow();
        }
        public static void DeleteProg(Player p,int id)
        {
            using var db = new DataBase();
            db.progs.Remove(db.progs.FirstOrDefault(i => i.id == id)!);
            db.SaveChanges();
        }
        public static void OpenGui(Player p)
        {
            var l = LoadProgs(p);
            if (l.Length > 0)
            {
                p.win = new Window()
                {
                    Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        List = l,
                        Buttons = [new Button("Создать","createrog",(e) => OpenCreateProg(p))]
                    }

                }]
                };
                p.SendWindow();
                return;
            }
            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        Buttons = [new Button("СОЗДАТЬ ПРОГРАММУ", "createprog", (args) => OpenCreateProg(p))]
                    }

                }]
            };
            p.SendWindow();
        }
    }
}
