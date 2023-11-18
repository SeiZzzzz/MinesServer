using System;
using System.Linq;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct MissPacket : IDataPart<MissPacket>
    {
        public const string packetName = "Miss";

        public string PacketName => packetName;

        public int Length => 1;

        public static MissPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
