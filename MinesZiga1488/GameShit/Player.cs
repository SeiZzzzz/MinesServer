using MinesServer.Server;
using NetCoreServer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Security.Cryptography;

namespace MinesServer.GameShit
{
    public class Player
    {
        public int Id { get; set; }
        public string name { get; set; }
        public long money { get; set; }
        public long creds { get; set; }
        public string hash { get; set; }
        public string passwd { get; set; }
        public Health health { get; set; }
        public int tail { get; set; }
        public int skin { get; set; }
        public int clanid { get; set; }
        public Vector2 pos = Vector2.Zero;
        public Basket crys { get; set; }
        public Inventory inventory { get; set; }
        public Stack<byte> geo = new Stack<byte>();
        public DateTime Delay;
        public int dir { get; set; }
        public int x
        {
            get => (int)pos.X;
            set => pos.X = value;

        }
        public int y
        {
            get => (int)pos.Y; 
            set => pos.Y = value;

        }
        public int ChunkX
        {
            get => (int)Math.Floor(pos.X / 32);
        }
        public int ChunkY
        {
            get => (int)Math.Floor(pos.Y / 32);
        }
        public Player()
        {
            Delay = DateTime.Now;
        }
        public bool CanAct { get { return Delay < DateTime.Now; } }
        public void AddDelay(double ms)
        {
            Delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        }
        public void Move(int x,int y,int dir)
        {
            if (CanAct)
            {
                if (!World.W.ValidCoord(x,y))
                {
                    var cell = World.W.GetCell(x, y);
                    if (!cell.isEmpty)
                    {
                        AddDelay(0.1);
                        return;
                    }
                    var newpos = new Vector2(x, y);
                    if (Vector2.Distance(pos, newpos) < 1.2f)
                    {
                        pos = newpos;
                    }
                    SendMap();
                }
                AddDelay(0.1);
            }
        }
        public void CreatePlayer()
        {
            name = "";
            money = 1000;
            creds = 0;
            hash = GenerateHash();
            passwd = "";
            health = new Health();
            health.MaxHP = 100;
            health.HP = 100;
            inventory = new Inventory();
            crys = new Basket(this);
            pos = new Vector2(0, 0);
            dir = 0;
            clanid = 0;
            skin = 0;
            tail = 0;
        }
        public void SendMoney()
        {

            if (this.money < 0)
            {
                this.money = long.MaxValue;
            }
            if (this.creds < 0)
            {
                this.creds = long.MaxValue;
            }
            this.connection.Send("P$", new V { money = this.money, creds = this.creds }.ToString());
        }
        public struct V
        {
            public long money;
            public long creds;

            public override string ToString()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
        }
        public void Init()
        {
            SendPing();
            SendWorldInfo();
            Send("sp", "125:57:200");
            Send("BA", "0");
            Send("BD", "0");
            Send("GE", " ");
            Send("@T", $"{x}:{y}");
            SendBInfo();
            Send("sp", "25:20:100000");
            Send("@B", this.crys.GetCry);
            SendHp();
            SendMoney();
            SendLvl();
            SendMap();

        }
        public string GenerateSessionId()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnoprtsuxyz0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public void SendWorldInfo()
        {
            Send("cf",
            "{\"width\":" + MServer.Instance.wrld.width + ",\"height\":" + MServer.Instance.wrld.height +
                ",\"name\":\"" + MServer.Instance.wrld.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            Send("CF",
            "{\"width\":" + MServer.Instance.wrld.width + ",\"height\":" + MServer.Instance.wrld.height +
                ",\"name\":\"" + MServer.Instance.wrld.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
        }
        public void SendPing()
        {
            Send("PI", "0:0:0");
        }
        public void SendHp()
        {
            Send("@L", health.HP + ":" + health.MaxHP);
        }
        public void SendBInfo()
        {
            Send("BI","{\"x\":" + pos.X + ",\"y\":" + pos.Y + ",\"id\":" + Id + ",\"name\":\"" + name + "\"}");
        }
        public void SendLvl()
        {
            var i = 0;
            Send("LV", i.ToString());
        }
        public void SendBots()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < MServer.Instance.wrld.chunksCountW && y < MServer.Instance.wrld.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = (ChunkX + xxx);
                    var y = (ChunkX + yyy);
                    if (valid(x,y))
                    { 
                        var ch = World.W.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = MServer.Instance.players[id.Key].player;

                            connection.SendBot(player.Id, (uint)player.pos.X, (uint)player.pos.Y, player.dir,
                                player.clanid, player.skin, player.tail);
                            connection.SendNick(id.Key, player.name);
                        }
                    }
                }
            }
        }
        public bool needupdmap = true;
        public void SendMap()
        {
            var valid = bool (uint x, uint y) => (x >= 0 && y >= 0) && (x < MServer.Instance.wrld.chunksCountW && y < MServer.Instance.wrld.chunksCountH);
            if (!valid(ChunkX,ChunkY))
            {
                return;
            }
            if (lastchunk != (ChunkX, ChunkY) || needupdmap)
            {
                MoveToChunk(ChunkX, ChunkY);
                lastchunk = lastchunk == null ? (ChunkX, ChunkY) : lastchunk;
                for (int x = -2;x <= 2;x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        var cx = (ChunkX + x);
                        var cy = (ChunkY + y);
                        if (valid(cx,cy))
                        {
                            var ch = World.W.chunks[cx, cy];
                            if (ch != null)
                            {
                                cx *= 32; cy *= 32;
                                connection.SendCells(32, 32, cx, cy, ch.getCells());
                            }
                        }
                    }
                }
                needupdmap = false;
            }
        }
        public void Send(string t,string c)
        {
            this.connection.Send(t, c);
        }
        public string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public void MoveToChunk(int x,int y)
        {
            lastchunk = lastchunk == null ? (x, y) : lastchunk;
            var chtoremove = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
            var chtoadd = World.W.chunks[x, y];
            if (World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2] != null)
            {
                if (chtoremove.bots.ContainsKey(this.Id))
                {
                    chtoremove.bots.Remove(this.Id);
                }

                if (!chtoadd.bots.ContainsKey(this.Id))
                {
                    chtoadd.AddBot(this);
                }
            }
        }
        [NotMapped]
        public (int, int)? lastchunk { get; private set; }
        [NotMapped]
        public Session connection { get; set; }

    }
}
