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
        public static MServer? Instance;
        public static bool started = false;
        public MServer(IPAddress address, int port) : base(address, port)
        {
            Instance = this;
            MConsole.InitCommands();
            GameShit.SysCraft.RDes.Init();
            time = new ServerTime();
            new World(Default.cfg.WorldName);
            time.Start();
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
