using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct OpenURLPacket(string Url) : ITopLevelPacket, IDataPart<OpenURLPacket>
    {
        public const string packetName = "GR";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Url);

        public static OpenURLPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Url, output);
    }
}
