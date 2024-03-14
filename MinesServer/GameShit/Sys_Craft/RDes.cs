using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.SysCraft
{
    public static class RDes
    {
        public static List<Recipie> recipies;
        public static void Init()
        {
            recipies = GetResipies();
        }
        public static List<Recipie> GetResipies()
        {
            var l = new List<Recipie>();
            if (!Directory.Exists("recipies"))
            {
                Directory.CreateDirectory("recipies");
                File.WriteAllText("recipies/first.json", JsonConvert.SerializeObject(new Recipie() { time = 2, result = new RC(0, 5), costcrys = [new RC(0, 25)], costres = [new RC(0, 1)] }, Formatting.Indented));
            }
            foreach (var i in Directory.GetFiles("recipies/"))
            {
                JsonConvert.DeserializeObject<Recipie>(File.ReadAllText(i));
                l.Add(JsonConvert.DeserializeObject<Recipie>(File.ReadAllText(i)));
            }
            return l;
        }
    }
}
