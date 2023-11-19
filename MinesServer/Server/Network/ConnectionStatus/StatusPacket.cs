using System.Text;

namespace MinesServer.Network.ConnectionStatus
{
    public readonly record struct StatusPacket(string message) : IDataPart<StatusPacket>
    {
        public const string packetName = "ST";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static StatusPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
