using MinesServer.Server;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public Dictionary<int, Player> bots = new Dictionary<int, Player>();
        public (int, int) pos;
        public byte[] cells = new byte[32 * 32];
        public Chunk((int, int) pos) => this.pos = pos;
        public void AddBot(Player player)
        {
            if (this != null)
            {
                this.bots.Add(player.Id, player);
            }
        }
        public void Load()
        {
            cells = World.W.map.LoadFrom((pos.Item1 * 32), (pos.Item2 * 32), 32, 32);
        }
        public void SendAround()
        {
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var chx = pos.Item1;
                    var chy = pos.Item2;
                    var cellpos = ((chx * 32) + x, (chy * 32) + y);
                    if (World.W.chunks[chx, chy] != null)
                    {
                        SendCellToBots(cellpos.Item1, cellpos.Item2, cells[x + y * 32]);
                    }
                }
            }
            cells = null;
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
