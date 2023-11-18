using System;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct GCChatEntry(string tag, string title, string nickname, string text, bool notification) : IDataPart<GCChatEntry>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => 9 + Encoding.UTF8.GetByteCount(tag) + Encoding.UTF8.GetByteCount(title) + Encoding.UTF8.GetByteCount(nickname) + Encoding.UTF8.GetByteCount(text);

        public static GCChatEntry Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('±');
            if (parts.Length != 4) throw new InvalidPayloadException($"(ChatEntry) Expected {4} parts but got {parts.Length}");
            if (parts[1] != "0" && parts[1] != "1") throw new InvalidPayloadException("Payload does not match any of the expected values");
            var msg = parts[3].Split(':', StringSplitOptions.TrimEntries);
            if (msg.Length != 2) throw new InvalidPayloadException($"(Message) Expected {2} parts but got {msg.Length}");
            return new(parts[0], parts[2], msg[0], msg[1], parts[1] == "1");
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{tag}±{(notification ? "1" : "0")}±{title}±{nickname}: {text}", output);
    }
}
