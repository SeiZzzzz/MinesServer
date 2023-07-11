using MinesServer.GameShit;
using MinesServer.GameShit.GUI;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace MinesServer.Server
{
    public class Auth
    {
        public bool complited = false;
        public string nick = "";
        public string passwd = "";
        public bool createnew = false;
        public void TryToAuth(Packet p, string sid, Session initiator)
        {
            var data = Encoding.Default.GetString(p.data).Split('_');
            int res;
            Player player = null;
            if (int.TryParse(data[1], out res))
            {
                player = DataBase.GetPlayerClassFromBD(res);
            }
            if (player == null)
            {
                initiator.Send("PI", "0:0:0");
                initiator.Send("cf",
                "{\"width\":" + 1 + ",\"height\":" + 1 +
                        ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    initiator.Send("CF",
                    "{\"width\":" + 1 + ",\"height\":" + 1 +
                        ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    new Builder()
                        .SetTitle("ВХОД")
                        .AddTextLine("Ник")
                        .AddIConsole()
                        .AddIConsolePlace("")
                        .AddButton("ОК", "%I%")
                        .AddButton("НОВЫЙ АКК", "newakk")
                        .Send(initiator);
                return;
            }
            if (CalculateMD5Hash(player.hash + sid) == data[2])
            {
                player.connection = initiator;
                initiator.player = player;
                player.Init();
                return;
            }
            initiator.Send("AH", "BAD");
        }
        public static bool NickNotAvl(string nick)
        {
            using var db = new DataBase();
            try
            {
                Console.WriteLine(db.players.Count(p => p.name == nick));

                return db.players.Count(p => p.name == nick) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void CreateNew(Session initiator)
        {
            temp = new Player();
            createnew = true;
            new Builder()
                   .SetTitle("НОВЫЙ ИГРОК")
                    .AddTextLine("Ник")
                     .AddIConsole()
                       .AddIConsolePlace("")
                        .AddButton("ОК", "%I%")
                        .Send(initiator);
        }
        public void SetPasswdForNew(Session initiator)
        {
            new Builder()
                  .SetTitle("НОВЫЙ ИГРОК")
                   .AddTextLine("Пароль")
                    .AddIConsole()
                      .AddIConsolePlace("")
                       .AddButton("ОК", "%I%")
                       .Send(initiator);
        }
        public void EndCreateAndInit(Session initiator)
        {
            complited = true;
            temp.CreatePlayer();
            temp.passwd = passwd;
            temp.name = nick;
            using var db = new DataBase();
            db.players.Add(temp);
            db.SaveChanges(); 
            temp.connection = initiator;
            initiator.player = temp;
            initiator.Send("AH", temp.Id + "_" + temp.hash);
            temp.Init();
        }
        public void TryToFindByNick(string name,Session initiator)
        {
            using var db = new DataBase();
            Player player = db.players.FirstOrDefault(p => p.name == name);
            if (player != default(Player))
            {
                temp = player;
                nick = name;
                new Builder()
              .SetTitle("ВХОД")
              .AddTextLine("Пароль")
              .AddIConsole()
              .AddIConsolePlace("")
              .AddButton("ОК", "%I%")
              .Send(initiator);
                return;
            }
            initiator.Send("OK", "Игрок не найден");
            initiator.Send("PI", "0:0:0");
            initiator.Send("cf",
            "{\"width\":" + 1 + ",\"height\":" + 1 +
                    ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            initiator.Send("CF",
            "{\"width\":" + 1 + ",\"height\":" + 1 +
                ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            new Builder()
                .SetTitle("ВХОД")
                .AddTextLine("Ник")
                .AddTextLine("игрок не найден")
                .AddIConsole()
                .AddIConsolePlace("")
                .AddButton("ОК", "%I%")
                .AddButton("НОВЫЙ АКК", "newakk")
                .Send(initiator);
        }
        public void TryToAuthByPlayer(string passwd,Session initiator)
        {
            if (temp.passwd == passwd)
            {
                complited = true;
                temp.connection = initiator;
                initiator.player = temp;
                initiator.Send("AH", temp.Id + "_" + temp.hash);
                initiator.player.Init();
                return;
            }
            new Builder()
               .SetTitle("ВХОД")
               .AddTextLine("Пароль")
               .AddTextLine("Не верный пароль")
               .AddIConsole()
               .AddIConsolePlace("")
               .AddButton("ОК", "%I%")
               .Send(initiator);

        }
        public Player temp = null;
        public static string GenerateSessionId()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnoprtsuxyz0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CalculateMD5Hash(string input)
        {
            HashAlgorithm hashAlgorithm = MD5.Create();
            var bytes = Encoding.ASCII.GetBytes(input);
            var array = hashAlgorithm.ComputeHash(bytes);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}


