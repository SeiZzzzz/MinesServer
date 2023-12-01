using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.Network;
using System.ComponentModel.DataAnnotations;

namespace MinesServer.GameShit
{
    public class Settings
    {
        [Key]
        public int id { get; set; }
        public void SendSettings(Player p)
        {
            var d = new Dictionary<string, string>();
            d["cc"] = "10";
            d["snd"] = "0";
            d["mus"] = "0";
            d["isca"] = "0";
            d["tsca"] = "0";
            d["mous"] = "1";
            d["pot"] = "0";
            d["frc"] = "1";
            d["ctrl"] = "1";
            d["mof"] = "1";
            p.connection.SendU(new SettingsPacket(d));
        }
        public Window SendSettingsGUI(Player p)
        {
            return new Window()
            {
                Title = "НА СТРОЙКЕ",
                Tabs = [new Tab()
                {
                    Label = "хуй",
                    Action = "settings",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = [new RichListEntry(RichListEntryType.DropDown, "хуй", "хуй", "хуй", "хуй")]
                        },
                        Text = "\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс\",\r\n                    \"ВНИМАНИЕ! При создании бокса теряется нихуя кристаллов\n",
                        Buttons = [new Button("<color=green>В БОКС</color>", $"hui", (args) => { })]
                    }
                },
                    new Tab()
                    {
                        Label = "хуй1",
                        Action = "hots"

                    },
                    new Tab()
                    {
                        Label = "хуй3",
                        Action = "config"

                    }
                ]
            };
        }
    }
}
