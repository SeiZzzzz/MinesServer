using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.Server;
using System.Windows.Interop;

namespace MinesServer.GameShit
{
    public static class MConsole
    {
        public static void InitCommands()
        {
            commands.Add("setitem", (p, arg) =>
            {
                if (arg.Split(" ").Length > 1 && int.TryParse(arg.Split(" ")[1], out var i) && int.TryParse(arg.Split(" ")[2], out var c))
                {
                    p.inventory.SetItem(i, c);
                    AddConsoleLine(p, "ok");
                    p.SendInventory();
                }
            });
            commands.Add("myid", (p, arg) =>
            {
                AddConsoleLine(p, p.Id.ToString());
            });
            commands.Add("getallmap", (p, arg) =>
            {
                List<IDataPartBase> l = new();
                for (int x = 0; x < World.W.chunksCountW; x++)
                {
                    for (int y = 0; y < World.W.chunksCountH; y++)
                    {
                        World.W.chunks[x, y].Load();
                        l.Add(new HBMapPacket(x * 32, y * 32, 32, 32, World.W.chunks[x, y].cells));
                    }
                }
                p.connection.SendB(new HBPacket(l.ToArray()));
            });
            commands.Add("setnick", (p, arg) =>
            {
                if (arg.Split(' ').Length > 0)
                {
                    p.name = arg.Split(' ')[1];
                    using var db = new DataBase();
                    db.SaveChanges();
                    db.Dispose();
                }
            });
        }
        public static void AddConsoleLine(Player p)
        {
            p.console.Enqueue(new Line { text = ">    " });
            if (p.console.Count > 16)
            {
                p.console.Dequeue();
                var l = p.console.Peek();
                l.text = "@@" + l.text;
            }
        }
        public static void AddConsoleLine(Player p,string text)
        {
            p.console.Enqueue(new Line { text = ">" + text });
            if (p.console.Count > 16)
            {
                p.console.Dequeue();
                var l = p.console.First();
                l.text = "@@" + l.text;
            }
        }
        public static void ShowConsole(Player p)
        {
            p.win = new Window()
            {
                Title = "Консоль",
                Tabs = [new Tab()
                {
                    Label = "",
                    Action = "console",
                    InitialPage = new Page()
                    {
                        Input = new InputConfig()
                        {
                            Placeholder = "cmd",
                            IsConsole = true
                        },
                        Text = string.Join("", p.console.Select(x => x.text + '\n').ToArray()),
                        Buttons = [new Button("ВЫПОЛНИТЬ", $"{ActionMacros.Input}", (args) => {
                            var msg = args.Input![1..];
                            AddConsoleLine(p, msg);
                            if (msg.Contains(' '))
                            {
                                if (commands.Keys.Contains(msg.Split(' ')[0]))
                                {
                                    commands[msg.Split(' ')[0]](p, msg);
                                    return;
                                }
                            }
                            if (commands.Keys.Contains(msg))
                            {
                                commands[msg](p, msg);
                                return;
                            }
                            AddConsoleLine(p,"бля это че нахуй");
                            ShowConsole(p);
                        })]
                    }
                }],
                ShowTabs = false
            };
            p.SendWindow();
        }
        public delegate void Command(Player p, string args);
        public static Dictionary<string, Command> commands = new Dictionary<string, Command>();
    }
    public class Line
    {
        public string text { get; set; }
    }
}
