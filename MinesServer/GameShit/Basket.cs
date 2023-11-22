using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Network.GUI;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit
{
    public class Basket
    {
        public int Id { get; set; }
        [NotMapped]
        public Player player;
        public Basket(Player player) => this.player = player;
        public Basket()
        {

        }
        public long[] cry =
        {
            0,
            0,
            0,
            0,
            0,
            0
        };
        public void AddCrys(int index, long val)
        {
            this.cry[index] += val;
            if (cry[index] < 0)
            {
                cry[index] = long.MaxValue;
            }
            SendBasket();
        }
        public void Boxcrys(long[] crys)
        {
            for (var i = 0; i < this.cry.Length; i++)
            {
                cry[i] += crys[i];
            }

            SendBasket();
        }
        public void ClearCrys()
        {
            for (var i = 0; i < this.cry.Length; i++)
            {
                this.cry[i] = 0;
            }

            SendBasket();
        }
        public bool RemoveCrys(int index, long val)
        {
            if (val < 0)
            {
                return false;
            }

            if ((this.cry[index] - val) >= 0)
            {
                this.cry[index] -= val;
                SendBasket();
                return true;
            }

            SendBasket();
            return false;
        }
        private int Buildcap()
        {
            return 1;
        }
        public Window OpenBoxGui()
        {
            return new Window()
            {
                ShowTabs = false,
                Title = "Создание бокса",
                Tabs = [new Tab() {
                    Label = "хуй",
                    Action = "dropbox",
                    InitialPage = new Page()
                    {
                        CrystalConfig = new CrystalConfig("  останется", "будет в боксе", [new CrysLine("", 0, 0, cry[0],0), new CrysLine("", 0, 0, cry[1], 0),
                            new CrysLine("", 0, 0, cry[2], 0), new CrysLine("", 0, 0, cry[3], 0),
                            new CrysLine("", 0, 0, cry[4], 0),new CrysLine("", 0, 0, cry[5], 0)]),
                        Text = "\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс\",\r\n                    \"ВНИМАНИЕ! При создании бокса теряется нихуя кристаллов\n",
                        Buttons = [new Button("<color=green>В БОКС</color>", $"dropbox:{ActionMacros.CrystalSliders}",(args) => { player.BBox(args.CrystalSliders); })]
                    }
                }]
            };
        }
        public void SendBasket()
        {
            using var db = new DataBase();
            db.SaveChanges();
            var p = new BasketPacket(cry[0], cry[1], cry[2], cry[3], cry[4], cry[5], Buildcap());
            player.connection.SendU(p);
        }
        public int cap = 0;
        public long AllCry => this.cry.Select((t, i) => cry[i]).Sum();
        public string GetCry => this.cry.Aggregate("", (current, t) => current + (t + ":")) + cap;
    }
}
