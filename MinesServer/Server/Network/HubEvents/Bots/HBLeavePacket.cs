using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBLeavePacket(int botId) : IDataPart<HBLeavePacket>
    {
        public const string packetName = "L";

        public string PacketName => packetName;

        public int Length => sizeof(ushort);

        public static HBLeavePacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom)));

        public int Encode(Span<byte> output)
        {
            var tmpbid = Convert.ToUInt16(botId);
            MemoryMarshal.Write(output, ref tmpbid);
            return sizeof(ushort);
        }
    }
}
