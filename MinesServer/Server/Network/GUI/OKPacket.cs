using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct OKPacket(string title, string text) : IDataPart<OKPacket>
    {
        public const string packetName = "OK";

        public string PacketName => packetName;

        public int Length => 1 + Encoding.UTF8.GetByteCount(title) + Encoding.UTF8.GetByteCount(text);

        public static OKPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('#');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(parts[0], parts[1]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(title + "#" + text, output);
    }
}
