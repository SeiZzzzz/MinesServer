using System.IO;
using System.Runtime.InteropServices;
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
        public void SetDurability(int x,int y,float d)
        {
            if (World.W.ValidCoord(x, y))
            {
                DurSet(x, y);
                if (dstream.IsEndOfStream())
                {
                    dstream.WriteFloat(GetProp(GetCell(x, y)).durability);
                }
                else
                {
                    dstream.WriteFloat(d);     
                }
            }
        }
        public void DurSet(int x,int y)
        {
            dstream.Position = x * MapHeight * 4 + y * 4;
        }
        public float GetDurability(int x, int y)
        {
            if (World.W.ValidCoord(x, y))
            {
                DurSet(x,y);
                if (dstream.IsEndOfStream())
                {
                    var dur = GetProp(GetCell(x, y)).durability;
                    dstream.WriteFloat(dur);
                    return dur;
                }
                else
                {
                    try
                    {
                        DurSet(x, y);
                        var dur = dstream.ReadFloat();
                        if (dur > 0)
                        {
                            return dur;
                        }
                        else
                        {
                            dur = GetProp(GetCell(x, y)).durability;
                            DurSet(x, y);
                            dstream.WriteFloat(dur);
                        }
                        return dur;
                    }
                    catch (Exception ex) { return 1; }
                }
            }
            return 0;
        }
        public void SetCell(int x, int y, byte type)
        {
            if (GetProp(type).isEmpty)
            {
                WithoutCheckSet(x, y, 0);
                WithoutCheckSetRoad(x, y, type);
                return;
            }
            DurSet(x, y);
            dstream.WriteFloat(GetProp(GetCell(x, y)).durability);
            WithoutCheckSet(x, y, 0);
        }
        public byte GetCell(int x, int y)
        {
            byte b = 32;
            if (TryGet1ByteWorld(x, y, out var cell))
            {
                b = cell;
            }
            if (b == 0 && TryGet1ByteRoads(x, y, out var rcell))
            {
                b = rcell;
            }
            return b;
        }
        public byte GetRoad(int x, int y)
        {
            byte b = 32;
            if (TryGet1ByteRoads(x, y, out var rcell))
            {
                b = rcell;
            }
            return b;
        }
        private bool TryGet1ByteWorld(int x, int y, out byte cell)
        {
            WSGto(x, y);
            try
            {
                cell = stream.ReadByte();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                cell = 32;
                return false;
            }
        }
        private bool TryGet1ByteRoads(int x, int y, out byte cell)
        {
            RSGto(x, y);
            try
            {
                cell = rstream.ReadByte();
                return true;
            }
            catch (Exception ex) { cell = 32; SetCell(x, y, 32); return true; }
        }
        private void WSGto(int x, int y) => stream.Position = x + y * this.MapHeight;
        private void RSGto(int x, int y) => rstream.Position = x + y * this.MapHeight;
        public byte[] LoadFrom(int x, int y, int width, int height)
        {
            var chunk = new byte[width * height];
            for (int cx = 0; cx < width; cx++)
            {
                for (int cy = 0; cy < height; cy++)
                {
                    byte b = 0;
                    if (TryGet1ByteWorld(x + cx, y + cy, out var result))
                    {
                        b = result;
                    }
                    if (b == 0 && TryGet1ByteRoads(x + cx, y + cy, out var c))
                    {
                        b = c;
                    }
                    chunk[cx + cy * height] = b;
                }
            }
            return chunk;
        }
        public void WithoutCheckSet(int x, int y, byte type)
        {
            WSGto(x, y);
            stream.WriteByte(type);
        }
        public void WithoutCheckSetRoad(int x, int y, byte type)
        {
            RSGto(x, y);
            rstream.WriteByte(type);
        }
        public static Cell GetProp(byte type)
        {
            return CellsSerializer.cells[type];
        }
        public void Save(int x, int y, byte[] array, int width, int height)
        {
            Console.WriteLine("saving");
            var c = 0;
            for (int tx = 0; tx < width; tx++)
            {
                for (int ty = 0; ty < height; ty++)
                {
                    c++;
                    if (GetProp(array[tx + ty * height]).isEmpty)
                    {
                        WithoutCheckSet(x + tx, y + ty, 0);
                        WithoutCheckSetRoad(x + tx, y + ty, array[tx + ty * height]);
                    }
                    else
                    {
                        WithoutCheckSet(x + tx, y + ty, array[tx + ty * height]);
                        if (!rstream.IsEndOfStream())
                        {
                            continue;
                        }
                        WithoutCheckSetRoad(x + tx, y + ty, 32);
                    }
                }
                Console.Write($"\r{c}/{array.Length}");
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
