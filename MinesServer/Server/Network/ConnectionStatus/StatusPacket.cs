using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.ConnectionStatus
{
    public readonly record struct StatusPacket(string Message) : ITopLevelPacket, IDataPart<StatusPacket>
    {
        public const string packetName = "ST";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Message);

        public static StatusPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Message, output);
    }
}
