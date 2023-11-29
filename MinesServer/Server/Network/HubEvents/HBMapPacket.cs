using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents
{
    public readonly record struct HBMapPacket(int X, int Y, int Width, int Height, byte[] Cells) : IHubPacket, IDataPart<HBMapPacket>
    {
        public const string packetName = "M";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 2 + sizeof(ushort) * 2 + Cells.Length;

        public static HBMapPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int width = Convert.ToInt32(decodeFrom[0]);
            int height = Convert.ToInt32(decodeFrom[1]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[2..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[4..]));
            return new(x, y, width, height, decodeFrom[6..(6 + width * height)].ToArray());
        }

        public int Encode(Span<byte> output)
        {
            if (Width * Height != Cells.Length) throw new ArgumentOutOfRangeException(nameof(Cells), "Chunk size does not match the cells array size");
            output[0] = Convert.ToByte(Width);
            output[1] = Convert.ToByte(Height);
            var bytesWritten = 2;
            var tmpx = Convert.ToUInt16(Convert.ToInt32(X));
            var tmpy = Convert.ToUInt16(Convert.ToInt32(Y));
            MemoryMarshal.Write(output[2..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[4..], in tmpy);
            bytesWritten += sizeof(ushort);
            var span = Cells.AsSpan();
            span.CopyTo(output[6..]);
            bytesWritten += span.Length;
            return bytesWritten;
        }
    }
}
