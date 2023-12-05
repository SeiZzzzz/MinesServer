using MinesServer.GameShit.Buildings;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Collections.Concurrent;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public ConcurrentDictionary<int, Player> bots = new();
        public (int, int) pos;
        private byte[] cells;
        public byte[] wcells;
        public byte[] rcells;
        public float[] durcells;
        public bool[] packsprop;
        public bool active = false;
        public Chunk((int, int) pos) => this.pos = pos;
        public bool ContainsAlive = false;
        public Dictionary<int, Pack> packs = new();
        public byte[] pastedcells
        {
            get => cells;
        }
        private byte this[int x, int y]
        {
            get => cells[x + y * 32];
            set => cells[x + y * 32] = value;
        }
        public int WorldX
        {
            get => pos.Item1 * 32;
        }
        public int WorldY
        {
            get => pos.Item2 * 32;
        }
        private long lasttick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public void Update()
        {
            if (shouldbeloaded())
            {
                updlasttick = false;
                var currenttick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (currenttick - lasttick > 700)
                {
                    UpdateCells();
                    lasttick = currenttick;
                }
                Load();
                return;
            }
            Dispose();
        }
        public void SetCell(int x, int y, byte cell, bool packmesh = false)
        {
            Load();
            if (World.GetProp(cell).isEmpty)
            {
                wcells[x + y * 32] = 0;
                rcells[x + y * 32] = cell;
                this[x, y] = cell;
            }
            else
            {
                wcells[x + y * 32] = cell;
                durcells[x + y * 32] = World.GetProp(cell).durability;
                this[x, y] = cell;
            }
            LoadPackProps();
            packsprop[x + y * 32] = packmesh ? true : false;
            if (active)
            {
                SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
            }
            Save();
        }
        public byte GetCell(int x, int y)
        {
            if (cells == null)
            {
                Load();
            }
            return this[x, y];

        }
        public void LoadPackProps()
        {
            if (packsprop == null)
            {
                packsprop = new bool[1024];
                foreach (var p in packs.Values)
                {
                    p.Build();
                }
            }
        }
        public float GetDurability(int x, int y) => durcells[x + y * 32];
        public void SetDurability(int x, int y, float d) => durcells[x + y * 32] = d;
        public void DestroyCell(int x, int y, World.destroytype t)
        {
            switch (t)
            {
                case World.destroytype.Cell:
                    if (wcells[x + y * 32] != 0)
                    {
                        wcells[x + y * 32] = 0;
                        rcells[x + y * 32] = rcells[x + y * 32] == 0 ? (byte)32 : rcells[x + y * 32];
                        this[x, y] = rcells[x + y * 32];
                    }
                    break;
                case World.destroytype.Road:
                    if (rcells[x + y * 32] != 32)
                    {
                        rcells[x + y * 32] = 32;
                        this[x, y] = wcells[x + y * 32] == 0 ? rcells[x + y * 32] : wcells[x + y * 32];
                    }
                    break;
                case World.destroytype.CellAndRoad:
                    this[x, y] = 32;
                    wcells[x + y * 32] = 0;
                    rcells[x + y * 32] = 32;
                    break;
            }
            if (active)
            {
                SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
            }
            Save();
        }
        public void SendDirectedFx(int fx, int x, int y, int dir, int bid = 0, int color = 0)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            MServer.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(id.Key, x, y, fx, dir, color)]));
                        }
                    }
                }
            }
        }
        public void SendFx(int x, int y, int fx)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            MServer.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBFXPacket(x,y,fx)]));
                        }
                    }
                }
            }
        }
        public void ResendPacks()
        {
            foreach (var p in packs.Values)
            {
                SendPack((char)p.type, p.x, p.y, p.cid, p.off);
            }
        }
        public void SendPack(char type, int x, int y, int cid, int off)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            var player = MServer.GetPlayer(id.Key);
                            if (player != null)
                            {
                                MServer.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBPacksPacket(x + y * World.W.height, [new HBPack(type, x, y, cid, off)])]));
                            }
                        }
                    }
                }
            }
        }
        public void ClearPack(int x, int y)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            MServer.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBPacksPacket(x + y * World.W.height, [])]));
                        }
                    }
                }
            }
        }
        public Pack? GetPack(int x, int y) => packs.ContainsKey(x + y * 32) ? packs[x + y * 32] : null;
        public void SetPack(int x, int y, Pack p)
        {
            packs[x + y * 32] = p;
            SendPack((char)p.type, WorldX + x, WorldY + y, p.cid, p.off);
        }
        public void RemovePack(int x, int y)
        {
            if (packs.ContainsKey(x + y * 32))
            {
                packs.Remove(x + y * 32);
                ClearPack(WorldX + x, WorldY + y);
            }
        }
        private void UpdateCells()
        {
            if (wcells == null || rcells == null)
            {
                return;
            }
            List<(int, int, byte)> cellstoupd = new();
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    this[x, y] = wcells[x + y * 32] == 0 ? rcells[x + y * 32] : wcells[x + y * 32];
                    var prop = World.GetProp(this[x, y]);
                    if (prop.isSand || prop.isBoulder || World.isAlive(this[x, y]))
                    {
                        cellstoupd.Add((WorldX + x, WorldY + y, this[x, y]));
                    }
                }
            }
            foreach (var c in cellstoupd)
            {
                if (World.isAlive(c.Item3) && Physics.Alive(c.Item1, c.Item2))
                {
                    updlasttick = true;
                }
                else if (World.GetProp(c.Item3).isSand && Physics.Sand(c.Item1, c.Item2))
                {
                    updlasttick = true;
                }
                else if (World.GetProp(c.Item3).isBoulder && Physics.Boulder(c.Item1, c.Item2))
                {
                    updlasttick = true;
                }
            }
        }
        private bool updlasttick = false;
        public void AddBot(Player player)
        {
            if (this != null && !bots.ContainsKey(player.Id))
            {
                bots[player.Id] = player;
            }
        }
        public bool shouldbeloaded()
        {
            return active && (ShouldBeLoadedBots() || ContainsAlive || updlasttick);
        }
        public void EmptyLoad()
        {
            cells = new byte[1024];
            wcells = new byte[1024];
            rcells = new byte[1024];
            durcells = new float[1024];
        }
        public void Dispose()
        {
            cells = null;
            wcells = null;
            durcells = null;
            rcells = null;

        }
        public void Load()
        {
            if (cells != null && wcells != null)
            {
                return;
            }
            LoadPackProps();
            wcells = new byte[1024]; rcells = new byte[1024]; durcells = new float[1024]; cells = new byte[1024];
            World.W.map.LoadChunk(this);
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (wcells[x + y * 32] == 0)
                    {
                        rcells[x + y * 32] = rcells[x + y * 32] == 0 ? (byte)32 : rcells[x + y * 32];
                        this[x, y] = rcells[x + y * 32];
                    }
                    else
                    {
                        this[x, y] = wcells[x + y * 32];
                        durcells[x + y * 32] = World.GetProp(wcells[x + y * 32]).durability;
                    }
                }
            }
        }
        public void Save() => World.W.map.SaveChunk(this);
        private bool ShouldBeLoadedBots()
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        if (ch.bots.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool valid(int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
        private void SendCellToBots(int x, int y, byte cell)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = (pos.Item1 + xxx);
                    var cy = (pos.Item2 + yyy);
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            MServer.GetPlayer(id.Key)?.connection?.SendCell(x, y, cell);
                        }
                    }
                }
            }
        }
    }
}
