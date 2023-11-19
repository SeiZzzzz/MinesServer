using MinesServer.Server;
using MoreLinq.Extensions;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public Dictionary<int, Player> bots = new();
        public (int, int) pos;
        public byte[] cells = new byte[32 * 32];
        public bool active = false;
        public Chunk((int, int) pos) => this.pos = pos;
        public bool ContainsAlive = false;
        public void Update()
        {
            if (shouldbeloaded())
            {
                Load();
                SendAround();
                return;
            }
            cells = null;
        }
        private void UpdateCells()
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (World.GetProp(cells[x + y * 32]).isAlive)
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
        public void Load() => cells = cells == null ? World.W.map.LoadFrom((pos.Item1 * 32), (pos.Item2 * 32), 32, 32) : cells;
        private bool ShouldBeLoadedBots()
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
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
            if (cells == null)
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
                    if (World.W.chunks[chx, chy] != null && cells != null)
                    {
                        var c = World.W.GetCell(cellpos.Item1, cellpos.Item2);
                        if (World.GetProp(c).isAlive)
                        {
                            ContainsAlive = true;
                        }
                        if (cells != null && cells[x + y * 32] != c)
                        {
                            cells[x + y * 32] = c;
                            SendCellToBots(cellpos.Item1, cellpos.Item2, cells[x + y * 32]);
                        }
                    }
                }
            }
        }

        private void SendCellToBots(int x, int y, byte cell)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < World.W.chunksCountW && y < World.W.chunksCountH);
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
