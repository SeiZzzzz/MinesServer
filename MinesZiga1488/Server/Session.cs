using MinesServer.GameShit;
using MinesServer.GameShit.GUI;
using NetCoreServer;
using System;
using System.CodeDom.Compiler;
using System.Text;

namespace MinesServer.Server
{
    public class Session : TcpSession
    {
        MServer father;
        public Player player;
        public Auth auth;
        public Session(TcpServer server) : base(server) { father = server as MServer; tyevents = new Dictionary<string, TYEventAction>(); ; te = new Dictionary<string, EventAction>(); InitEvents(); }
        public void InitEvents()
        {
            te.Add("AU", (p) =>
            {
                auth = new Auth();
                auth.TryToAuth(p, sid, this);
            });
            te.Add("PO", (p) =>
            {
                var po = Encoding.Default.GetString(p.data).Split(':');
                Send("PI", $"{0}:{0}:{int.Parse(po[0]) - 10}");
            });
            InitTYEvents();
            te.Add("TY", (p) =>
            {
                var ty = new TYPacket(p.data);
                if (tyevents.Keys.Contains(ty.eventType))
                {
                    tyevents[ty.eventType](ty);
                }
            });
        }
        public void InitTYEvents()
        {
            tyevents.Add("Xmov", (ty) =>
            {
                father.time.AddAction(() =>
                {
                    int.TryParse(Encoding.UTF8.GetString(ty.data).Trim(), out var dir);
                    player.Move(ty.x, ty.y, dir > 9 ? dir - 10 : dir);
                });
            });
            tyevents.Add("GUI_", GUI);
            tyevents.Add("Locl", (ty) =>
            {
                var msg = Encoding.Default.GetString(ty.data);
                if (msg == "console")
                {
                    this.player.ShowConsole();
                }
            });
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            this.DecodeRecive(new Packet(buffer));
        }
        protected override void OnDisconnected()
        {
            using var db = new DataBase();
            db.SaveChanges();
            if (player == null)
            {
                return;
            }
            father.players.Remove(player.Id);

        }
        public void GUI(TYPacket ty)
        {
            Newtonsoft.Json.Linq.JObject jo = null;
            try
            {
                jo = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(ty.data));
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return;
            }
            if (jo == null)
            {
                return;
            }
            if ((auth != null && !auth.complited))
            {
                var button = jo["b"];
                if (button.ToString() == "exit")
                {
                    auth.temp = null;
                    auth.nick = "";
                    auth.passwd = "";
                    auth.createnew = false;
                    new Builder()
                        .SetTitle("ВХОД")
                        .AddTextLine("Ник")
                        .AddIConsole()
                        .AddIConsolePlace("")
                        .AddButton("ОК", "%I%")
                        .AddButton("НОВЫЙ АКК", "newakk")
                        .Send(this);
                    return;
                }    
                if (auth.createnew)
                {
                    if (auth.nick == "")
                    {
                        if (Auth.NickNotAvl(button.ToString()))
                        {
                            new Builder()
                            .SetTitle("НОВЫЙ ИГРОК")
                            .AddTextLine("Ник")
                            .AddTextLine("Ник не доступен")
                            .AddIConsole()
                            .AddIConsolePlace("")
                             .AddButton("ОК", "%I%")
                        .Send(this);
                            return;
                        }
                        auth.nick = button.ToString();
                        auth.SetPasswdForNew(this);
                    }
                    else if(auth.passwd == "")
                    {
                        auth.passwd = button.ToString();
                        auth.EndCreateAndInit(this);
                        auth.createnew = false;
                    }
                    return;
                }
                if (button.ToString().StartsWith("newakk"))
                {
                    auth.CreateNew(this);
                }
                else if(auth.nick == "")
                {
                    auth.TryToFindByNick(button.ToString(),this);
                }
                else if(auth.passwd == "" && auth.temp != null)
                {
                    auth.TryToAuthByPlayer(button.ToString(), this);
                }
                return;
            }
            father.time.AddAction(() =>
            {
                HorbDecoder.Decode(jo["b"].ToString(), player);
            });
        }
        protected override void OnConnected()
        {
            sid = Auth.GenerateSessionId();
            //load from bd
            Console.WriteLine($"{this.ToString()} connected");
            Send("ST", "черный хуй в твоей жопе");
            Send("AU", sid);
        }
        public string sid { get; set; }
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
            try
            {
                SendAsync(p.Compile);
            }
            catch (Exception ex) { }
        }
        public void SendCell(int x, int y, byte cell)
        {
            if ((x >= 0 && y >= 0) && (x <= World.W.width && y <= World.W.height))
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
        public void SendBot(int bid, uint x, uint y, int dir, int cid, int skin, int tail)
        {
            var data = new byte[13];
            data[0] = (byte)'X';
            data[1] = (byte)dir;
            data[2] = (byte)skin;
            data[3] = (byte)tail;
            System.Buffer.BlockCopy(BitConverter.GetBytes(bid), 0, data, 4, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 6, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 8, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(cid), 0, data, 10, 2);
            Send("HB", data);
        }
        public void SendNick(int id, string nick)
        {
            Send("NL", id + ":" + nick);
        }
        public delegate void EventAction(Packet p);
        public delegate void TYEventAction(TYPacket tp);
        public Dictionary<string, TYEventAction> tyevents;
        public Dictionary<string, EventAction> te;
        public void DecodeRecive(Packet p)
        {
            if (te.Keys.Contains(p.eventType))
            {
                te[p.eventType](p);
            }
        }
    }
}
