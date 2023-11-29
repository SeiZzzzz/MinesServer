using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct ChatPacket : ITypicalPacket, IDataPart<ChatPacket>
    {
        public readonly string message;

        public const string packetName = "Chat";

        public string PacketName => packetName;

        public ChatPacket(string message) => this.message = message;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static ChatPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
