using Microsoft.Identity.Client;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.Network.Programmator;
using MinesServer.Server;
using System.Drawing;

namespace MinesServer.GameShit.Programmator
{
    public static class StaticGUI
    {
        public static void NewProg(Player p,string name)
        {
            using var db = new DataBase();
            db.players.Attach(p);
            var prog = new Program(p, "", name);
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
            return progs.Select(i => new ListEntry(i.title, new Button("open", "openprog", (a) => OpenProg(p,i)))).ToArray();
        }
        public static void OpenProg(Player p, Program prog)
        {
            p.connection?.SendU(new OpenProgrammatorPacket(prog.id, prog.title, ""));
            p.win = null;
        }
        public static void StartedProg(Player p,(int id,string source) data)
        {
            using var db = new DataBase();
            var programm = db.progs.FirstOrDefault(i => i.id == data.id);
            if (programm != default)
            {
                programm.progtext = data.source;
                db.SaveChanges();
                p.RunProgramm(programm);
            }
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
                        Buttons = []
                    }

                }]
                };
                p.SendWindow();
                return;
            }
            var naming = (Player p) =>
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
            };
            var progs = p.programs;
            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        Buttons = [new Button("СОЗДАТЬ ПРОГРАММУ", "createprog", (args) => naming(p))]
                    }

                }]
            };
            p.SendWindow();
        }
    }
}
