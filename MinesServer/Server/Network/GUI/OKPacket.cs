using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct OKPacket(string Title, string Text) : ITopLevelPacket, IDataPart<OKPacket>
    {
        public const string packetName = "OK";

        public string PacketName => packetName;

        public int Length => 1 + Encoding.UTF8.GetByteCount(Title) + Encoding.UTF8.GetByteCount(Text);

        public static OKPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('#');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(parts[0], parts[1]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Title + "#" + Text, output);
    }
}
