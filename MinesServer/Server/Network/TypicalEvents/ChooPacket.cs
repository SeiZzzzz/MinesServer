using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct ChooPacket : IDataPart<ChooPacket>
    {
        public readonly string tag;

        public const string packetName = "Choo";

        public string PacketName => packetName;

        public ChooPacket(string tag) => this.tag = tag;

        public int Length => Encoding.UTF8.GetByteCount(tag);

        public static ChooPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(tag, output);
    }
}
