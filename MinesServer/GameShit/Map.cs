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
                WSGto(x, y);
                stream.Write(0);
                RSGto(x, y);
                rstream.Write(type);
                return;
            }
            WSGto(x, y);
            stream.Write(type);
        }
        public byte GetCell(int x, int y)
        {
            byte b = 32;
            WSGto(x, y);
            if (!stream.EndOfStream)
            {
                b = stream.Read1Byte();
            }
            if (b == 0)
            {
                RSGto(x, y);
                if (!rstream.EndOfStream)
                {
                    RSGto(x, y);
                    b = rstream.Read1Byte();
                }
                else
                {
                    RSGto(x, y);
                    rstream.Write(32);
                    b = 32;
                }
            }
            return b;
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
                    WSGto(x + cx, y + cy);
                    byte b = stream.Read1Byte();
                    if (b == 0)
                    {
                        RSGto(x + cx, y + cy);
                        if (!rstream.EndOfStream)
                        {
                            b = rstream.Read1Byte();
                        }
                        else
                        {
                            b = 32;
                        }
                    }
                    chunk[cx + cy * height] = b;

                }
            }
            return chunk;
        }
        public void WithoutCheckSet(int x,int y,byte type)
        {
            WSGto(x, y);
            stream.Write(type);
        }
        public void WithoutCheckSetRoad(int x, int y, byte type)
        {
            RSGto(x, y);
            rstream.Write(type);
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
                        stream.Write(0);
                        RSGto(x + tx, y + ty);
                        rstream.Write(array[tx + ty * height]);
                    }
                    else
                    {
                        WSGto(x + tx, y + ty);
                        stream.Write(array[tx + ty * height]);
                        RSGto(x + tx, y + ty);
                        if (!rstream.EndOfStream)
                        {
                            continue;
                        }
                        RSGto(x + tx, y + ty);
                        rstream.Write(32);
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
