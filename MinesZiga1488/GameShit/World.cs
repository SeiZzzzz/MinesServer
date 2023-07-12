namespace MinesServer.GameShit
{
    public class World
    {
        public string name { get; private set; }
        public Map map { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int chunksCountW { get { return width / 32; } }
        public int chunksCountH { get { return height / 32; } }
        public Chunk[,] chunks;
        public static World W;
        public World(string name, int width, int height)
        {
            W = this;
            this.width = width;
            this.height = height;
            this.name = name;
            map = new Map(width, height);
            Console.WriteLine($"Creating World Preset{width} x {height}({chunksCountW} x {chunksCountH} chunks)");
            chunks = new Chunk[chunksCountW, chunksCountH];
            Console.WriteLine("EmptyMapGeneration");
            var x = DateTime.Now;
            CreateEmptyMap();
            Console.WriteLine("");
            Console.WriteLine($"{(DateTime.Now - x).Seconds} s loading");
            x = DateTime.Now;
            Console.WriteLine("Creating chunkmesh");
            CreateChunks();
            Console.WriteLine($"{(DateTime.Now - x).Microseconds} ms loading");
            Console.WriteLine("LoadConfirmed");
        }
        public void CreateChunks()
        {
            for (int chx = 0; chx < chunksCountW; chx++)
            {
                for (int chy = 0; chy < chunksCountH; chy++)
                {
                    chunks[chx, chy] = new Chunk((chx, chx));
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            if (chunks[chx, chy] != null)
                            {
                                var cell = GetCell((chx * 32) + x, (chy * 32) + y);
                                chunks[chx, chy].cells[x + y * 32] = cell;
                            }
                        }
                    }
                }
            }
        }
        public void DestroyByBoom()
        {

        }
        public void DestroyCellByBz(int x,int y)
        {
            var cell = GetCell(x, y);
            if (cell != null && GetProp(cell).is_destructible && map.mapmesh[0, x + y * height] != 0)
            {
                map.mapmesh[1, x + y * height] = 0;
            }
            else if (cell != 0 && GetProp(cell).is_destructible)
            {
                map.mapmesh[0, x + y * height] = 32;
                map.mapmesh[1, x + y * height] = 0;
            }
        }
        public void CreateEmptyMap()
        {
            int cells = 0;
            var j = DateTime.Now;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells += 1;
                    SetCell(x, y, 35);
                }
                if (DateTime.Now - j > TimeSpan.FromSeconds(2))
                {
                    Console.Write($"\r{cells}/{width * height}");
                    j = DateTime.Now;
                }
            }
        }
        public static Cell GetProp(byte type)
        {
            return CellsSerializer.cells[type];
        }
        public void SetCell(int x, int y, byte cell)
        {
            if (CellsSerializer.cells[cell].isEmpty)
            {
                map.mapmesh[0, x + y * height] = cell;
            }
            else
            {
                map.mapmesh[1, x + y * height] = cell;
            }
            UpdateChunkByCoords(x, y);
        }
        public byte GetCell(int x, int y)
        {
            if (map.mapmesh[1, x + y * height] == 0)
            {
                return map.mapmesh[0, x + y * height];
            }
            return map.mapmesh[1, x + y * height];
        }
        public bool ValidCoord(int x, int y) => (x >= 0 && y >= 0) && (x < width && y < height);
        public void UpdateChunkByCoords(int x, int y)
        {
            var ch = GetChunk(x, y);
            if (ch != null)
            {
                ch.Update();
            }
        }
        private (int, int) GetChunkPosByCoords(int x, int y) => ((int)Math.Floor((decimal)x / 32), (int)Math.Floor((decimal)y / 32));
        public Chunk GetChunk(int x, int y)
        {
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
