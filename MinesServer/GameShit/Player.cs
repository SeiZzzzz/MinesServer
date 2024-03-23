using MinesServer.Enums;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Skills;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Numerics;

namespace MinesServer.GameShit
{
    public class Player
    {
        #region forprogs
        PData _pdata;
        public PData programsData
        {
            get
            {
                _pdata ??= new PData(this);
                return _pdata;
            }
        }
        public void RunProgramm(Program p = null)
        {
            if (p == null)
            {
                programsData.Run();
                return;
            }
            
            programsData.Run(p);
        }
        #endregion
        #region fields
        [NotMapped]
        public Session? connection { get; set; }
        [NotMapped]
        public bool online
        {
            get => connection != null;
        }
        public Player() => Delay = DateTime.Now;
        public DateTime lastPlayersend = DateTime.Now;
        public DateTime lastPacks = DateTime.Now;
        public DateTime afkstarttime = DateTime.Now;
        public Queue<Action> playerActions = new();
        public int Id { get; set; }
        public string name { get; set; }
        public Clan? clan { get; set; }
        public Rank? clanrank { get; set; }
        public int pause = 3500;
        public List<Program> programs { get; set; }
        [NotMapped]
        public int cid { get => clan == null ? 0 : clan.id; }
        public Resp resp { get; set; }
        public long money { get; set; }
        public long creds { get; set; }
        public string hash { get; set; }
        public string passwd { get; set; }
        [NotMapped]
        public int tail { get => programsData.ProgRunning ? 1 : 0; }
        public int skin
        {
            get
            {
                if (online)
                    return _skin;
                return 1;
            }
            set => _skin = value;
        }
        private int _skin;
        public bool autoDig { get; set; }
        public Vector2 pos = Vector2.Zero;
        public int c190stacks = 1;
        public DateTime lastc190hit = DateTime.Now;
        public Basket crys { get; set; }
        public Inventory inventory { get; set; }
        public Health health { get; set; }
        public Settings settings { get; set; }
        public PlayerSkills skillslist { get; set; }
        public Stack<byte> geo = new Stack<byte>();
        public Queue<Line> console = new Queue<Line>();
        private bool actionpertick = true;
        [NotMapped]
        public Window? win;
        [NotMapped]
        private float cb;
        public DateTime Delay = DateTime.Now;
        public bool CanAct { get => !(Delay > DateTime.Now); }
        public bool OnRoad { get => World.isRoad(World.GetCell(x, y)); }
        public int dir { 
            get;
            set;
        }
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
        public double Pause
        {
            get => World.GetCell(x, y) == 35 ? 2 : 2;
        }
        #endregion
        #region actions
        public void Update()
        {
            actionpertick = false;
            if (DateTime.Now - lastc190hit >= TimeSpan.FromMinutes(1))
            {
                c190stacks = 1;
                lastc190hit = DateTime.Now;
            }
            if (!online)
            {
                if (DateTime.Now - afkstarttime > TimeSpan.FromSeconds(5))
                {
                    DataBase.activeplayers.Remove(this);
                    health.Death();
                }
                return;
            }
            if (DateTime.Now - lastPlayersend > TimeSpan.FromSeconds(4))
            {
                ReSendBots();
            }
            var cell = World.GetCell(x, y);
            var cellprop = World.GetProp(cell);
            if (!cellprop.isEmpty)
            {
                health.Hurt(cellprop.fall_damage);
                if (cell == 90)
                {
                    GetBox(x, y);
                    World.DamageCell(x, y, 1);
                }
                else if (cellprop.is_destructible)
                {
                    World.Destroy(x, y);
                }
            }
            if (programsData.ProgRunning)
            {
                programsData.Step();
                return;
            }
            while (playerActions.Count > 0)
            {
                playerActions.Dequeue()();
            }
        }
        public void SetResp(Resp r)
        {
            resp = r;
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
            if (!World.W.ValidCoord(x, y) || World.GunRadius(x, y, this))
            {
                return;
            }
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).isPickable && !World.GetProp(cell).isEmpty)
            {
                geo.Push(cell);
                World.Destroy(x, y);
            }
            else if (World.GetProp(cell).isEmpty && World.GetProp(cell).can_place_over && geo.Count > 0 && !World.PackPart(x, y))
            {
                var cplaceable = geo.Pop();
                World.SetCell(x, y, cplaceable);
                World.SetDurability(x, y, World.isCry(cplaceable) ? 0 : Physics.r.Next(1, 101) > 99 ? 0 : World.GetProp(cplaceable).durability);
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
            connection?.CloseWindow();

        }
        private void Mine(byte cell, int x, int y)
        {
            float dob = 1 + (float)Math.Truncate(cb);
            foreach (var c in skillslist.skills.Values)
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
            cb += dob - odob;
            crys.AddCrys(type, odob);
            World.AddDob(type, odob);
            SendDFToBots(2, x, y, this.Id, (int)(odob < 255 ? odob : 255), (type == 1 ? 3 : type == 2 ? 1 : type == 3 ? 2 : type));
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
            connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "+ " + b.AllCrys)]));
        }
        private void OnDestroy(byte type)
        {
            foreach (var c in skillslist.skills.Values)
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
            if (!World.W.ValidCoord(x, y))
            {
                return;
            }
            SendDFToBots(0, this.x, this.y, this.Id, dir);
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
                foreach (var c in skillslist.skills.Values)
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
            if (World.GetProp(cell).isBoulder)
            {
                var plusy = dir == 2 ? -1 : dir == 0 ? 1 : 0;
                var plusx = dir == 3 ? 1 : dir == 1 ? -1 : 0;
                if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
                {
                    World.MoveCell(x, y, plusx, plusy);
                    foreach (var c in skillslist.skills.Values)
                    {
                        if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                        {
                            c.AddExp(this);
                        }
                    }
                }
            }
        }
        public void AddAciton(Action a, double delay)
        {
            if (CanAct && !actionpertick)
            {
                Delay = DateTime.Now + TimeSpan.FromMicroseconds(delay);
                playerActions.Enqueue(a);
                actionpertick = true;
            }
        }
        public bool Move(int x, int y, int dir = -1)
        {

            if (!World.W.ValidCoord(x, y) || win != null)
            {
                tp(this.x, this.y);
                return false;
            }
            this.dir = dir;
            if (dir == -1)
            {
                this.dir = pos.X > x ? 1 : pos.X < x ? 3 : pos.Y > y ? 2 : 0;
            }
            var cell = World.GetCell(x, y);
            if (!World.GetProp(cell).isEmpty)
            {
                if (dir == -1)
                {
                    if (autoDig)
                    {
                        Bz();
                    }
                    tp(this.x, this.y);
                    return true;
                }
                tp(this.x, this.y);
                return false;
            }
            var newpos = new Vector2(x, y);
            if (Vector2.Distance(pos, newpos) < 1.2f)
            {
                foreach (var c in skillslist.skills.Values)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnMove, this))
                    {
                        if (c.type == SkillType.Movement)
                        {
                            c.AddExp(this);
                        }
                    }
                }
                pos = newpos;
            }
            else
            {
                tp(this.x, this.y);
                return false;
            }
            SendMyMove();
            SendMap();
            if (World.ContainsPack(x, y, out var pack) && (pack.cid == cid || pack.cid == 0) && !programsData.ProgRunning)
            {
                win = pack.GUIWin(this)!;
                SendWindow();
            }
            return false;
        }
        public void Build(string type)
        {
            int x = (int)GetDirCord().X, y = (int)GetDirCord().Y;
            if (!World.W.ValidCoord(x, y) || World.GunRadius(x, y, this) || World.PackPart(x, y))
            {
                return;
            }
            var buildskills = skillslist.skills.Values.Where(c => c.effecttype == SkillEffectType.OnBld);
            switch (type)
            {
                case "G":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildGreen && World.GetProp(x, y).isEmpty)
                        {
                            c.AddExp(this);
                            if (crys.RemoveCrys(0, (long)c.GetEffect()))
                            {
                                World.SetCell(x, y, 101);
                            }
                        }
                    }
                    break;
                case "V":

                    break;
                case "R":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildRoad)
                        {
                            c.AddExp(this);
                            if (crys.RemoveCrys(0, (long)c.GetEffect()) && World.GetProp(x, y).isEmpty)
                            {
                                World.SetCell(x, y, 35);
                            }
                        }
                    }
                    break;
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
            settings = new Settings(true);
            crys = new Basket(this);
            skillslist = new PlayerSkills();
            AddBasicSkills();
            health.LoadHealth(this);
            pos = new Vector2(0, 0);
            dir = 0;
            clan = null;
            skin = 0;
            RandomResp();
        }
        public void RandomResp()
        {
            using var db = new DataBase();
            var re = db.resps.Where(i => i.ownerid == 0);
            var rp = Physics.r.Next(0, re.Count());
            var resp = re.ElementAt(rp);
            var pos = resp.GetRandompoint();
            this.pos = new Vector2(pos.Item1, pos.Item2);
            SetResp(resp);
            db.SaveChanges();
        }
        public Resp? GetCurrentResp()
        {
            using var db = new DataBase();
            World.ContainsPack(resp.x, resp.y, out var p);
            return p as Resp;
        }
        private void AddBasicSkills()
        {
            //базовые скиллы
            skillslist.InstallSkill(SkillType.MineGeneral.GetCode(), 0, this);
            skillslist.InstallSkill(SkillType.Digging.GetCode(), 1, this);
            skillslist.InstallSkill(SkillType.Movement.GetCode(), 2, this);
        }
        public void Init()
        {
            if (DataBase.activeplayers.FirstOrDefault(p => p.Id == Id) == default)
            {
                DataBase.activeplayers.Add(this);
            }
            connection.auth = null;
            crys.player = this;
            skillslist.LoadSkills();
            health.LoadHealth(this);
            connection?.SendPing();
            connection?.SendWorldInfo();
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
            settings.SendSettings(this);
            OnLoad();
        }
        #endregion
        #region senders
        private void OnLoad()
        {
            SendClan();
            SendMap();
            foreach (var p in World.W.GetChunk(ChunkX, ChunkY).packs.Values)
                connection?.SendB(new HBPacket([new HBPacksPacket(x + y * World.CellsHeight, [new HBPack((char)p.type, p.x, p.y, (byte)p.cid, (byte)p.off)])]));
        }
        public void Beep() => connection?.SendU(new BibikaPacket());

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
        public void OpenClan()
        {
            if (clan != null)
            {
                using var db = new DataBase();
                db.clans.Where(i => i.id == clan.id).Include(p => p.members).Include(p => p.reqs).FirstOrDefault()?.OpenClanWin(this);
            }
        }
        public void SendAutoDigg()
        {
            connection?.SendU(new AutoDiggPacket(autoDig));
        }
        public void HurtEffect(int x, int y)
        {
            SendFXoBots(0, x, y);
        }
        public void tp(int x, int y)
        {
            connection?.SendU(new TPPacket(x, y));
            SendMyMove();
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
        public void SendClan()
        {
            if (cid == 0)
                connection?.SendU(new ClanHidePacket());
            else
                connection?.SendU(new ClanShowPacket(cid));

        }
        public void SendCrys()
        {
            crys.SendBasket();
        }
        public void SendSpeed()
        {
            connection?.SendU(new SpeedPacket(pause / 100, (int)(pause / 100 * 0.6), 100000));
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
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
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
                            var player = DataBase.GetPlayer(id.Key);
                            if (player != null)
                            {
                                packets.Add(new HBBotPacket(player.Id, player.x, player.y, player.dir, player.skin, player.cid, player.tail));
                            }
                        }
                    }
                }
            }
            connection?.SendB(new HBPacket(packets.ToArray()));
            lastPlayersend = DateTime.Now;
        }
        public void ReSendPacks()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var x = (ChunkX + xxx);
                    var y = (ChunkY + yyy);
                    if (valid(x, y))
                    {
                        var ch = World.W.chunks[x, y];
                        foreach (var p in ch.packs.Values)
                        {
                            connection?.SendB(new HBPacket([new HBPacksPacket((p.x / 32) + (p.y / 32) * World.ChunksH, [new HBPack((char)p.type, p.x, p.y, (byte)p.cid, (byte)p.off)])]));
                        }
                    }
                }
            }
            lastPlayersend = DateTime.Now;
        }
        public void SendMyMove()
        {
            if (connection == null)
            {
                return;
            }
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
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
                        if (ch != null)
                        {

                            cx *= 32; cy *= 32;
                            foreach (var id in ch.bots)
                            {
                                DataBase.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBBotPacket(Id, this.x, this.y, dir, skin, cid, tail)]));
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
                    chtoremove.bots.Remove(Id, out var p);
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
        public void SendMap(bool force = false)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
            if (!valid(ChunkX, ChunkY))
            {
                return;
            }
            if (lastchunk != (ChunkX, ChunkY) || force)
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
                            List<HBPack> packs = new();
                            if (ch != null)
                            {
                                cx *= 32; cy *= 32;
                                packetsmap.Add(new HBMapPacket(cx, cy, 32, 32, ch.cells));
                                foreach (var p in ch.packs.Values)
                                {
                                    packs.Add(new HBPack((char)p.type, p.x, p.y, (byte)p.cid, (byte)p.off));
                                }
                                connection?.SendB(new HBPacket([new HBPacksPacket(ch.pos.Item1 + ch.pos.Item2 * World.ChunksH, packs.ToArray())]));
                                foreach (var id in ch.bots)
                                {
                                    var player = DataBase.GetPlayer(id.Key);
                                    if (player != null)
                                    {
                                        packetsmap.Add(new HBBotPacket(player.Id, player.x, player.y, player.dir, player.skin, player.cid, player.tail));
                                    }
                                }
                            }
                        }
                    }
                }
                connection?.SendB(new HBPacket(packetsmap.ToArray()));
                lastPlayersend = DateTime.Now;
            }
        }
        public void SendDFToBots(int fx, int fxx, int fxy, int bid, int dir, int col = 0)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (valid(this.ChunkX + xxx, this.ChunkY + yyy))
                    {
                        var x = (this.ChunkX + xxx);
                        var y = (this.ChunkY + yyy);
                        var ch = World.W.chunks[x, y];

                        foreach (var player in ch.bots.Select(id => DataBase.GetPlayer(id.Key)))
                        {
                            player?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(Id, fxx, fxy, fx, dir, col)]));
                        }
                    }
                }
            }
        }
        public void SendFXoBots(int fx, int fxx, int fxy)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (valid(ChunkX + xxx, ChunkY + yyy))
                    {
                        var x = ChunkX + xxx;
                        var y = ChunkY + yyy;
                        var ch = World.W.chunks[x, y];

                        foreach (var player in ch.bots.Select(id => DataBase.GetPlayer(id.Key)))
                        {
                            player?.connection?.SendB(new HBPacket([new HBFXPacket(fxx, fxy, fx)]));
                        }
                    }
                }
            }
        }
        public void SendLocalMsg(string msg)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.ChunksW && y < World.ChunksH);
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
                            var player = DataBase.GetPlayer(id.Key);
                            if (player != null)
                            {
                                player?.connection?.SendB(new HBPacket([new HBChatPacket(Id, x, y, msg)]));
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
                    chtoremove.bots.Remove(this.Id, out var p);
                }
            }
        }

    }
}
