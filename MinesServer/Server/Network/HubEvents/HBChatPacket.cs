using System.Runtime.InteropServices;
using System.Text;

namespace MinesServer.Network.HubEvents
{
    public readonly record struct HBChatPacket(int bid, int x, int y, string message) : IDataPart<HBChatPacket>
    {
        public const string packetName = "C";

        public string PacketName => packetName;

        public int Length => sizeof(ushort) * 4 + Encoding.UTF8.GetByteCount(message);

        public static HBChatPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int bid = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom));
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[2..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[4..]));
            int messageLength = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[6..]));
            string message = Encoding.UTF8.GetString(decodeFrom[8..(8 + messageLength)]);
            return new(bid, x, y, message);
        }

        public int Encode(Span<byte> output)
        {
            var tmpbid = Convert.ToUInt16(bid);
            var tmpx = Convert.ToUInt16(x);
            var tmpy = Convert.ToUInt16(y);
            var bytesWritten = Encoding.UTF8.GetBytes(message, output[8..]);
            var tmplen = Convert.ToUInt16(bytesWritten);
            MemoryMarshal.Write(output, in tmpbid);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[2..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[4..], in tmpy);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[6..], in tmplen);
            bytesWritten += sizeof(ushort);
            return bytesWritten;
        }
    }
}
