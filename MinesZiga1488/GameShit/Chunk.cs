using MinesServer.Server;

namespace MinesServer.GameShit
{
    public class Chunk
    {
        public Dictionary<int, Player> bots = new Dictionary<int, Player>();
        public (int, int) pos;
        public Cell[] cells = new Cell[32 * 32];
        public Chunk((int, int) pos) => this.pos = pos;
        public void AddBot(Player player)
        {
            if (this != null)
            {
                this.bots.Add(player.Id, player);
            }
        }
        public byte[] getCells()
        {
            var l = new List<byte>();
            foreach (var c in cells)
            {
                l.Add(c.type);
            }
            return l.ToArray();
        }
        public void Update()
        {
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var chx = pos.Item1;
                    var chy = pos.Item2;
                    if (World.W.chunks[chx, chy] != null)
                    {
                        var cell = World.W.GetCell((int)((chx * 32) + x), (int)(((chy * 32) + y)));
                        if (cells[x + y * 32] != cell)
                        {
                            cells[x + y * 32] = cell;
                            SendCellToBots(((chx * 32) + x), ((chy * 32) + y), cell);
                        }
                    }
                }
            }
        }
        public void SendCellToBots(int x, int y, Cell cell)
        {
            var valid = bool (int x, int y) => (x >= 0 && y >= 0) && (x < MServer.Instance.wrld.chunksCountW && y < MServer.Instance.wrld.chunksCountH);
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
                            var player = MServer.Instance.players[id.Key];
                            player.SendCell(x, y, cell.type);
                        }
                    }
                }
            }
        }
    }
}
