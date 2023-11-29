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
            /*HorbDecoder.InitCommands();*/
            time = new ServerTime();
            new World(Default.cfg.WorldName, 32 * 10, 32 * 10);
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
            return db.players.FirstOrDefault(i => i.Id == id);
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
