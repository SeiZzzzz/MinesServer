using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.Network;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms.Design;

namespace MinesServer.GameShit
{
    public class Settings
    {
        public Settings()
        {

        }
        public Settings(bool n)
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
            serialized = Newtonsoft.Json.JsonConvert.SerializeObject(d);
        }
        public string this[string key]
        {
            get
            {
                sett ??= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
                return sett[key];
            }
            set
            {
                using var db = new DataBase();
                sett ??= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
                sett[key] = value;
                db.SaveChanges();
            }
        }
        [Key]
        public int id { get; set; }
        public string serialized { get; set; }
        [NotMapped]
        private Dictionary<string, string> sett = null;
        public void SendSettings(Player p)
        {
            sett ??= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
            p.connection?.SendU(new SettingsPacket(sett));
        }
        public void Save(Player p, Dictionary<string, string> list)
        {
            foreach(var i in list)
            {
                this[i.Key] = i.Value;
            }
            SendSettings(p);
            SendSettingsGUI(p);
        }
        public void SendSettingsGUI(Player p)
        {
            Button[] btns = [new Button("Сохранить", $"save:{ActionMacros.RichList}", (args) => { Save(p, args.RichList); })];
            if (p.cid == 0)
            {
                btns = btns.Append(new Button("Создать клан", $"clancreate", (args) => { Clan.OpenCreateWindow(p); })).ToArray();
            }
            p.win = new Window()
            {
                ShowTabs = true,
                Title = "НА СТРОЙКЕ",
                Tabs = [new Tab()
                {
                    Label = "Настройки",
                    Action = "settings",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = [RichListEntry.DropDown("Масштаб интерфейса", "isca", ["мелко", "КРУПНО"], int.Parse(this["isca"])),
                            RichListEntry.DropDown("Масштаб территории", "tsca", ["мелко", "КРУПНО"], int.Parse(this["tsca"])),
                            RichListEntry.Bool("Включить управление мышкой", "mous", this["mous"].ToBool()),
                            RichListEntry.Bool("Упрощенный режим графики", "pot", this["pot"].ToBool()),
                            RichListEntry.Bool("ринудительно обновлять породы (увеличит потр. CPU)", "frc", this["frc"].ToBool()),
                            RichListEntry.Bool("CTRL переключает скорость робота (вместо удерживания)", "ctrl", this["ctrl"].ToBool()),
                            RichListEntry.Bool("Отключить ближайшие звуки", "mof", this["mof"].ToBool())
                            ]
                        },
                        Buttons = btns
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
            p.SendWindow();
        }
    }
}
