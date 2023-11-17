using MinesServer.GameShit.Generator;

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
            if (!map.MapExists)
            {
                Console.WriteLine($"Creating World Preset{width} x {height}({chunksCountW} x {chunksCountH} chunks)");
                chunks = new Chunk[chunksCountW, chunksCountH];
                Console.WriteLine("EmptyMapGeneration");
                x = DateTime.Now;
                //CreateEmptyMap(114);
                gen.StartGeneration();
                Console.WriteLine("Generation End");
                Console.WriteLine($"{DateTime.Now - x} loaded");
                x = DateTime.Now;
            }
            chunks = new Chunk[chunksCountW, chunksCountH];
            Console.WriteLine("Creating chunkmesh");
            x = DateTime.Now;
            CreateChunks();
            Console.WriteLine($"{DateTime.Now - x} loaded");
            Console.WriteLine("LoadConfirmed");
            Console.WriteLine("Started");
            gen.GenerateSpawn(4);
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
        public void DestroyByBoom()
        {

        }
        public void Destroy(int x,int y)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
                var c = map.GetRoad(x, y);
                if (c != 0)
                {
                    map.SetCell(x, y, c);
                }
                else
                {
                    map.SetCell(x, y, 32);
                }
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
                    SetCell(x, y, cell);
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
        public void SetCell(int x, int y, byte cell)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            map.SetCell(x, y, cell);
            UpdateChunkByCoords(x, y);
        }
        public byte GetCell(int x, int y)
        {
            if (!ValidCoord(x, y))
            {
                return 0;
            }
            return map.GetCell(x, y);
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
        public Chunk GetChunk(int x, int y)
        {
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
