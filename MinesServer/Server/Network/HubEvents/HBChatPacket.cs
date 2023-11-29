using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;
using System.Text;

namespace MinesServer.Network.HubEvents
{
    public readonly record struct HBChatPacket(int BotId, int X, int Y, string Message) : IHubPacket, IDataPart<HBChatPacket>
    {
        public const string packetName = "C";

        public string PacketName => packetName;

        public int Length => sizeof(ushort) * 4 + Encoding.UTF8.GetByteCount(Message);

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
            var tmpbid = Convert.ToUInt16(BotId);
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            var bytesWritten = Encoding.UTF8.GetBytes(Message, output[8..]);
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
