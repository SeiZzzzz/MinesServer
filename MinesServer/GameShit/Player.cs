using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Skills;
using MinesServer.Network.World;
using MinesServer.Network;
using MinesServer.Server;
using NetCoreServer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using System.Security.Cryptography;
using MinesServer.Network.BotInfo;
using MinesServer.Network.GUI;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Movement;

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
        public PlayerSkills skillslist { get; set; }
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
                ReSendBots();
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
        public bool CanAct { get { return Delay <= DateTime.Now; } }
        public void AddDelay(double ms)
        {
            Delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        }
        public void Bz(int x, int y)
        {
            var cell = World.W.GetCell(x, y);
            var d = World.W.map.GetDurability(x, y);
            var hitdmg = 1f;
            foreach(var c in skillslist.skills)
            {
                if (c != null && c.UseSkill(c.effecttype,this))
                {
                    if (c.name == "d")
                    {
                        hitdmg *= c.GetEffect() / 100f;
                        Console.WriteLine(c.GetExp());
                    }
                    c.Up();
                }
            }
            if ((d - hitdmg) <= 0)
            {
                World.W.map.SetDurability(x, y, 0);
                World.W.Destroy(x, y);
            }
            else if (d >= 0)
            {
                World.W.map.SetDurability(x, y, d - hitdmg);
            }
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
                    tp(this.x, this.y);
                    return;
                }

                var cell = World.W.GetCell(x, y);
                if (!World.GetProp(cell).isEmpty)
                {
                    tp(this.x, this.y);
                    return;
                }
                var newpos = new Vector2(x, y);
                this.dir = dir;
                if (Vector2.Distance(pos, newpos) < 1.2f)
                {
                    pos = newpos;
                }
                else
                {
                    tp(this.x, this.y);
                }
                SendMap();
                AddDelay(0.000001);
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
            inventory.items = new int[49];
            crys = new Basket(this);
            skillslist = new PlayerSkills();
            AddBasicSkills();
            pos = new Vector2(0, 0);
            dir = 0;
            clanid = 0;
            skin = 0;
            tail = 0;
            using var db = new DataBase();
            db.SaveChanges();
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
            new MoneyPacket(this.money, this.creds);
            this.connection.SendU(new MoneyPacket(this.money, this.creds));
        }
        private void AddBasicSkills()
        {
            skillslist.InstallSkill("m", 0);
            skillslist.InstallSkill("d", 1);
            skillslist.InstallSkill("M", 2);
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
            using (var db = new DataBase())
            {
                crys = db.baskets.First(x => x.Id == Id);
                crys.player = this;
                inventory = db.inventories.First(x => x.Id == Id);
                health = db.healths.First(x => x.Id == Id);
                skillslist = db.skills.First(x => x.Id == Id);
                skillslist.LoadSkills();
            }
            y = 1;
            x = World.W.gen.spawns[new Random().Next(World.W.gen.spawns.Count)].Item1;
            connection.SendPing();
            connection.SendWorldInfo();
            SendAutoDigg();
            SendGeo();
            tp(x, y);
            SendBInfo();
            SendSpeed();
            SendCrys();
            SendHp();
            SendMoney();
            SendLvl();
            SendMap();
            SendInventory();
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
        public void SendAutoDigg()
        {
            connection.SendU(new AutoDiggPacket(autoDig));
        }
        public void tp(int x,int y)
        {
            connection.SendU(new TPPacket(x, y));
        }
        public void SendGeo()
        {
            connection.SendU(new GeoPacket(""));
        }
        public void SendCrys()
        {
            crys.SendBasket();
        }
        public void SendSpeed()
        {
            connection.SendU(new SpeedPacket(25,20,100000));
        }
        public void SendInventory()
        {
            connection.SendU(inventory.InvToSend());
        }
        public void SendHp()
        {
            connection.SendU(new LivePacket(health.HP, health.MaxHP));
        }
        public void SendBInfo()
        {
            connection.SendU(new BotInfoPacket(name,(int)pos.X,(int)pos.Y,Id));
        }
        public void SendLvl()
        {
            var i = 0;
            connection.SendU(new LevelPacket(i));
        }
        public void SendOnline()
        {
            connection.SendU(new OnlinePacket(connection.online, 0));
        }
        public void ReSendBots()
        {
            List<IDataPartBase> packets = new();
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
                                packets.Add(new HBPacket([new HBBotPacket(Id, (int)pos.X, (int)pos.Y, dir, 0, clanid, 0)]));
                            }
                        }
                    }
                }
            }
            connection.SendB(new HBPacket(packets.ToArray()));
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
                List<IDataPartBase> packets = new();
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        var cx = (ChunkX + x);
                        var cy = (ChunkY + y);
                        if (valid(cx, cy))
                        {
                            var ch = World.W.chunks[cx, cy];
                            ch.active = true;
                            ch.Load();
                            if (ch != null)
                            {
                                
                                cx *= 32; cy *= 32;
                                packets.Add(new HBPacket([new HBMapPacket(cx, cy, 32, 32, ch.cells)]));
                                foreach (var id in ch.bots)
                                {
                                    var j = MServer.GetPlayer(id.Key);
                                    if (j != null)
                                    {
                                        var player = j.player;
                                        packets.Add(new HBPacket([new HBBotPacket(Id, (int)pos.X, (int)pos.Y, dir, 0, clanid, 0)]));
                                    }
                                }
                            }
                        }
                    }
                }
                connection.SendB(new HBPacket(packets.ToArray()));
                needupdmap = false;
            }
        }
        public void SendLocalMsg(string msg)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = ChunkX + xxx;
                    var y = ChunkY + yyy;
                    if (valid(x, y))
                    {
                        var ch = World.W.chunks[x, y];
                        foreach (var id in ch.bots)
                        {
                            var player = MServer.GetPlayer(id.Key);
                            if (player != null)
                            {
                                player.SendLocalChat(msg);
                            }
                        }
                    }
                }
            }
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
            if (lastchunk.HasValue)
            {
                var chtoremove = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
                if (chtoremove.bots.ContainsKey(this.Id))
                {
                    chtoremove.bots.Remove(this.Id);
                }
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
