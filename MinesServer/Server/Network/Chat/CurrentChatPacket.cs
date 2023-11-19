using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct CurrentChatPacket(string tag, string name) : IDataPart<CurrentChatPacket>
    {
        public const string packetName = "mO";

        public string PacketName => packetName;

        public int Length => 1 + Encoding.UTF8.GetByteCount(tag) + Encoding.UTF8.GetByteCount(name);

        public static CurrentChatPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(parts[0], parts[1]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{tag}:{name}", output);
    }
}
