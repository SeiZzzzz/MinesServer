using MinesServer.GameShit;
using NetCoreServer;

namespace MinesServer.Server
{
    public class Session : TcpSession
    {
        MServer father;
        public Player player;
        public Session(TcpServer server) : base(server) { father = server as MServer; tyevents = new Dictionary<string, TYTypedEvent>(); ; te = new Dictionary<string, TypedEvent>();InitEvents(); }
        public void InitEvents()
        {
            te.Add("AU", () =>
            {
                Console.WriteLine("Au");
                Send("AH", this.player.Id + "_" + this.player.hash);
            });
            te.Add("PO", () =>
            {
                Console.WriteLine("Ping");
            });
            InitTYEvents();
            te.Add("TY", () =>
            {
                Console.WriteLine("Other Methods");
            });
        }
        public void InitTYEvents()
        {
            tyevents.Add("Xmov", () =>
            {
                Console.WriteLine("Move");
            });
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            this.DecodeRecive(new Packet(buffer));
        }
        protected override void OnDisconnected()
        {
            if (player == null)
            {
                return;
            }
            father.players.Remove(player.Id);

        }
        protected override void OnConnected()
        {
            //load from bd
            Console.WriteLine($"{this.ToString()} connected");
            player = new Player();
            player.connection = this;
            player.CreatePlayer();
            father.players.Add(player.Id,this);
            Send("AU", player.GenerateSessionId());
            player.Init();
        }
        public void Send(string eventType, byte[] data)
        {
            Send(new Packet("B", eventType, data));
        }
        public void Send(string eventType, string data)
        {
            Send(new Packet("U", eventType, data));
        }
        public void Send(Packet p)
        {
            //Console.WriteLine("[S->C] " + p.dataType + " " + p.eventType + " [" + string.Join(",", p.data) + "] " + Encoding.UTF8.GetString(p.data));
            SendAsync(p.Compile);
        }
        public void SendCell(int x, int y, byte cell)
        {
            if ((x >= 0 && y >= 0) && (x <= MServer.Instance.wrld.width && y <= MServer.Instance.wrld.height))
            {
                var dat = new byte[1];
                dat[0] = cell;
                SendCells(1, 1, x, y, dat);
            }
        }
        public void SendCells(int w, int h, int x, int y, byte[] cells)
        {
            var data = new byte[7 + cells.Length];
            data[0] = (byte)'M';
            data[1] = (byte)w;
            data[2] = (byte)h;
            var _x = BitConverter.GetBytes(x);
            System.Buffer.BlockCopy(_x, 0, data, 3, 2);
            var _y = BitConverter.GetBytes(y);
            System.Buffer.BlockCopy(_y, 0, data, 5, 2);
            System.Buffer.BlockCopy(cells, 0, data, 7, cells.Length);
            Send("HB", data);
        }
        public delegate void TypedEvent();
        public delegate void TYTypedEvent();
        public Dictionary<string, TYTypedEvent> tyevents;
        public Dictionary<string, TypedEvent> te;
        public void DecodeRecive(Packet p)
        {
            te[p.eventType]();
        }
    }
}
