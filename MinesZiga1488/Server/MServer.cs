using MinesServer.GameShit;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace MinesServer.Server
{
    public class MServer : TcpServer
    {
        public ServerTime time { get; private set; }
        public World wrld;
        public Dictionary<int,Session> players;
        public static MServer? Instance;
        public MServer(IPAddress address, int port) : base(address, port)
        {
            Instance = this;
            players = new Dictionary<int, Session>();
            time = new ServerTime();
            wrld = new World(Default.cfg.WorldName, 32 * 10, 32 * 10);
            DataBase.Load();
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
