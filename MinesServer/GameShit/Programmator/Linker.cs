using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator
{
    public static class Linker
    {
        public static void OpenGui(Player p)
        {
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
                    Buttons = [new Button("Создать", $"create2{ActionMacros.Input}", (args) => { })]
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
