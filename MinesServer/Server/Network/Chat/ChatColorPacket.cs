using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatColorPacket(short color = 0) : IDataPart<ChatColorPacket>
    {
        public const string packetName = "mC";

        public string PacketName => packetName;

        public int Length => color.Digits();

        public static ChatColorPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var color = short.Parse(Encoding.UTF8.GetString(decodeFrom));
            if (color < 0 || color >= 20) throw new InvalidPayloadException($"Color is out of range [{0},{20})");
            return new(color);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(color.ToString(), output);
    }
}
