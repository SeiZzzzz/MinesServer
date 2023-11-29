using MinesServer.Network.Constraints;

namespace MinesServer.Network
{
    [Obsolete("This packet is no longer supported by the client.")]
    public readonly struct PurchasePacket : ITopLevelPacket, IDataPart<PurchasePacket>
    {
        public const string packetName = "$$";

        public string PacketName => packetName;

        public int Length => 1;

        public static PurchasePacket Decode(ReadOnlySpan<byte> decodeFrom)
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
