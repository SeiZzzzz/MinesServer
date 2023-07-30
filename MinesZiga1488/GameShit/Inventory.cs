using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.Server;
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
                    5,(x, y) => 
                    {

                    }
                }
            };
        }
        public void SetItem(int id,int col)
        {
            var x = items;
            x[id] = col;
            items = x;
            using var db = new DataBase();
            db.SaveChanges();
        }
        private string getinv()
        {
            var t = "";
            for (int i = 0;i < items.Length;i++)
            {
                if (items[i] > 0)
                {
                    t += $"{i}#{items[i]}#";
                }
            }
            if (t == "")
            {
                return "";
            }
            return t.Substring(0, t.Length - 1);
        }
        public string InvToSend()
        {
            return $"show:{Lenght}:{selected}:{getinv()}";
        }
        public void Use(int x, int y)
        {

        }
        public Dictionary<int,ItemUsage> typeditems;
        public delegate void ItemUsage(int x, int y);
        public void Choose(int id)
        {
            selected = id;
        }
        public int selected = -1;
        [NotMapped]
        public int Lenght
        {
            get
            {
                var l = 0;
                for(int i = 0;i < items.Length;i++)
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
        public int[] items
        {
            get
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
                return i;
            }
            set
            {
                itemstobd = string.Join(';', value);
            }
        }
    }
}
