using System;
using System.Linq;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct MisoPacket : IDataPart<MisoPacket>
    {
        public const string packetName = "Miso";

        public string PacketName => packetName;

        public int Length => 1;

        public static MisoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual(stackalloc byte[1] { (byte)'0' })) throw new InvalidPayloadException("Invalid payload");
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
