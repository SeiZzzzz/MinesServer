using System;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct GeoPacket(string message) : IDataPart<GeoPacket>
    {
        public const string packetName = "GE";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static GeoPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
