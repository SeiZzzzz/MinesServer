using System;
using System.Linq;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatListPacket(GCChatEntry[] chats) : IDataPart<ChatListPacket>
    {
        public const string packetName = "mL";

        public string PacketName => packetName;

        public int Length => chats.Length - 1 + chats.Sum(x => x.Length);

        public static ChatListPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split('#').Select(x => GCChatEntry.Decode(Encoding.UTF8.GetBytes(x))).ToArray());

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join("#", chats.Select(x => {
            Span<byte> span = stackalloc byte[x.Length];
            x.Encode(span);
            return Encoding.UTF8.GetString(span);
        })), output);
    }
}
