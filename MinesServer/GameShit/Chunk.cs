using MinesServer.GameShit.Buildings;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public Dictionary<int, Player> bots = new();
        public (int, int) pos;
        private byte[] cells;
        public byte[] wcells;
        public byte[] rcells;
        public float[] durcells;
        public bool active = false;
        public Chunk((int, int) pos) => this.pos = pos;
        public bool ContainsAlive = false;
        public Dictionary<int, Pack> packs = new();
        public byte[] pastedcells
        {
            get => cells;
        }
        public byte this[int x, int y]
        {
            get => cells[x + y * 32];
            private set => cells[x + y * 32] = value;
        }
        public void Update()
        {
            if (shouldbeloaded())
            {
                Load();
                SendAround();
                return;
            }
        }
        public void SetCell(int x,int y,byte cell)
        {
            //dosomeshit
        }
        public void SendDirectedFx(int fx,int x,int y,int dir,int bid = 0,int color = 0)
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
                                
                                player.SendB(new HBPacket([new HBDirectedFXPacket(bid,x,y,fx,dir,color)]));
                            }
                        }
                    }
                }
            }
        }
        public void SendPack(char type,int x,int y,int cid,int off)
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
                                player.SendB(new HBPacket([new HBPacksPacket(x + y * World.W.height, [new HBPack(type,x,y,cid,off)])]));
                            }
                        }
                    }
                }
            }
        }
        public void ClearPack(int x,int y)
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
                                player.SendB(new HBPacket([new HBPacksPacket(x + y * World.W.height, [])]));
                            }
                        }
                    }
                }
            }
        }
        public void SendPacks()
        {

        }
        public Pack? GetPackAt(int x, int y) => packs.ContainsKey(x + y * 32) ? packs[x + y * 32] : null;
        public void SetPack(int x, int y, Pack p) => packs[x + y * 32] = p;
        private void UpdateCells()
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (World.isAlive(pastedcells[x + y * 32]))
                    {
                        //upd
                    }
                }
            }
        }
        public void AddBot(Player player)
        {
            if (this != null)
            {
                this.bots.Add(player.Id, player);
            }
        }
        public bool shouldbeloaded()
        {
            return active && (ShouldBeLoadedBots() || ContainsAlive);
        }
        public void Load()
        {
            wcells = new byte[1024];rcells = new byte[1024];durcells = new float[1024];cells = new byte[1024];
            World.W.map.LoadChunk(this);
            for(int x = 0;x < 32;x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (wcells[x + y * 32] == 0)
                    {
                        rcells[x + y * 32] = rcells[x + y * 32] == 0 ? (byte)32 : rcells[x + y * 32];
                        this[x,y] = rcells[x + y * 32];
                    }
                    else
                    {
                        durcells[x + y * 32] = World.GetProp(wcells[x + y * 32]).durability;
                    }
                    this[x, y] = wcells[x + y * 32];
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
        public void SendAround()
        {
            ContainsAlive = false;
            if (pastedcells == null)
            {
                return;
            }
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var chx = pos.Item1;
                    var chy = pos.Item2;
                    var cellpos = ((chx * 32) + x, (chy * 32) + y);
                    if (World.W.chunks[chx, chy] != null && pastedcells != null)
                    {
                        var c = World.GetCell(cellpos.Item1, cellpos.Item2);
                        if (World.isAlive(this[x,y]))
                        {
                            ContainsAlive = true;
                        }
                        if (pastedcells != null && this[x, y] != c)
                        {
                            this[x, y] = c;
                            SendCellToBots(cellpos.Item1, cellpos.Item2, pastedcells[x + y * 32]);
                        }
                    }
                }
            }
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
                            var player = MServer.GetPlayer(id.Key);
                            if (player != null)
                            {
                                player.SendCell(x, y, cell);
                            }
                        }
                    }
                }
            }
        }
    }
}
