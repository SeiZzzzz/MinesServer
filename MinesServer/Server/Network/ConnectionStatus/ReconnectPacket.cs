using MinesServer.Network.Constraints;

namespace MinesServer.Network.ConnectionStatus
{
    public readonly struct ReconnectPacket : ITopLevelPacket, IDataPart<ReconnectPacket>
    {
        public const string packetName = "RC";

        public string PacketName => packetName;

        public int Length => 1;

        public static ReconnectPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'_'])) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = [(byte)'_'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
