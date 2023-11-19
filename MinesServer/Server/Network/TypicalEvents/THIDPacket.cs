using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct THIDPacket : IDataPart<THIDPacket>
    {
        public readonly string marker;

        public const string packetName = "THID";

        public string PacketName => packetName;

        public THIDPacket(string marker) => this.marker = marker;

        public int Length => Encoding.UTF8.GetByteCount(marker);

        public static THIDPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(marker, output);
    }
}
