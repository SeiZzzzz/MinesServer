using Microsoft.Identity.Client;
using Syroot.BinaryData;
using System.IO;

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
            if (!File.Exists(path))
            {
                MapExists = false;
            }
            stream = new BinaryStream(File.Open(path, FileMode.OpenOrCreate));
            rstream = new BinaryStream(File.Open(rpath, FileMode.OpenOrCreate));
        }
        public bool MapExists = true;
        public void SetCell(int x, int y, byte type)
        {
            if (GetProp(type).isEmpty)
            {
                WithoutCheckSet(x, y, 0);
                WithoutCheckSetRoad(x, y, type);
                return;
            }
            WithoutCheckSet(x, y, 0);
        }
        public byte GetCell(int x, int y)
        {
            byte b = 32;
            if (TryGet1ByteWorld(x, y, out var cell))
            {
                b = cell;
            }
            if (b == 0 && TryGet1ByteRoads(x, y,out var rcell))
            {
                b = rcell;
            }
            return b;
        }
        private bool TryGet1ByteWorld(int x,int y, out byte cell)
        {
            WSGto(x, y);
            try
            {
                cell = stream.Read1Byte();
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                cell = 0; 
                return false; }
        }
        private bool TryGet1ByteRoads(int x, int y, out byte cell)
        {
            RSGto(x, y);
            try
            {
                cell = rstream.Read1Byte();
                return true;
            }
            catch (Exception ex) { cell = 32;SetCell(x,y,32) ; return true; }
        }
        private void WSGto(int x, int y) => stream.BaseStream.Position = x + y * this.MapHeight;
        private void RSGto(int x, int y) => rstream.BaseStream.Position = x + y * this.MapHeight;
        public byte[] LoadFrom(int x, int y, int width, int height)
        {
            var chunk = new byte[width * height];
            for (int cx = 0; cx < width; cx++)
            {
                for (int cy = 0; cy < height; cy++)
                {
                    byte b = 0;
                    if (TryGet1ByteWorld(x + cx,y + cy,out var result))
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
        public void WithoutCheckSet(int x,int y,byte type)
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
                        WSGto(x + tx, y + ty);
                        stream.WriteByte(0);
                        RSGto(x + tx, y + ty);
                        rstream.WriteByte(array[tx + ty * height]);
                    }
                    else
                    {
                        WSGto(x + tx, y + ty);
                        stream.WriteByte(array[tx + ty * height]);
                        if (!rstream.EndOfStream)
                        {
                            continue;
                        }
                        RSGto(x + tx, y + ty);
                        rstream.WriteByte(32);
                    }
                }
                Console.Write($"\r{c}/{array.Length}");
            }

        }
        private BinaryStream stream;
        private BinaryStream rstream;
        public readonly string path;
        public readonly string rpath;
        public readonly int MapWidth;
        public readonly int MapHeight;
    }


}
