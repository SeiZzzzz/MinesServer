using MinesServer.Network.Constraints;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct MisoPacket : ITypicalPacket, IDataPart<MisoPacket>
    {
        public const string packetName = "Miso";

        public string PacketName => packetName;

        public int Length => 1;

        public static MisoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'0'])) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = [(byte)'0'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
