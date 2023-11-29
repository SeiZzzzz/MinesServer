using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct GeoPacket(string Message) : ITopLevelPacket, IDataPart<GeoPacket>
    {
        public const string packetName = "GE";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Message);

        public static GeoPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Message, output);
    }
}
