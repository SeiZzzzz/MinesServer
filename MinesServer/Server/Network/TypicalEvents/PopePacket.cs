using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PopePacket : IDataPart<PopePacket>
    {
        public readonly string source;

        public const string packetName = "Pope";

        public string PacketName => packetName;

        public PopePacket(string source) => this.source = source;

        public int Length => source.Length;

        public static PopePacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(source, output);
    }
}
