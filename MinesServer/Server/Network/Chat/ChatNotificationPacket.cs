using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatNotificationPacket(short Amount = 0) : ITopLevelPacket, IDataPart<ChatNotificationPacket>
    {
        public const string packetName = "mN";

        public string PacketName => packetName;

        public int Length => Amount.Digits();

        public static ChatNotificationPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(short.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Amount.ToString(), output);
    }
}
