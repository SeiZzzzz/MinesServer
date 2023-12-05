using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;
namespace MinesServer.Server
{
    public class MServer : TcpServer
    {
        public System.Timers.Timer timer;
        public ServerTime time { get; private set; }
        public Dictionary<int, Player> players;
        public static MServer? Instance;
        public static bool started = false;
        public MServer(IPAddress address, int port) : base(address, port)
        {
            Instance = this;
            MConsole.InitCommands();
            players = new Dictionary<int, Player>();
            time = new ServerTime();
            new World(Default.cfg.WorldName, 32 * 32, 32 * 32);
            time.Start();
            OptionKeepAlive = true;
        }

        public static Player? GetPlayer(int id)
        {
            if (Instance!.players.Keys.Contains(id))
            {
                return Instance.players[id];
            }
            var db = new DataBase();
            return db.players
                .Where(i => i.Id == id)
                .Include(p => p.clanrank)
                .Include(p => p.clan)
                .Include(p => p.inventory)
                .Include(p => p.crys)
                .Include(p => p.skillslist)
                .Include(p => p.settings)
                .Include(p => p.health)
                .Include(p => p.resp)
                .FirstOrDefault();
        }
        public static Player? GetPlayer(string name)
        {
            var db = new DataBase();
            return db.players
                .Where(i => i.name == name)
                .Include(p => p.clanrank)
                .Include(p => p.clan)
                .Include(p => p.inventory)
                .Include(p => p.crys)
                .Include(p => p.skillslist)
                .Include(p => p.settings)
                .Include(p => p.health)
                .Include(p => p.resp)
                .FirstOrDefault();
        }
        protected override TcpSession CreateSession()
        {
            var s = new Session(this);
            return s;
        }
        protected override void OnError(SocketError error)
        {
            Default.WriteError(error.ToString());
        }
    }
}
