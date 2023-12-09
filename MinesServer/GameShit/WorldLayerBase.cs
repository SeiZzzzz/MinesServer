using static MinesServer.GameShit.World;

namespace MinesServer.GameShit
{
    public abstract class WorldLayerBase<T>(string filename) where T : unmanaged
    {
        protected readonly T[]?[] _data = new T[ChunksW * ChunksH][];
        protected readonly T[]?[] _buffer = new T[ChunksW * ChunksH][];

        protected readonly HashSet<int> _updatedChunks = [];

        private FileStream? _stream;
        protected FileStream Stream => _stream ??= new(filename, FileMode.OpenOrCreate);

        /// <summary>
        /// Currently loaded chunks, indecies
        /// </summary>
        public IEnumerable<int> LoadedChunks => Enumerable.Range(0, _data.Length).Where(x => _data[x] is not null);

        /// <summary>
        /// The chunks that were updated since last <see cref='Commit()'/> call, indecies
        /// </summary>
        public IEnumerable<int> UpdatedChunks => _updatedChunks;

        public T? this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= CellsWidth || y < 0 || y >= CellsHeight) return null;
                var data = Data(x / ChunkWidth, y / ChunkHeight);
                return data[x % ChunkWidth + y % ChunkHeight * ChunkHeight];
            }
            set
            {
                if (x < 0 || x >= CellsWidth || y < 0 || y >= CellsHeight) return;
                var chunkIndex = GetChunkIndex(x / ChunkWidth, y / ChunkHeight);
                var buffer = Buffer(chunkIndex);
                buffer[x % ChunkWidth + y % ChunkHeight * ChunkHeight] = value!.Value;
                _updatedChunks.Add(chunkIndex);
            }
        }

        /// <summary>
        /// Forces a write to the data array, also forcibly write the value to the disk.
        /// </summary>
        /// <remarks>
        /// Do NOT use this method for bulk changes! To update multiple values in a chunk consider writing them using <see cref='this[int, int]'/> and then calling <see cref='Commit()'/> instead.
        /// </remarks>
        /// <param name="x">X coordinate of the value</param>
        /// <param name="y">Y coordinate of the value</param>
        /// <param name="value">The new value</param>
        public abstract void ForceWrite(int x, int y, T value);

        /// <summary>
        /// Commits all changes to the main array and writed changes to the disk.
        /// </summary>
        public void Commit()
        {
            foreach (var index in _updatedChunks)
                if (_buffer[index] is not null)
                {
                    var chunk = _buffer[index]!;
                    if (_data[index] is null)
                    {
                        _data[index] = chunk;
                        continue;
                    }
                    chunk.CopyTo(_data[index]!, 0);
                    WriteToFile(index, chunk);
                }
            _updatedChunks.Clear();
        }

        /// <summary>
        /// Unloads a chunk
        /// </summary>
        /// <param name="chunkx">X coordinate of the chunk</param>
        /// <param name="chunky">Y coordinate of the chunk</param>
        public void Unload(int chunkx, int chunky) => Unload(GetChunkIndex(chunkx, chunky));

        /// <summary>
        /// Unloads a chunk
        /// </summary>
        /// <param name="chunkIndex">The index of the chunk</param>
        public void Unload(int chunkIndex)
        {
            if (chunkIndex < 0 || chunkIndex >= ChunksAmount) return;
            _buffer[chunkIndex] = _data[chunkIndex] = null;
        }

        public bool Exists => File.Exists(filename);

        public ReadOnlyMemory<T> ReadOnly_Data(int chunkx, int chunky) => ReadOnly_Data(GetChunkIndex(chunkx, chunky));

        public ReadOnlyMemory<T> ReadOnly_Data(int chunkIndex) => Data(chunkIndex);

        protected T[] Data(int chunkx, int chunky) => Data(GetChunkIndex(chunkx, chunky));

        protected T[] Data(int chunkIndex) => _data[chunkIndex] ??= ReadFromFile(chunkIndex);

        protected T[] Buffer(int chunkx, int chunky) => Buffer(GetChunkIndex(chunkx, chunky));

        protected T[] Buffer(int chunkIndex) => _buffer[chunkIndex] ??= Data(chunkIndex);

        protected T[] ReadFromFile(int chunkx, int chunky) => ReadFromFile(GetChunkIndex(chunkx, chunky));

        protected abstract T[] ReadFromFile(int chunkIndex);

        protected void WriteToFile(int chunkx, int chunky, T[] data) => WriteToFile(GetChunkIndex(chunkx, chunky), data);

        protected abstract void WriteToFile(int chunkindex, T[] data);

        protected static int GetChunkIndex(int chunkx, int chunky) => chunkx + chunky * ChunksH;
    }
}
