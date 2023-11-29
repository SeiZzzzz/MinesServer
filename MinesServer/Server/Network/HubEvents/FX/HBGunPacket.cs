using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.FX
{
    public readonly record struct HBGunPacket(int X, int Y, int Color, int[] Bots) : IHubPacket, IDataPart<HBGunPacket>
    {
        public const string packetName = "Z";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 2 + sizeof(ushort) * 2 + Bots.Length * 2;

        public static HBGunPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int amount = Convert.ToInt32(decodeFrom[0]);
            int color = Convert.ToInt32(decodeFrom[1]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[2..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[4..]));
            var bots = new int[amount];
            for (int i = 0; i < amount; i++)
                bots[i] = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[(6 + i * 2)..]));
            return new(x, y, color, bots);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(Bots.Length);
            output[1] = Convert.ToByte(Color);
            var bytesWritten = 2;
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            MemoryMarshal.Write(output[2..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[4..], in tmpy);
            bytesWritten += sizeof(ushort);
            for (int i = 0; i < Bots.Length; i++)
            {
                var tmpi = Convert.ToUInt16(Bots[i]);
                MemoryMarshal.Write(output[(6 + i * sizeof(ushort))..], in tmpi);
                bytesWritten += sizeof(ushort);
            }
            return bytesWritten;
        }
    }
}
