using MinesServer.GameShit;
using MinesServer.GameShit.GUI;
using NetCoreServer;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Timers;

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
            HorbDecoder.InitCommands();
            time = new ServerTime();
            new World(Default.cfg.WorldName, 32 * 1000, 32 * 1000);
            DataBase.Load();
            timer = new System.Timers.Timer(1);
            timer.Elapsed += (object s, ElapsedEventArgs e) => { time.Update(); };
            timer.Start();
            OptionKeepAlive = true;
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
