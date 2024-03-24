using MinesServer.Network;
using MinesServer.Network.Constraints;

namespace MinesServer.Server.Network.TypicalEvents
{
    public readonly struct TAURPacket : ITypicalPacket, IDataPart<TAURPacket>
    {
        public const string packetName = "TAUR";

        public string PacketName => packetName;

        public int Length => 1;

        public static TAURPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            try
            {
                if (!decodeFrom.SequenceEqual([(byte)'_'])) throw new InvalidPayloadException("Invalid payload");
                return new();
            }
            catch { return new(); }
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = [(byte)'_'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
