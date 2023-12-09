using static MinesServer.GameShit.World;

namespace MinesServer.GameShit
{
    /// <summary>
    /// A slightly more efficient variation of WorldLayer<byte>
    /// </summary>
    public class ByteWorldLayer(string filename) : WorldLayerBase<byte>(filename)
    {
        public override void ForceWrite(int x, int y, byte value)
        {
            if (x < 0 || x >= CellsWidth || y < 0 || y >= CellsHeight) return;
            this[x, y] = value;
            var chunkIndex = GetChunkIndex(x / ChunkWidth, y / ChunkHeight);
            var data = Data(chunkIndex);
            var cellPos = x % ChunkWidth + y % ChunkHeight * ChunkHeight;
            data[cellPos] = value;
            lock (Stream)
            {
                Stream.Position = chunkIndex * ChunkVolume + cellPos;
                Stream.Write([value], 0, sizeof(byte));
            }
        }

        protected override byte[] ReadFromFile(int chunkIndex)
        {
            lock (Stream)
            {
                var temp = new byte[ChunkVolume];
                Stream.Position = chunkIndex * temp.Length;
                Stream.Read(temp, 0, ChunkVolume);
                return temp;
            }
        }

        protected override void WriteToFile(int chunkindex, byte[] data)
        {
            lock (Stream)
            {
                Stream.Position = chunkindex * ChunkVolume;
                Stream.Write(data);
            }
        }
    }
}
