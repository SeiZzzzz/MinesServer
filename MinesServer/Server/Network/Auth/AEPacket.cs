using System.Text;

namespace MinesServer.Network.Auth
{
    public readonly record struct AEPacket(string message) : IDataPart<AEPacket>
    {
        public const string packetName = "AE";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static AEPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
