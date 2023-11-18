using SimpleJSON;
using System;
using System.Linq;
using System.Text;
using System.Threading.Channels;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatMessagesPacket(string channel, GCMessage[] messages) : IDataPart<ChatMessagesPacket>
    {
        public const string packetName = "mU";

        public string PacketName => packetName;

        public int Length => 16 + Encoding.UTF8.GetByteCount(channel) + messages.Sum(x => x.Length + 2) + messages.Length - 1;

        public static ChatMessagesPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            var messages = new GCMessage[obj["h"].Count];
            for (int i = 0; i < messages.Length; i++)
                messages[i] = GCMessage.Decode(Encoding.UTF8.GetBytes(obj["h"][i].Value));
            return new(obj["ch"], messages);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"ch":"{{channel}}","h":[{{string.Join(",", messages.Select(x =>
        {
            Span<byte> span = stackalloc byte[x.Length];
            x.Encode(span);
            return $"\"{Encoding.UTF8.GetString(span)}\"";
        }))}}]}""", output);
    }
}
