using MinesServer.Network.Constraints;

namespace MinesServer.Network.GUI
{
    public readonly struct GuPacket : ITopLevelPacket, IDataPart<GuPacket>
    {
        public const string packetName = "Gu";

        public string PacketName => packetName;

        public int Length => 1;

        public static GuPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
