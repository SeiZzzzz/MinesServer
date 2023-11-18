using System;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.FX
{
    public readonly record struct HBGunPacket(int x, int y, int color, int[] bots) : IDataPart<HBGunPacket>
    {
        public const string packetName = "Z";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 2 + sizeof(ushort) * 2 + bots.Length * 2;

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
            output[0] = Convert.ToByte(bots.Length);
            output[1] = Convert.ToByte(color);
            var bytesWritten = 2;
            var tmpx = Convert.ToUInt16(x);
            var tmpy = Convert.ToUInt16(y);
            MemoryMarshal.Write(output[2..], ref tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[4..], ref tmpy);
            bytesWritten += sizeof(ushort);
            for (int i = 0; i < bots.Length; i++)
            {
                var tmpi = Convert.ToUInt16(bots[i]);
                MemoryMarshal.Write(output[(6 + i * sizeof(ushort))..], ref tmpi);
                bytesWritten += sizeof(ushort);
            }
            return bytesWritten;
        }
    }
}
