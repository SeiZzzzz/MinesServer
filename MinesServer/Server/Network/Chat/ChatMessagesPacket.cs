using MinesServer.Network.Constraints;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatMessagesPacket(string Channel, GCMessage[] Messages) : ITopLevelPacket, IDataPart<ChatMessagesPacket>
    {
        public const string packetName = "mU";

        public string PacketName => packetName;

        public int Length => 16 + Encoding.Default.GetByteCount(Channel) + Messages.Sum(x => x.Length + 2) + Messages.Length - 1;

        public static ChatMessagesPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            var messages = new GCMessage[obj["h"].Count];
            for (int i = 0; i < messages.Length; i++)
                messages[i] = GCMessage.Decode(Encoding.UTF8.GetBytes(obj["h"][i].Value));
            return new(obj["ch"], messages);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"ch":"{{Channel}}","h":[{{string.Join(",", Messages.Select(x =>
        {
            Span<byte> span = stackalloc byte[x.Length];
            x.Encode(span);
            return $"\"{Encoding.UTF8.GetString(span)}\"";
        }))}}]}""", output);
    }
}
