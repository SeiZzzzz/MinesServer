using MinesServer.GameShit;
using MinesServer.GameShit.GUI;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;
namespace MinesServer.Server
{
    public class MServer : TcpServer
    {
        public System.Timers.Timer timer;
        public ServerTime time { get; private set; }
        public Dictionary<int, Session> players;
        public static MServer? Instance;
        public MServer(IPAddress address, int port) : base(address, port)
        {
            Instance = this;
            players = new Dictionary<int, Session>();
            /*HorbDecoder.InitCommands();*/
            time = new ServerTime();
            new World(Default.cfg.WorldName, 32 * 10, 32 * 10);
            DataBase.Load();
            time.Start();
            OptionKeepAlive = true;
        }

        public static Session GetPlayer(int id)
        {
            if (Instance.players.Keys.Contains(id))
            {
                return Instance.players[id];
            }
            return null;
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
