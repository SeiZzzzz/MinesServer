using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Buildings
{
    public class Storage : Pack, IDamagable
    {
        public long this[int index]
        {
            get => crysinside[index];
            set => crysinside[index] = value;
        }
        [NotMapped]
        public float charge { get; set; }
        public DateTime brokentimer { get; set; }
        public int hp { get; set; }
        #region crysshit
        public long[] crysinside = new long[6];
        public long ze
        {
            get { return crysinside[0]; }
            set { crysinside[0] = value; }
        }

        public long cr
        {
            get { return crysinside[1]; }
            set { crysinside[1] = value; }
        }

        public long si
        {
            get { return crysinside[2]; }
            set { crysinside[2] = value; }
        }

        public long be
        {
            get { return crysinside[3]; }
            set { crysinside[3] = value; }
        }

        public long fi
        {
            get { return crysinside[4]; }
            set { crysinside[4] = value; }
        }

        public long go
        {
            get { return crysinside[5]; }
            set { crysinside[5] = value; }
        }
        #endregion
        private Storage()
        {
        }
        public Storage(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Storage)
        {
            hp = 1000;
            using var db = new DataBase();
            db.storages.Add(this);
            db.SaveChanges();
        }
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y + 1, 35, false);
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y + 1, 35, true);
            base.Build();
        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            if (crysinside.Sum() > 0)
            {
                Box.BuildBox(x, y, crysinside, null);
                crysinside = new long[6];
            }
            using var db = new DataBase();
            db.storages.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[29]++;
            }
        }
        private void StockTransfer(long[]? sliders, Player p)
        {
            if (sliders == null)
                return;
            for (int i = 0; i < 6; i++)
            {
                var count = p.crys.cry[i] + crysinside[i];
                if (count - sliders[i] >= 0 && sliders[i] >= 0)
                {
                    p.crys.cry[i] = count - sliders[i];
                    crysinside[i] = sliders[i];
                }
            }
            p.crys.SendBasket();
            p.win = GUIWin(p);
        }
        #endregion
        public override Window? GUIWin(Player p)
        {
            var ok = new Button("transfer", $"transfer:{ActionMacros.CrystalSliders}", (args) => StockTransfer(args.CrystalSliders, p));
            var cryslines = crysinside.Select((cry, id) => new CrysLine("", 0, 0, p.crys.cry[id] + cry, (int)(cry))).ToArray();
            var page = new Page()
            {
                Title = "Склад",
                CrystalConfig = new CrystalConfig(" ", " ", cryslines),
                Buttons = [ok]
            };
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "хй",
                    Label = "хуху",
                    InitialPage = page
                }]
            };
        }
    }
}
