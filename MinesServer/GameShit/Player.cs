using MinesServer.Enums;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Skills;
using MinesServer.Network;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.Movement;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using NetCoreServer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MinesServer.GameShit
{
    public class Player
    {
        #region fields
        [NotMapped]
        public Session? connection { get; set; }
        public Player() => Delay = DateTime.Now;
        public DateTime lastPlayersend = DateTime.Now;
        public int Id { get; set; }
        public string name { get; set; }
        public int respid { get; set; }
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
        public Settings settings { get; set; }
        public PlayerSkills skillslist { get; set; }
        public Stack<byte> geo = new Stack<byte>();
        public Queue<Line> console = new Queue<Line>();
        [NotMapped]
        public Window? win;
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
        #endregion
        #region actions
        public void SetResp(Resp r)
        {
            respid = r.id;
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
        public Vector2 GetDirCord(bool pack = false)
        {
            var x = (uint)(pos.X + (dir == 3 ? 1 : dir == 1 ? -1 : 0));
            var y = (uint)(pos.Y + (dir == 0 ? 1 : dir == 2 ? -1 : 0));
            if (pack)
            {
                x = (uint)(pos.X + (dir == 3 ? 2 : dir == 1 ? -2 : 0));
                y = (uint)(pos.Y + (dir == 0 ? 2 : dir == 2 ? -2 : 0));
            }
            return new Vector2(x, y);
        }
        public void Geo()
        {
            int x = (int)GetDirCord().X, y = (int)GetDirCord().Y;
            if (!World.W.ValidCoord(x,y))
            {
                return;
            }
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).isPickable && !World.GetProp(cell).isEmpty)
            {
                geo.Push(cell);
                World.Destroy(x, y);
            }
            else if(World.GetProp(cell).isEmpty && World.GetProp(cell).can_place_over && geo.Count > 0 && !World.PackPart(x,y))
            {
                World.SetCell(x, y, geo.Pop());
                World.SetDurability(x, y, 0);
            }
            SendGeo();
        }
        public void BBox(long[]? c)
        {
            var boxc = GetDirCord();
            if (!World.W.ValidCoord((int)boxc.X, (int)boxc.Y) || c == null)
            {
                return;
            }
            Box.BuildBox((int)boxc.X, (int)boxc.Y, c, this);
            connection.CloseWindow();
            
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
                    if (c.type == SkillType.MineGeneral)
                    {
                        dob += c.GetEffect();
                        c.AddExp(this, (float)Math.Truncate(dob));
                    }
                }
            }
            cb += dob - odob;
            crys.AddCrys(type, odob);
            SendDFToBots(2, x, y, (int)odob, type);
        }
        public void GetBox(int x, int y)
        {
            var b = Box.GetBox(x, y);
            if (b == null)
            {
                return;
            }
            crys.Boxcrys(b.bxcrys);
            crys.SendBasket();
            using var db = new DataBase();
            db.Remove(b);
            db.SaveChanges();
            connection?.SendB(new HBPacket([new HBChatPacket(Id, x, y, "+" + b.AllCrys)]));
        }
        private void OnDestroy(byte type)
        {
            foreach (var c in skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                {
                    c.AddExp(this);
                }
            }
        }
        public void Bz()
        {
            int x = (int)GetDirCord().X, y = (int)GetDirCord().Y;
            SendDFToBots(0, x, y,dir);
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).damage > 0)
            {
                health.Hurt(World.GetProp(cell).damage);
            }
            if (!World.GetProp(cell).is_diggable)
            {
                return;
            }
            if (cell == 90)
            {
                GetBox(x, y);
                World.DamageCell(x, y, 1);
                return;
            }
            float hitdmg = 0.2f;
            if (World.isCry(cell))
            {
                hitdmg = 1f;
                Mine(cell, x, y);
            }
            else
            {
                foreach (var c in skillslist.skills)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                    {
                        hitdmg = c.type switch
                        {
                            SkillType.Digging => hitdmg * (c.GetEffect() / 100f),
                            _ => 1f
                        };
                    }
                }
            }
            if (World.DamageCell(x, y, hitdmg)) OnDestroy(cell);
        }
        public void Move(int x, int y, int dir)
        {
            foreach (var c in skillslist.skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnMove, this))
                {
                    if (c.type == SkillType.Movement)
                    {
                        c.AddExp(this);
                    }
                }
            }
            try
            {
                if (!CanAct || !World.W.ValidCoord(x, y) || win != null)
                {
                    tp(this.x, this.y);
                    return;
                }

                var cell = World.GetCell(x, y);
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
                SendMyMove();
                SendMap();
                AddDelay(0.000001);
                if(World.ContainsPack(x,y,out var pack))
                {
                    win = pack.GUIWin(this)!;
                    SendWindow();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
        #region creating
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
            settings = new Settings();
            crys = new Basket(this);
            skillslist = new PlayerSkills();
            AddBasicSkills();
            health.LoadHealth(this);
            pos = new Vector2(0, 0);
            dir = 0;
            clanid = 0;
            skin = 0;
            tail = 0;
            RandomResp();
        }
        public void RandomResp()
        {
            using var db = new DataBase();
            var re = db.resps.Where(i => i.ownerid == 0);
            var resp = re.FirstOrDefault()!;
            var pos = resp.GetRandompoint();
            this.pos = new Vector2(pos.Item1, pos.Item2);
            SetResp(resp);
            db.SaveChanges();
        }
        public Resp? GetCurrentResp()
        {
            using var db = new DataBase();
            return db.resps.FirstOrDefault(i => i.id == respid);
        }
        private void AddBasicSkills()
        {
            //базовые скиллы
            skillslist.InstallSkill(SkillType.MineGeneral.GetCode(), 0,this);
            skillslist.InstallSkill(SkillType.Digging.GetCode(), 1,this);
            skillslist.InstallSkill(SkillType.Movement.GetCode(), 2,this);
        }
        public void Init()
        {
            if (MServer.Instance.players.Keys.Contains(Id))
            {
                MServer.Instance.players.Remove(Id);
                connection?.Disconnect();
                return;
            }
            MServer.Instance.players.Add(Id, this);
            connection.auth = null;
            using (var db = new DataBase())
            {
                crys = db.baskets.First(x => x.Id == Id);
                crys.player = this;
                inventory = db.inventories.First(x => x.Id == Id);
                health = db.healths.First(x => x.Id == Id);
                settings = db.settings.First(x => x.id == Id);
                skillslist = db.skills.First(x => x.Id == Id);
                skillslist.LoadSkills();
                health.LoadHealth(this);
            }
            MoveToChunk(ChunkX, ChunkY);
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
            MConsole.AddConsoleLine(this, "или вас попросили выполнить команду,");
            MConsole.AddConsoleLine(this, "сосите хуй глотайте сперму");
            for (var i = 0; i < 8; i++)
            {
                MConsole.AddConsoleLine(this);
            }
            SendMap();

        }
        #endregion
        #region senders
        public void SendWindow()
        {
            if (win != null)
            {
                connection?.SendU(new GUIPacket(win.ToString()));
                return;
            }
            connection?.SendU(new GuPacket());
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
            connection?.SendU(new MoneyPacket(this.money, this.creds));
        }
        public void SendAutoDigg()
        {
            connection?.SendU(new AutoDiggPacket(autoDig));
        }
        public void HurtEffect(int x,int y)
        {
            SendFXoBots(0, x, y);
        }
        public void tp(int x, int y)
        {
            connection?.SendU(new TPPacket(x, y));
        }
        public void SendGeo()
        {
            if (geo.Count > 0)
            {
                connection?.SendU(new GeoPacket(World.GetProp(geo.Peek()).name));
                return;
            }
            connection?.SendU(new GeoPacket(""));
        }
        public void SendCrys()
        {
            crys.SendBasket();
        }
        public void SendSpeed()
        {
            connection?.SendU(new SpeedPacket(25, 20, 100000));
        }
        public void SendInventory()
        {
            connection?.SendU(inventory.InvToSend());
        }
        public void SendBInfo()
        {
            connection?.SendU(new BotInfoPacket(name, (int)pos.X, (int)pos.Y, Id));
        }
        public void SendLvl()
        {
            connection?.SendU(new LevelPacket(skillslist.lvlsummary()));
        }
        public void SendOnline()
        {
            connection?.SendU(new OnlinePacket(connection.online, 0));
        }
        #endregion
        #region renders
        public void ReSendBots()
        {
            List<IHubPacket> packets = new();
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = (ChunkX + xxx);
                    var y = (ChunkY + yyy);
                    if (valid(x, y))
                    {
                        var ch = World.W.chunks[x, y];
                        foreach (var id in ch.bots)
                        {
                            var player = MServer.GetPlayer(id.Key);
                            if (player != null)
                            {
                                packets.Add(new HBBotPacket(player.Id, player.x, player.y, player.dir, 0, player.clanid, 0));
                            }
                        }
                    }
                }
            }
            connection?.SendB(new HBPacket(packets.ToArray()));
            lastPlayersend = DateTime.Now;
        }
        public void Update()
        {
            if (DateTime.Now - lastPlayersend > TimeSpan.FromSeconds(4))
            {
                ReSendBots();
            }
        }
        public void SendMyMove()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
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
                            foreach (var id in ch.bots)
                            {
                                MServer.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBBotPacket(Id, this.x, this.y, dir, 0, clanid, 0)]));
                            }
                        }
                    }
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
                List<IHubPacket> packetsmap = new();
                List<IHubPacket> packetspacks = new();
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
                                foreach (var p in ch.packs.Values)
                                {
                                    connection?.SendB(new HBPacket([new HBPacksPacket(p.x + p.y * World.W.height, [new HBPack((char)p.type, p.x, p.y, p.cid, p.off)])]));
                                }
                                cx *= 32; cy *= 32;
                                packetsmap.Add(new HBMapPacket(cx, cy, 32, 32, ch.pastedcells));
                                foreach (var id in ch.bots)
                                {
                                    var player = MServer.GetPlayer(id.Key);
                                    if (player != null)
                                    {
                                        packetsmap.Add(new HBBotPacket(player.Id, player.x, player.y, player.dir, 0, player.clanid, 0));
                                    }
                                }
                            }
                        }
                    }
                }
                connection?.SendB(new HBPacket(packetsmap.ToArray()));
                connection?.SendB(new HBPacket(packetspacks.ToArray()));
                lastPlayersend = DateTime.Now;
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
                              player?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(Id, this.x, this.y, fx, dir, col)]));
                        }
                    }
                }
            }
        }
        public void SendFXoBots(int fx, int fxx, int fxy)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (valid(ChunkX + xxx,ChunkY + yyy))
                    {
                        var x = ChunkX + xxx;
                        var y = ChunkY + yyy;
                        var ch = World.W.chunks[x, y];

                        foreach (var player in ch.bots.Select(id => MServer.GetPlayer(id.Key)))
                        {
                            player?.connection?.SendB(new HBPacket([new HBFXPacket(fxx, fxy, fx)]));
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
                                player?.connection?.SendLocalChat(msg);
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
                connection?.SendU(new GuPacket());
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

    }
}
