using System;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBBotsPacket(int[] bots) : IDataPart<HBBotsPacket>
    {
        public const string packetName = "B";

        public string PacketName => packetName;

        public int Length => 2 + bots.Length * sizeof(short);

        public static HBBotsPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int num2 = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom));
            int[] array = new int[num2];
            for (int j = 0; j < num2; j++)
                array[j] = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[(2 + j * 2)..]));
            return new(array);
        }

        public int Encode(Span<byte> output)
        {
            var tmplen = Convert.ToUInt16(bots.Length);
            MemoryMarshal.Write(output, ref tmplen);
            var bytesWritten = sizeof(ushort);
            for (int j = 0; j < bots.Length; j++)
            {
                var num3 = Convert.ToUInt16(bots[j]);
                MemoryMarshal.Write(output[(2 + j * 2)..], ref num3);
                bytesWritten += sizeof(ushort);
            }
            return bytesWritten;
        }
    }
}
