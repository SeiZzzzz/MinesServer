using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.FX
{
    public readonly record struct HBFXPacket(int X, int Y, int FX) : IHubPacket, IDataPart<HBFXPacket>
    {
        public const string packetName = "F";

        public string PacketName => packetName;

        public int Length => sizeof(byte) + sizeof(ushort) * 2;

        public static HBFXPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int fx = Convert.ToInt32(decodeFrom[0]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[1..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[3..]));
            return new(x, y, fx);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(FX);
            var bytesWritten = 1;
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            MemoryMarshal.Write(output[1..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[3..], in tmpy);
            bytesWritten += sizeof(ushort);
            return bytesWritten;
        }
    }
}
