using System;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents
{
    public readonly record struct HBMapPacket(int x, int y, int width, int height, byte[] cells) : IDataPart<HBMapPacket>
    {
        public const string packetName = "M";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 2 + sizeof(ushort) * 2 + cells.Length;

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
            if (width * height != cells.Length) throw new InvalidPayloadException("Chunk size does not match the cells array size");
            output[0] = Convert.ToByte(width);
            output[1] = Convert.ToByte(height);
            var bytesWritten = 2;
            var tmpx = Convert.ToUInt16(Convert.ToInt32(x));
            var tmpy = Convert.ToUInt16(Convert.ToInt32(y));
            MemoryMarshal.Write(output[2..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[4..], in tmpy);
            bytesWritten += sizeof(ushort);
            var span = cells.AsSpan();
            span.CopyTo(output[6..]);
            bytesWritten += span.Length;
            return bytesWritten;
        }
    }
}
