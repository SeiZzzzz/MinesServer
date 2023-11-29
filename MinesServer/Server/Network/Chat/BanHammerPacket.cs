using MinesServer.Network.Constraints;

namespace MinesServer.Network.Chat
{
    [Obsolete("This packet is no longer supported by the client.")]
    public readonly record struct BanHammerPacket(bool IsEnabled) : ITopLevelPacket, IDataPart<BanHammerPacket>
    {
        public const string packetName = "SU";

        public string PacketName => packetName;

        public int Length => 1;

        public static BanHammerPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'0']) && !decodeFrom.SequenceEqual([(byte)'1'])) throw new InvalidPayloadException("Payload does not match any expected values");
            return new(decodeFrom[0] == (byte)'1');
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = IsEnabled ? [(byte)'1'] : [(byte)'0'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
