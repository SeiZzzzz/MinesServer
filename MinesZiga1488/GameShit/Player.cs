using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.Server;
using NetCoreServer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace MinesServer.GameShit
{
    public class Player
    {
        public bool locked = false;
        public int Id { get; set; }
        public string name { get; set; }
        public long money { get; set; }
        public long creds { get; set; }
        public string hash { get; set; }
        public string passwd { get; set; }
        public int tail { get; set; }
        public int skin { get; set; }
        public int clanid { get; set; }
        public bool autoDig { get; set; }
        public Vector2 pos = Vector2.Zero;
        public Basket crys { get; set; }
        public Inventory inventory { get; set; }
        public Health health { get; set; }
        public Stack<byte> geo = new Stack<byte>();
        Queue<Line> console = new Queue<Line>();
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
        public Packs inside = Packs.None;
        [NotMapped]
        public object insidesmf
        {
            get
            {
                if (locked)
                {
                    return true;
                }
                return inside;
            }
            set
            {
                if (!(bool)value)
                {
                    locked = false;
                    inside = Packs.None;
                }
            }
        }
        public void Update()
        {
            if (DateTime.Now - lastPlayersend > TimeSpan.FromMilliseconds(500))
            {
                SendBots();
                lastPlayersend = DateTime.Now;
            }
        }
        public DateTime lastPlayersend = DateTime.Now;
        public Player()
        {
            Delay = DateTime.Now;
        }
        private class Line
        {
            public string text { get; set; }
        }
        public void ShowConsole()
        {
            locked = true;
            new Builder()
                .AddIConsole()
                .AddIConsolePlace("cmd")
                .AddTextLines(console.Select(x => x.text).ToArray())
                .AddButton("ВЫПОЛНИТЬ", "%I%")
                .AddButton("ВЫЙТИ", "exit")
                .Send(connection);
        }
        public bool CanAct { get { return Delay < DateTime.Now; } }
        public void AddDelay(double ms)
        {
            Delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        }
        public void Bz(int x, int y)
        {
            var cell = World.W.GetCell(x, y);
            World.W.SetCell(x, y, 35);
        }
        public Vector2 GetDirCord()
        {
            var x = (uint)(pos.X + (dir == 3 ? 1 : dir == 1 ? -1 : 0));
            var y = (uint)(pos.Y + (dir == 0 ? 1 : dir == 2 ? -1 : 0));
            return new Vector2(x, y);
        }
        public void Move(int x, int y, int dir)
        {
            try
            {
                if (!CanAct || !World.W.ValidCoord(x, y))
                {
                    AddDelay(0.1);
                    connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                    return;
                }

                var cell = World.W.GetCell(x, y);
                if (!World.GetProp(cell).isEmpty)
                {
                    connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                    AddDelay(0.01);
                    return;
                }
                var newpos = new Vector2(x, y);
                this.dir = dir;
                if (Vector2.Distance(pos, newpos) < 1.2f)
                {
                    pos = newpos;
                }
                SendMap();
                AddDelay(0.01);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            if (MServer.Instance.players.Keys.Contains(this.Id))
            {
                MServer.Instance.players.Remove(this.Id);
                this.connection.Disconnect();
                return;
            }
            MServer.Instance.players.Add(Id, connection);
            connection.auth = null;
            using var db = new DataBase();
            crys = db.baskets.First(x => x.Id == Id);
            inventory = db.inventories.First(x => x.Id == Id);
            health = db.healths.First(x => x.Id == Id);
            y = 1;
            x = World.W.gen.spawns[new Random().Next(World.W.gen.spawns.Count)].Item1;
            SendPing();
            SendWorldInfo();
            Send("sp", "125:57:200");
            Send("BA", "0");
            Send("BD", "0");
            Send("GE", " ");
            Send("@T", $"{x}:{y}");
            SendBInfo();
            Send("sp", "25:20:100000");
            SendCrys();
            SendHp();
            SendMoney();
            SendLvl();
            SendMap();
            console.Enqueue(new Line { text = "@@> Добро пожаловать в консоль!" });
            for (var i = 0; i < 4; i++)
            {
                AddConsoleLine();
            }

            AddConsoleLine("Если вы не понимаете, что происходит,");
            AddConsoleLine("или вас попросили выполнить команду,");
            AddConsoleLine("сосите хуй глотайте сперму");
            for (var i = 0; i < 8; i++)
            {
                AddConsoleLine();
            }

        }
        public void AddConsoleLine(string text)
        {
            console.Enqueue(new Line { text = ">" + text });
            if (console.Count > 16)
            {
                console.Dequeue();
                var l = console.First();
                l.text = "@@" + l.text;
            }
        }

        public void AddConsoleLine()
        {
            console.Enqueue(new Line { text = ">    " });
            if (console.Count > 16)
            {
                console.Dequeue();
                var l = console.Peek();
                l.text = "@@" + l.text;
            }
        }
        public void SendCrys()
        {
            Send("@B", crys.GetCry);
        }
        public void SendWorldInfo()
        {
            Send("cf",
            "{\"width\":" + World.W.width + ",\"height\":" + World.W.height +
                ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            Send("CF",
            "{\"width\":" + World.W.width + ",\"height\":" + World.W.height +
                ",\"name\":\"" + World.W.name + "\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
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
            Send("BI", "{\"x\":" + pos.X + ",\"y\":" + pos.Y + ",\"id\":" + Id + ",\"name\":\"" + name + "\"}");
        }
        public void SendLvl()
        {
            var i = 0;
            Send("LV", i.ToString());
        }
        public void SendBots()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = (ChunkX + xxx);
                    var y = (ChunkX + yyy);
                    if (valid(x, y))
                    {
                        var ch = World.W.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var j = MServer.GetPlayer(id.Key);
                            if (j != null)
                            {
                                var player = j.player;
                                connection.SendBot(player.Id, (uint)player.pos.X, (uint)player.pos.Y, player.dir,
                                    player.clanid, player.skin, player.tail);
                                connection.SendNick(id.Key, player.name);
                            }
                        }
                    }
                }
            }
        }
        public bool needupdmap = true;
        public void SendMap()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            if (!valid(ChunkX, ChunkY))
            {
                return;
            }
            if (lastchunk != (ChunkX, ChunkY) || needupdmap)
            {
                MoveToChunk(ChunkX, ChunkY);
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        var cx = (ChunkX + x);
                        var cy = (ChunkY + y);
                        if (valid(cx, cy))
                        {
                            var ch = World.W.chunks[cx, cy];
                            if (ch != null)
                            {
                                cx *= 32; cy *= 32;
                                connection.SendCells(32, 32, cx, cy, ch.cells);
                            }
                        }
                    }
                }
                needupdmap = false;
            }
        }
        public void SendLocalMsg(byte[] msg)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = ChunkX + xxx;
                    var y = ChunkY + yyy;
                    if (valid(x,y))
                    {
                        var ch = World.W.chunks[x, y];
                        foreach (var id in ch.bots)
                        {
                            var player = MServer.GetPlayer(id.Key);
                            player.SendLocalChat(msg.Length, Id, this.x, this.y,
                                msg);
                        }
                    }
                }
            }
        }
        public void Send(string t, string c)
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
        public void OnDisconnect()
        {
            var chtoremove = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
            if (chtoremove.bots.ContainsKey(this.Id))
            {
                chtoremove.bots.Remove(this.Id);
            }
        }
        public void MoveToChunk(int x, int y)
        {
            if (lastchunk != null && World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2] != null)
            {
                var chtoremove = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
                if (chtoremove.bots.ContainsKey(this.Id))
                {
                    chtoremove.bots.Remove(this.Id);
                }
            }
            var chtoadd = World.W.chunks[x, y];
            lastchunk = (x, y);
            if (World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2] != null)
            {
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
