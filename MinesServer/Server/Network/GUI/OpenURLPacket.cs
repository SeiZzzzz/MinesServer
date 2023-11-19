using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct OpenURLPacket(string url) : IDataPart<OpenURLPacket>
    {
        public const string packetName = "GR";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(url);

        public static OpenURLPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(url, output);
    }
}
