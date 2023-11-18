using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    [Obsolete("FIX THIS!!!!!!")]
    public readonly struct ChinPacket : IDataPart<ChinPacket>
    {
        public readonly string message;

        public const string packetName = "Chin";

        public string PacketName => packetName;

        public ChinPacket(string msg) => message = msg;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static ChinPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
