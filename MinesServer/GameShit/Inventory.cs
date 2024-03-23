using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Consumables;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit
{
    public class Inventory
    {
        public int Id { get; set; }
        public Inventory()
        {
            typeditems = new Dictionary<int, ItemUsage>
            {
                {
                    0,
                    (p) =>
                    {
                        return true;
                    }
                },
                {
                    1,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 6, -2, 3, (int)coord.X, (int)coord.Y, p))
                        {
                            new Resp((int)coord.X, (int)coord.Y, p.Id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    2,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -3, 4, (int)coord.X, (int)coord.Y, p))
                        {
                            new Up((int)coord.X, (int)coord.Y, p.Id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    3,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-3, 3, -3, 3, (int)coord.X, (int)coord.Y, p))
                        {
                            new Market((int)coord.X, (int)coord.Y, p.Id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    4,
                    (p) =>
                    {
                        return true;
                    }
                },
                {
                    5,
                    (p) =>
                    {
                        if (!World.GunRadius((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p))
                        {
                            ShitClass.Boom((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p);
                            return true;
                        }
                        return false;
                    }
                },
                {
                    6,
                    (p) =>
                    {
                        if (!World.GunRadius((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p))
                        {
                            ShitClass.Prot((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p);
                            return true;
                        }
                        return false;
                    }
                },
                {
                    7,
                    (p) =>
                    {
                        ShitClass.Raz((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p);
                        return true;
                    }
                },
                {
                    11,
                    (p) =>
                    {
                        var cell = World.GetCell((int)p.GetDirCord().X, (int)p.GetDirCord().Y);
                        var cellprop = World.GetProp(cell);
                        if (cellprop.isEmpty)
                        {
                            World.SetCell((int)p.GetDirCord().X ,(int)p.GetDirCord().Y, 50);
                            return true;
                        }
                        return false;
                    }
                },
                {
                    24,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 2, (int)coord.X, (int)coord.Y, p))
                        {
                            new Crafter((int)coord.X, (int)coord.Y, p.Id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    26,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 2, (int)coord.X, (int)coord.Y, p) && p.clan != null)
                        {
                            new Gun((int)coord.X, (int)coord.Y, p.Id, p.cid).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    29, (p) => {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 1, (int)coord.X, (int)coord.Y, p))
                        {
                            new Storage((int)coord.X, (int)coord.Y, p.Id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    40,
                    (p) =>
                    {
                        ShitClass.C190Shot((int)p.GetDirCord().X, (int)p.GetDirCord().Y, p);
                        return true;
                    }
                },
                {
                    41,
                    (p) =>
                    {
                        var cell = World.GetCell((int)p.GetDirCord().X, (int)p.GetDirCord().Y);
                        var cellprop = World.GetProp(cell);
                        if(cell == 36)
                        {
                            World.SetCell((int)p.GetDirCord().X ,(int)p.GetDirCord().Y, 104);
                            return true;
                        }
                        else if (cellprop.isEmpty)
                        {
                            World.SetCell((int)p.GetDirCord().X ,(int)p.GetDirCord().Y, 36);
                            return true;
                        }
                        return false;
                    }
                },
            };
        }
        public int this[int index]
        {
            get
            {
                if (items == null)
                {
                    var splited = itemstobd.Split(";");
                    items = new int[49];
                    if (splited.Length > 1)
                    {
                        for (var it = 0; it < splited.Length; it++)
                        {
                            items[it] = int.Parse(splited[it]);
                        }
                    }
                }
                return items[index];
            }
            set
            {
                using var db = new DataBase();
                db.Attach(this);
                items[index] = value;
                itemstobd = string.Join(';', items);
                DataBase.GetPlayer(Id)?.SendInventory();
                db.SaveChanges();
            }
        }
        private Dictionary<int, int> getinv()
        {
            var dick = new Dictionary<int, int>();
            var t = "";
            for (int i = 0; i < 49; i++)
            {
                if (this[i] > 0)
                {
                    dick[i] = this[i];
                }
            }
            return dick;
        }
        public InventoryPacket InvToSend()
        {
            return new InventoryPacket(new InventoryShowPacket(getinv(), selected, Lenght));
        }
        public DateTime time = DateTime.Now;
        public void Use(Player p)
        {
            if (DateTime.Now - time >= TimeSpan.FromMilliseconds(400))
            {
                if (typeditems.ContainsKey(selected) && !World.ContainsPack((int)p.GetDirCord().X, (int)p.GetDirCord().Y, out var pack) && (World.GetProp((int)p.GetDirCord().X, (int)p.GetDirCord().Y).can_place_over || selected == 40) && this[selected] > 0)
                {
                    if (typeditems[selected](p))
                    {
                        this[selected]--;
                        p.SendInventory();
                    }
                }
                time = DateTime.Now;
            }
        }
        public Dictionary<int, ItemUsage> typeditems;
        public delegate bool ItemUsage(Player p);
        public void Choose(int id, Player p)
        {
            ITopLevelPacket packet = InventoryPacket.Choose("Мы не хотим этого!", new bool[0, 0], 123, 123, 12);
            selected = id;
            if (id == -1)
            {
                packet = InventoryPacket.Close();
            }
            p.connection?.SendU(InvToSend());
            p.connection?.SendU(packet);
        }
        public int selected = -1;
        [NotMapped]
        public int Lenght
        {
            get
            {
                var l = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] > 0)
                    {
                        l++;
                    }
                }
                return l;
            }
        }
        public string itemstobd { get; set; } = "";
        [NotMapped]
        public int[] items { get; set; } = null;
    }
}
