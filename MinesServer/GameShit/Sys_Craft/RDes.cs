using Newtonsoft.Json;
using System.Net.WebSockets;

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
            var rid = 0;
            var l = new List<Recipie>();
            if (!Directory.Exists("recipies"))
            {
                Directory.CreateDirectory("recipies");
                File.WriteAllText("recipies/first.json", JsonConvert.SerializeObject(new Recipie() { time = 2, result = new RC(0, 5), costcrys = [new RC(0, 25)], costres = [new RC(0, 1)] }, Formatting.Indented));
            }
            foreach (var i in Directory.GetFiles("recipies/"))
            {
                var r = JsonConvert.DeserializeObject<Recipie>(File.ReadAllText(i));
                var n = r;
                n.id = rid;
                r = n;
                l.Add(r);
                rid++;
            }
            return l;
        }
        public static Recipie ByResultId(int res_id)
        {
            return RDes.recipies.FirstOrDefault(i => i.result.id == res_id);
        }
    }
}
