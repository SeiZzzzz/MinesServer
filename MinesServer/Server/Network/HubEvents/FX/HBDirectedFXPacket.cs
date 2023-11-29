using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.FX
{
    public readonly record struct HBDirectedFXPacket(int BotId, int X, int Y, int FX, int Direction, int Color) : IHubPacket, IDataPart<HBDirectedFXPacket>
    {
        public const string packetName = "D";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 3 + sizeof(ushort) * 3;

        public static HBDirectedFXPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int fx = Convert.ToInt32(decodeFrom[0]);
            int dir = Convert.ToInt32(decodeFrom[1]);
            int col = Convert.ToInt32(decodeFrom[2]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[3..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[5..]));
            int bid = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[7..]));
            return new(bid, x, y, fx, dir, col);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(FX);
            output[1] = Convert.ToByte(Direction);
            output[2] = Convert.ToByte(Color);
            var bytesWritten = 3;
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            var tmpbid = Convert.ToUInt16(BotId);
            MemoryMarshal.Write(output[3..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[5..], in tmpy);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[7..], in tmpbid);
            bytesWritten += sizeof(ushort);
            return bytesWritten;
        }
    }
}
