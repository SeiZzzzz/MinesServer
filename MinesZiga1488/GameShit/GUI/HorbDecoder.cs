using MinesServer.GameShit.Buildings;
using MinesServer.Server;

namespace MinesServer.GameShit.GUI
{
    public static class HorbDecoder
    {
        public static void InitCommands()
        {
            commands.Add("myid", (p, arg) =>
            {
                p.AddConsoleLine(p.Id.ToString());
            });
            commands.Add("setnick", (p, arg) =>
            {
                p.name = arg.Split(' ')[1];
                using var db = new DataBase();
                db.SaveChanges();
            });
        }
        public static void Decode(string msg, Player p)
        {
            if (msg == "exit")
            {
                p.connection.Send("Gu", "");
                p.insidesmf = false;
            }
            else if (p.insidesmf is not bool)
            {
                switch (p.insidesmf)
                {
                    case Packs.Teleport:
                        Console.WriteLine("how tf");
                        return;
                }
            }
            else
            {
                ConsoleCommand(msg, p);
                p.ShowConsole();
            }
        }
        public delegate void Command(Player p, string args);
        public static Dictionary<string, Command> commands = new Dictionary<string, Command>();
        public static void ConsoleCommand(string msg, Player p)
        {
            p.AddConsoleLine(msg);
            if (msg.Contains(' '))
            {
                if (commands.Keys.Contains(msg.Split(' ')[0]))
                {
                    commands[msg.Split(' ')[0]](p, msg);
                    return;
                }
            }
            if (commands.Keys.Contains(msg))
            {
                commands[msg](p, msg);
                return;
            }
            p.AddConsoleLine("бля это че нахуй");
        }
    }
}
