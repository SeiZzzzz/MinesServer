﻿using MinesServer.Network.GUI;
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
                    0,(x, y) =>
                    {

                    }
                },
                {
                    1,(x, y) =>
                    {

                    }
                },
                {
                    2,(x, y) =>
                    {

                    }
                },
                {
                    3,(x, y) =>
                    {

                    }
                },
                {
                    4,(x, y) =>
                    {

                    }
                },
                {
                    5,World.Boom
                }
            };
        }
        public void SetItem(int id, int col)
        {
            var x = items;
            x[id] = col;
            items = x;
            using var db = new DataBase();
            db.SaveChanges();
        }
        private Dictionary<int, int> getinv()
        {
            var dick = new Dictionary<int, int>();
            var t = "";
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] > 0)
                {
                    dick[i] = items[i];
                }
            }
            return dick;
        }
        public InventoryPacket InvToSend()
        {
            return new InventoryPacket(new InventoryShowPacket(getinv(), selected, Lenght));
        }
        public void Use(int x, int y)
        {
            if (typeditems.ContainsKey(selected) && World.GetProp(World.GetCell(x,y)).can_place_over)
            {
                typeditems[selected](x,y);
            }
        }
        public Dictionary<int, ItemUsage> typeditems;
        public delegate void ItemUsage(int x, int y);
        public void Choose(int id,Player p)
        {
            selected = id;
            p.connection.SendU(InventoryPacket.Choose("ты хуесос", new bool[0,0], 123, 123, 12));
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
        public string itemstobd { get; set; }
        [NotMapped]
        private int[] iret = null;
        [NotMapped]
        public int[] items
        {
            get
            {
                if (iret == null)
                {
                    var splited = itemstobd.Split(";");
                    var i = new int[49];
                    if (splited.Length > 0)
                    {
                        for (var it = 0; it < splited.Length; it++)
                        {
                            i[it] = int.Parse(splited[it]);
                        }
                    }
                    iret = i;
                }
                return iret;
            }
            set
            {
                itemstobd = string.Join(';', value);
            }
        }
    }
}
