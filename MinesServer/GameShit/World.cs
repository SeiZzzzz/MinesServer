using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Generator;
using MinesServer.Network.Constraints;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.World;
using MinesServer.Server;
using MoreLinq.Extensions;
using System.IO.Pipes;
using System.Numerics;

namespace MinesServer.GameShit
{
    public class World
    {
        public string name { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int chunksCountW { get { return width / 32; } }
        public int chunksCountH { get { return height / 32; } }
        public readonly Map map;
        public Chunk[,] chunks;
        public static World W;
        public Gen gen;
        public World(string name, int width, int height)
        {
            W = this;
            this.width = width;
            this.height = height;
            this.name = name;
            map = new Map(width, height);
            gen = new Gen(width, height);
            var x = DateTime.Now;
            chunks = new Chunk[chunksCountW, chunksCountH];
            CreateChunks();
            if (!map.MapExists)
            {
                EmptyChunks();
                Console.WriteLine($"Creating World Preset{width} x {height}({chunksCountW} x {chunksCountH} chunks)");
                Console.WriteLine("EmptyMapGeneration");
                x = DateTime.Now;
                //CreateEmptyMap(114);
                gen.StartGeneration();
                Console.WriteLine("Generation End");
                Console.WriteLine($"{DateTime.Now - x} loaded");
                x = DateTime.Now;
            }
            CreateSpawns(4);
            Console.WriteLine("Creating chunkmesh");
            x = DateTime.Now;
            Console.WriteLine($"{DateTime.Now - x} loaded");
            Console.WriteLine("LoadConfirmed");
            Console.WriteLine("Started");
            DataBase.Load();
            MServer.started = true;
        }
        public void CreateSpawns(int c)
        {
            var r = new Random();
            using (var db = new DataBase())
            {
                var y = 10;
                var x = 0;
                while (db.resps.Where(i => i.ownerid == 0).Count() < c)
                {
                    x = r.Next(x,width + 1);
                    if (x == width)
                    {
                        y++;
                        x = 0;
                    }
                    if (CanBuildPack(-10, 5, -10, 5, x, y, null, true))
                    {
                        for(int rx = -10;rx <= 10;rx++)
                        {
                            for (int ry = -10;ry <= 10;ry++)
                            {
                               SetCell(x + rx, y + ry, 36);
                            }
                        }
                        new Market(x - 7, y - 4, 0).Build();
                        new Resp(x - 8,y + 7, 0).Build();
                        new Up(x, y - 4, 0).Build();

                    }
                }
            }

        }
        public void CreateChunks()
        {
            for (int chx = 0; chx < chunksCountW; chx++)
            {
                for (int chy = 0; chy < chunksCountH; chy++)
                {
                    chunks[chx, chy] = new Chunk((chx, chy));
                }
            }
        }
        public void EmptyChunks()
        {
            for (int chx = 0; chx < chunksCountW; chx++)
            {
                for (int chy = 0; chy < chunksCountH; chy++)
                {
                    chunks[chx, chy].EmptyLoad();
                }
            }
        }
        public bool CanBuildPack(int left, int right, int bottom, int top, int x, int y, Player player, bool ignoreplace = false)
        {
            var h = 0;
            List<IHubPacket> packets = new();
            for (int cx = left; cx <= right; cx++)
            {
                for (int cy = bottom; cy <= top; cy++)
                {
                    var p = GetProp(GetCell(x + cx, y + cy));
                    if (!ValidCoord(x + cx, y + cy) || (ignoreplace && (!p.is_diggable || !p.is_destructible || GetCell(x + cx, y + cy) == 36)) || PackPart(x + cx, y + cy) || ((!p.can_place_over || !p.isEmpty) && !ignoreplace))
                    {
                        if (player != null && ValidCoord(x + cx,y + cy))
                        {
                            packets.Add(new HBFXPacket(x + cx, y + cy, 0));
                        }

                        h++;
                    }
                }
            }
            if (h > 0)
            {
                if (packets.Count > 0)
                {
                    player.connection.SendB(new HBPacket(packets.ToArray()));
                }
                return false;
            }
            return true;
        }
        public enum destroytype
        {
            Cell,
            Road,
            CellAndRoad
        }
        public static bool DamageCell(int x, int y, float dmg)
        {
            var d = GetDurability(x, y);
            if ((d - dmg) <= 0)
            {
                SetDurability(x, y, 0);
                Destroy(x, y);
                return true;
            }
            SetDurability(x, y, d - dmg);
            return false;
        }
        public static void Destroy(int x, int y, destroytype t = destroytype.Cell)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.DestroyCell(x - ch.WorldX, y - ch.WorldY, t);
        }
        public static float GetDurability(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return 0f;
            }
            var ch = W.GetChunk(x, y);
            return ch.GetDurability(x - ch.WorldX, y - ch.WorldY);
        }
        public static void SetDurability(int x, int y, float d)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.SetDurability(x - ch.WorldX, y - ch.WorldY, d);
        }
        public void CreateEmptyMap(byte cell)
        {
            int cells = 0;
            var j = DateTime.Now;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells += 1;
                    World.SetCell(x, y, cell);
                }
                if (DateTime.Now - j > TimeSpan.FromSeconds(2))
                {
                    Console.Write($"\r{cells}/{width * height}");
                    j = DateTime.Now;
                }
            }
            Console.Write($"\r{cells}/{width * height}");
            Console.WriteLine("");
        }
        public static Cell GetProp(byte type)
        {
            return CellsSerializer.cells[type];
        }
        public static bool IsEmpty(int x, int y)
        {
            return GetProp(x, y).isEmpty && !PackPart(x, y);
        }
        public static Cell GetProp(int x, int y)
        {
            return CellsSerializer.cells[GetCell(x, y)];
        }
        public static void MoveCell(int x, int y, int plusx, int plusy)
        {
            if (!W.ValidCoord(x + plusx, y + plusy))
            {
                return;
            }
            var cell = GetCell(x, y);
            var durability = GetDurability(x, y);
            Destroy(x, y, destroytype.Cell);
            SetCell(x + plusx, y + plusy, cell);
            SetDurability(x + plusx, y + plusy, durability);
        }
        public static void SetCell(int x, int y, byte cell, bool packmesh = false)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.SetCell(x - ch.WorldX, y - ch.WorldY, cell, packmesh);
        }
        public static bool PackPart(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return false;
            }
            var ch = W.GetChunk(x, y);
            ch.LoadPackProps();
            return ch.packsprop[(x - ch.WorldX) + (y - ch.WorldY) * 32];
        }
        public static void AddPack(int x, int y, Pack p)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.SetPack(x - ch.WorldX, y - ch.WorldY, p);
        }
        public static void RemovePack(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.RemovePack(x - ch.WorldX, y - ch.WorldY);
        }
        public static byte GetCell(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return 0;
            }
            var ch = W.GetChunk(x, y);
            return ch.GetCell(x - ch.WorldX, y - ch.WorldY);
        }
        public async void AsyncAction(int secdelay, Action act)
        {
            await Task.Run(delegate ()
            {
                System.Threading.Thread.Sleep(secdelay * 100);
                act();
            });
        }
        public Stack<Player> GetPlayersFromPos(int x, int y)
        {
            var st = new Stack<Player>();
            foreach (var id in GetChunk(x, y).bots.Keys)
            {
                var p = MServer.GetPlayer(id);
                if (p == null)
                {
                    continue;
                }
                if (p.x == x && p.y == y)
                {
                    st.Push(p);
                }

            }
            return st;
        }
        public static bool ContainsPack(int x, int y, out Pack p)
        {
            if (!W.ValidCoord(x,y))
            {
                p = null;
                return true;
            }
            var chpos = W.GetChunkPosByCoords(x, y);
            var ch = W.chunks[chpos.Item1, chpos.Item2];
            p = ch.GetPack((x - ch.WorldX), (y - ch.WorldY))!;
            if (p == null)
            {
                return false;
            }
            return true;
        }
        public static bool isAlive(byte cell)
        {
            return ((CellType)cell) switch
            {
                CellType.AliveBlue or CellType.AliveCyan or CellType.AliveRed or CellType.AliveNigger or CellType.AliveViol or CellType.AliveWhite or CellType.AliveRainbow => true,
                _ => false
            };
        }
        public static bool isCry(byte cell)
        {
            return ((CellType)cell) switch
            {
                CellType.XGreen or CellType.Green => true,
                CellType.XBlue or CellType.Blue => true,
                CellType.XRed or CellType.Red => true,
                CellType.XViolet or CellType.Violet => true,
                CellType.White => true,
                CellType.XCyan or CellType.Cyan => true,
                _ => false
            };
        }
        public bool ValidCoord(int x, int y) => (x >= 0 && y >= 0) && (x < width && y < height);
        private (int, int) GetChunkPosByCoords(int x, int y) => ((int)Math.Floor((float)x / 32), (int)Math.Floor((float)y / 32));
        public void UpdateChunkByCoords(int x, int y)
        {
            var ch = GetChunk(x, y);
            if (ch != null)
            {
                ch.Update();
            }
        }
        public static bool GunRadius(int x,int y,Player player)
        {
            for (int chx = -21; chx <= 21; chx++)
            {
                for (int chy = -21; chy <= 21; chy++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(x + chx, y + chy)) <= 20f)
                    {
                        if (World.W.ValidCoord(x + chx, y + chy) && (ContainsPack(x + chx, y + chy, out var p) && p is Gun && (p as Gun).charge > 0 && (p as Gun).cid != player.cid))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static DateTime lastpackupd = DateTime.Now;
        public static DateTime lastpackeffect = DateTime.Now;
        public static void Update()
        {
            if (DateTime.Now - lastpackupd >= TimeSpan.FromHours(1))
            {
                using var db = new DataBase();
                for (int chx = 0; chx < W.chunksCountW; chx++)
                {
                    for (int chy = 0; chy < W.chunksCountH; chy++)
                    {
                        foreach(var pack in W.chunks[chx, chy].packs)
                        {
                            if (pack.Value != null && pack.Value is IDamagable)
                            {
                                db.Attach(pack.Value);
                                var damagable = pack.Value as IDamagable;
                                damagable?.Damage(2);
                                if (damagable.NeedEffect())
                                {
                                    damagable.SendBrokenEffect();
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                lastpackupd = DateTime.Now;
            }
            
            if (DateTime.Now - lastpackeffect >= TimeSpan.FromSeconds(0.5))
            {
                using var db = new DataBase();
                for (int chx = 0; chx < W.chunksCountW; chx++)
                {
                    for (int chy = 0; chy < W.chunksCountH; chy++)
                    {
                        foreach (var pack in W.chunks[chx, chy].packs)
                        {
                            if (pack.Value != null && pack.Value is IDamagable)
                            {
                                var damagable = pack.Value as IDamagable;
                                if (damagable.NeedEffect())
                                {
                                    damagable.SendBrokenEffect();
                                }
                                if (pack.Value != null && pack.Value is Gun)
                                {
                                    var gun = pack.Value as Gun;
                                    db.Attach(gun);
                                    gun.Update();
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                lastpackeffect = DateTime.Now;
            }
            if (DateTime.Now - lastcryupdate >= TimeSpan.FromHours(1))
            {
                for (int i = 0; i < W.cryscostmod.Length; i++)
                {
                    var p = ((W.summary[i] + W.summary.Sum()) / 100);
                    if (p > 0)
                    {
                        if (p > 20 && (W.cryscostbase[i] + W.cryscostmod[i]) > W.cryscostbase[i])
                        {
                            W.cryscostmod[i] -= 1;
                        }
                        else if (p < 10 && (W.cryscostbase[i] + W.cryscostmod[i]) < 70)
                        {
                            W.cryscostmod[i] += 1;
                        }
                    }
                }
                W.summary = new long[6];
                lastcryupdate = DateTime.Now;
            }
        }
        public static DateTime lastcryupdate = DateTime.MinValue;
        public static int GetCrysCost(int i)
        {
            return W.cryscostbase[i] + W.cryscostmod[i];
        }
        public static void AddDob(int t, long dob)
        {
            W.summary[t] += dob;
        }
        public int[] cryscostmod = { 10, 10, 15, 10, 15, 15 };
        public int[] cryscostbase = { 8, 16, 24, 26, 24, 40 };
        public long[] summary = new long[6];
        public Chunk GetChunk(int x, int y)
        {
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
