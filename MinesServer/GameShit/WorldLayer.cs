using System.Runtime.InteropServices;
using static MinesServer.GameShit.World;

namespace MinesServer.GameShit
{
    public class WorldLayer<T>(string filename) : WorldLayerBase<T>(filename) where T : unmanaged
    {
        readonly int _typeSize = Marshal.SizeOf<T>();

        public override void ForceWrite(int x, int y, T value)
        {
            if (x < 0 || x >= CellsWidth || y < 0 || y >= CellsHeight) return;
            this[x, y] = value;
            var chunkIndex = GetChunkIndex(x / ChunkWidth, y / ChunkHeight);
            var data = Data(chunkIndex);
            var cellPos = x % ChunkWidth + y % ChunkHeight * ChunkHeight;
            data[cellPos] = value;
            lock (_stream)
            {
                Span<byte> temp = stackalloc byte[_typeSize];
                MemoryMarshal.Write(temp, in value);
                _stream.Position = chunkIndex * ChunkVolume + cellPos;
                _stream.Write(temp);
            }
        }

        protected override T[] ReadFromFile(int chunkIndex)
        {
            lock (_stream)
            {
                var chunk = new T[ChunkVolume];
                Span<byte> temp = stackalloc byte[ChunkVolume * _typeSize];
                _stream.Position = chunkIndex * temp.Length;
                _stream.Read(temp);
                for (int i = 0, j = 0; i < temp.Length; i += _typeSize, j++)
                    chunk[j] = MemoryMarshal.Read<T>(temp[i..(i + _typeSize)]);
                return chunk;
            }
        }

        protected override void WriteToFile(int chunkindex, T[] data)
        {
            lock (_stream)
            {
                Span<byte> temp = stackalloc byte[data.Length * _typeSize];
                for (int i = 0, j = 0; i < temp.Length; i += _typeSize, j++)
                    MemoryMarshal.Write(temp[i..(i + _typeSize)], in data[j]);
                _stream.Position = chunkindex * ChunkVolume * _typeSize;
                _stream.Write(temp);
            }
        }
    }
}
