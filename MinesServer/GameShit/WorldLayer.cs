using System.Runtime.InteropServices;
using static MinesServer.GameShit.World;

namespace MinesServer.GameShit
{
    public class WorldLayer<T>(string filename) : WorldLayerBase<T>(filename) where T : unmanaged
    {
        public override void ForceWrite(int x, int y, T value)
        {
            if (x < 0 || x >= CellsWidth || y < 0 || y >= CellsHeight) return;
            this[x, y] = value;
            var chunkIndex = GetChunkIndex(x / ChunkWidth, y / ChunkHeight);
            var data = Data(chunkIndex);
            var cellPos = x % ChunkWidth + y % ChunkHeight * ChunkHeight;
            data[cellPos] = value;
            lock (Stream)
            {
                var typeSize = Marshal.SizeOf<T>();
                Span<byte> temp = stackalloc byte[typeSize];
                MemoryMarshal.Write(temp, in value);
                Stream.Position = chunkIndex * ChunkVolume + cellPos;
                Stream.Write(temp);
            }
        }

        protected override T[] ReadFromFile(int chunkIndex)
        {
            lock (Stream)
            {
                var chunk = new T[ChunkVolume];
                var typeSize = Marshal.SizeOf<T>();
                Span<byte> temp = stackalloc byte[ChunkVolume * typeSize];
                Stream.Position = chunkIndex * temp.Length;
                Stream.Read(temp);
                for (int i = 0, j = 0; i < temp.Length; i += typeSize, j++)
                    chunk[j] = MemoryMarshal.Read<T>(temp[i..(i + typeSize)]);
                return chunk;
            }
        }

        protected override void WriteToFile(int chunkindex, T[] data)
        {
            lock (Stream)
            {
                var typeSize = Marshal.SizeOf<T>();
                Span<byte> temp = stackalloc byte[data.Length * typeSize];
                for (int i = 0, j = 0; i < temp.Length; i += typeSize, j++)
                    MemoryMarshal.Write(temp[i..(i + typeSize)], in data[j]);
                Stream.Position = chunkindex * ChunkVolume;
                Stream.Write(temp);
            }
        }
    }
}
