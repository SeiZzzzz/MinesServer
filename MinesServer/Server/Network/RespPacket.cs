using MinesServer.Network.Constraints;

namespace MinesServer.Network
{
    public readonly struct RespPacket : ITopLevelPacket, IDataPart<RespPacket>
    {
        public const string packetName = "@R";

        public string PacketName => packetName;

        public int Length => 1;

        public static RespPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'1'])) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = [(byte)'1'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
