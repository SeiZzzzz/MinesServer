using MinesServer.Network.Constraints;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct INVNPacket : ITypicalPacket, IDataPart<INVNPacket>
    {
        public const string packetName = "INVN";

        public string PacketName => packetName;

        public int Length => 1;

        public static INVNPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
