using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBBotsPacket(int[] Bots) : IHubPacket, IDataPart<HBBotsPacket>
    {
        public const string packetName = "B";

        public string PacketName => packetName;

        public int Length => 2 + Bots.Length * sizeof(short);

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
            var tmplen = Convert.ToUInt16(Bots.Length);
            MemoryMarshal.Write(output, in tmplen);
            var bytesWritten = sizeof(ushort);
            for (int j = 0; j < Bots.Length; j++)
            {
                var num3 = Convert.ToUInt16(Bots[j]);
                MemoryMarshal.Write(output[(2 + j * 2)..], in num3);
                bytesWritten += sizeof(ushort);
            }
            return bytesWritten;
        }
    }
}
