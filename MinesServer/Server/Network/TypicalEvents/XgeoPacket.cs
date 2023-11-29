using MinesServer.Network.Constraints;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct XgeoPacket : ITypicalPacket, IDataPart<XgeoPacket>
    {
        public const string packetName = "Xgeo";

        public string PacketName => packetName;

        public int Length => 1;

        public static XgeoPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
