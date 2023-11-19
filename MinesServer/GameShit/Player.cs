using MinesServer.Enums;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.Skills;
using MinesServer.Network;
using MinesServer.Network.BotInfo;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.Movement;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Numerics;

namespace MinesServer.GameShit
{
    public class Player
    {
        public Player() => Delay = DateTime.Now;
        public DateTime lastPlayersend = DateTime.Now;
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
        public Queue<Line> console = new Queue<Line>();
        [NotMapped]
        public Window win;
        [NotMapped]
        private float cb;
        public DateTime Delay;
        public bool CanAct { get { return Delay <= DateTime.Now; } }
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
                if (win != null)
                {
                    return true;
                }
                return inside;
            }
            set
            {
                if (!(bool)value)
                {
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
        public void AddDelay(double ms) //delay on action
        {
            Delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        }
        private int ParseCryType(CellType cell)
        {
            return cell switch
            {
                CellType.XGreen or CellType.Green => 0,
                CellType.XBlue or CellType.Blue => 1,
                CellType.XRed or CellType.Red => 2,
                CellType.XViolet or CellType.Violet => 3,
                CellType.White => 4,
                CellType.XCyan or CellType.Cyan => 5,
                _ => 0
            };
        }
        private void Mine(byte cell, int x, int y)
        {
            float dob = 1 + (float)Math.Truncate(cb);
            dob *= (CellType)cell switch
            {
                CellType.XGreen => 4,
                CellType.XBlue => 3,
                CellType.XRed => 2,
                CellType.XViolet => 2,
                CellType.XCyan => 2,
                _ => 1
            };
            cb -= (float)Math.Truncate(cb);
            long odob = (long)Math.Truncate(dob);
            var type = ParseCryType((CellType)cell);
            foreach (var c in skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDigCrys, this))
                {
                    if (c.name == "m")
                    {
                        dob += c.GetEffect();
                        c.AddExp(this, (float)Math.Truncate(dob));
                    }
                    c.Up(this);
                }
            }
            cb += dob - odob;
            crys.AddCrys(type, odob);
            SendDFToBots(2, x, y, (int)odob, type);
        }
        public void Bz(int x, int y)
        {
            SendDFToBots(0, x, y, this.dir);
            var cell = World.W.GetCell(x, y);
            /*
            if (!World.GetProp(cell).is_destructible)
            {
                return;
            }*/
            var d = World.W.map.GetDurability(x, y);
            var hitdmg = 0.2f;
            if (World.GetProp(cell).isCry)
            {
                hitdmg = 1f;
                Mine(cell,x,y);
            }
            else
            {
                foreach (var c in skillslist.skills)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                    {
                        hitdmg = c.name switch
                        {
                            "d" => hitdmg * (c.GetEffect() / 100f),
                            _ => 1f
                        };
                    }
                }
            }
            if ((d - hitdmg) <= 0)
            {
                foreach (var c in skillslist.skills)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                    {
                        c.AddExp(this);
                        c.Up(this);
                    }
                }
                World.W.map.SetDurability(x, y, 0);
                World.W.Destroy(x, y);
            }
            else if (d >= 0)
            {
                World.W.map.SetDurability(x, y, d - hitdmg);
            }
        }
        public void Move(int x, int y, int dir)
        {
            foreach (var c in skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnMove, this))
                {
                    if (c.name == "M")
                    {
                        if (c.isUpReady())
                        {
                            //ресенд мувхендлера с зависимостью
                            c.Up(this);
                        }
                        c.AddExp(this);
                    }
                }
            }
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
            inventory = new Inventory();
            inventory.items = new int[49];
            crys = new Basket(this);
            skillslist = new PlayerSkills();
            AddBasicSkills();
            health.LoadHealth(this);
            pos = new Vector2(0, 0);
            dir = 0;
            clanid = 0;
            skin = 0;
            tail = 0;
            using var db = new DataBase();
            db.SaveChanges();
        }
        private void AddBasicSkills()
        {
            //базовые скиллы
            skillslist.InstallSkill(SkillType.MineGeneral.GetCode(), 0);
            skillslist.InstallSkill(SkillType.Digging.GetCode(), 1);
            skillslist.InstallSkill(SkillType.Movement.GetCode(), 2);
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
                health.LoadHealth(this);
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
            health.SendHp();
            SendMoney();
            SendLvl();
            SendMap();
            SendInventory();
            console.Enqueue(new Line { text = "@@> Добро пожаловать в консоль!" });
            for (var i = 0; i < 4; i++)
            {
                MConsole.AddConsoleLine(this);
            }

            MConsole.AddConsoleLine(this, "Если вы не понимаете, что происходит,");
            MConsole.AddConsoleLine(this,"или вас попросили выполнить команду,");
            MConsole.AddConsoleLine(this,"сосите хуй глотайте сперму");
            for (var i = 0; i < 8; i++)
            {
                MConsole.AddConsoleLine(this);
            }

        }
        #region senders
        public void SendWindow()
        {
            connection.SendU(new GUIPacket(this.win.ToString()));
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
        public void SendAutoDigg()
        {
            connection.SendU(new AutoDiggPacket(autoDig));
        }
        public void tp(int x, int y)
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
            connection.SendU(new SpeedPacket(25, 20, 100000));
        }
        public void SendInventory()
        {
            connection.SendU(inventory.InvToSend());
        }
        public void SendBInfo()
        {
            connection.SendU(new BotInfoPacket(name, (int)pos.X, (int)pos.Y, Id));
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
                                packets.Add(new HBBotPacket(Id, (int)pos.X, (int)pos.Y, dir, 0, clanid, 0));
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
                                packets.Add(new HBMapPacket(cx, cy, 32, 32, ch.cells));
                                foreach (var id in ch.bots)
                                {
                                    var j = MServer.GetPlayer(id.Key);
                                    if (j != null)
                                    {
                                        var player = j.player;
                                        packets.Add(new HBBotPacket(Id, (int)pos.X, (int)pos.Y, dir, 0, clanid, 0));
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
        public void SendDFToBots(int fx, int fxx, int fxy, int dir, int col = 0)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (valid(this.ChunkX + xxx, this.ChunkY + yyy))
                    {
                        var x = (this.ChunkX + xxx);
                        var y = (this.ChunkY + yyy);
                        var ch = World.W.chunks[x, y];

                        foreach (var player in ch.bots.Select(id => MServer.GetPlayer(id.Key)))
                        {
                            player.SendB(new HBPacket([new HBDirectedFXPacket(this.Id, fxx, fxy, fx, dir, col)]));
                        }
                    }
                }
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
        #endregion
        public string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public void CallWinAction(string text)
        {
            if (win == null)
            {
                connection.SendU(new GuPacket());
                return;
            }
            win.ProcessButton(text);
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
