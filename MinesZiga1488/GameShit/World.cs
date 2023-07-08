using Microsoft.EntityFrameworkCore.Update.Internal;
using MinesServer.Server;
using NetCoreServer;

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
            CreateEmptyMap();
            chunks = new Chunk[chunksCountW, chunksCountH];
        }
        public void CreateChunks()
        {
            for (int chx = 0; chx < chunksCountW; chx++)
            {
                for (int chy = 0; chy < chunksCountH; chy++)
                {
                    chunks[chx, chy] = new Chunk((chx,chx));
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
        public void CreateEmptyMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.mapmesh[x + y * height] = new Cell(x, y, 35);
                }
            }
        }
        public void SetCell(int x,int y, Cell cell)
        {
            map.mapmesh[x + y * height] = cell;
            UpdateChunkByCoords(x, y);
        }
        public Cell GetCell(int x,int y)
        {
            return map.mapmesh[x + y * height];
        }
        public void UpdateChunkByCoords(int x, int y) => GetChunk(x, y).Update();
        private (int, int) GetChunkPosByCoords(int x, int y) => ((int)Math.Floor((decimal)x / 32), (int)Math.Floor((decimal)y / 32));
        public Chunk GetChunk(int x, int y)
        {
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
