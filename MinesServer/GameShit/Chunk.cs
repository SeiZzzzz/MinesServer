using MinesServer.GameShit.Buildings;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Security.Cryptography;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public Dictionary<int, Player> bots = new();
        public (int, int) pos;
        private byte[] cells = new byte[32 * 32];
        public byte[] wcells = new byte[32 * 32];
        public byte[] rcells = new byte[32 * 32];
        public float[] durcells = new float[32 * 32];
        public bool active = false;
        public Chunk((int, int) pos) => this.pos = pos;
        public bool ContainsAlive = false;
        public Dictionary<int, Pack> packs = new();
        public byte[] pastedcells
        {
            get => cells;
        }
        public void Update()
        {
            if (shouldbeloaded())
            {
                LoadN();
                //SendAround();
                return;
            }
        }
        public byte GetCell(int x,int y)
        {
            //LoadChunkMB()
            return pastedcells[x + y * 32];
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
        public void LoadN() => World.W.map.LoadChunk(this);
        public void SaveN() => World.W.map.SaveChunk(this);
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
            if (wcells == null)
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
                    if (World.W.chunks[chx, chy] != null && wcells != null)
                    {
                        var c = World.GetCell(cellpos.Item1, cellpos.Item2);
                        if (World.isAlive(wcells[x + y * 32]))
                        {
                            ContainsAlive = true;
                        }
                        if (wcells != null && wcells[x + y * 32] != c)
                        {
                            wcells[x + y * 32] = c;
                            SendCellToBots(cellpos.Item1, cellpos.Item2, wcells[x + y * 32]);
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
