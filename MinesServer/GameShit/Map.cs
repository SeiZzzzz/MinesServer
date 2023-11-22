using RT.Util.Streams;

namespace MinesServer.GameShit
{
    public class Map
    {
        public Map(int mapWidth, int mapHeight)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            path = World.W.name + ".mapb";
            rpath = World.W.name + "_roads.mapb";
            dpath = World.W.name + "_durability.mapb";
            if (!File.Exists(path))
            {
                MapExists = false;
            }
            stream = new BinaryStream(File.Open(path, FileMode.OpenOrCreate));
            rstream = new BinaryStream(File.Open(rpath, FileMode.OpenOrCreate));
            dstream = new BinaryStream(File.Open(dpath, FileMode.OpenOrCreate));
        }
        public bool MapExists = true;
        public void SaveChunk(Chunk ch)
        {
            lock (ch.durcells)
            {
                stream.Position = ch.pos.Item1 * World.W.chunksCountH * 1024 + ch.pos.Item2 * 1024;
                stream.Write(ch.wcells);
                rstream.Position = ch.pos.Item1 * World.W.chunksCountH * 1024 + ch.pos.Item2 * 1024;
                rstream.Write(ch.rcells);
                dstream.Position = ch.pos.Item1 * World.W.chunksCountH * 4 * 1024 + ch.pos.Item2 * 4 * 1024;
                var durbytes = new byte[ch.durcells.Length * 4];
                Buffer.BlockCopy(ch.durcells, 0, durbytes, 0, durbytes.Length);
                dstream.Write(durbytes);
            }
        }
        public void LoadChunk(Chunk ch)
        {
                stream.Position = ch.pos.Item1 * World.W.chunksCountH * 1024 + ch.pos.Item2 * 1024;
                stream.Read(ch.wcells);
                rstream.Position = ch.pos.Item1 * World.W.chunksCountH * 1024 + ch.pos.Item2 * 1024;
                rstream.Read(ch.rcells);
                dstream.Position = ch.pos.Item1 * World.W.chunksCountH * 4 * 1024 + ch.pos.Item2 * 4 * 1024;
                var durbytes = new byte[ch.durcells.Length * 4];
                dstream.Read(durbytes);
                Buffer.BlockCopy(durbytes, 0, ch.durcells, 0, ch.durcells.Length);
        }
        public void SaveAllChunks()
        {
            for (int x = 0; x < World.W.chunksCountW; x++)
            {

                for (int y = 0; y < World.W.chunksCountH; y++)
                {
                    var ch = World.W.chunks[x, y];
                    ch.Save();
                    ch.Dispose();
                }
            }
        }
        private BinaryStream stream;
        private BinaryStream rstream;
        private BinaryStream dstream;
        public readonly string path;
        public readonly string rpath;
        public readonly string dpath;
        public readonly int MapWidth;
        public readonly int MapHeight;
    }
    public static class ext
    {
        public static bool IsEndOfStream(this BinaryStream stream)
        {
            return stream.Position >= stream.Length;
        }
    }
}
