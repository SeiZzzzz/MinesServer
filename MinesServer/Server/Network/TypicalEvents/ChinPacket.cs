using System;
using System.Linq;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct ChinPacket : IDataPart<ChinPacket>
    {
        public const string packetName = "Chin";

        public string PacketName => packetName;

        public int Length => 1;

        public static ChinPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = stackalloc byte[1] { (byte)'0' };
            span.CopyTo(output);
            return span.Length;
        }
    }
}
