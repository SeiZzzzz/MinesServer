using System;
using System.Linq;

namespace MinesServer.Network.Programmator
{
    public readonly struct BibikaPacket : IDataPart<BibikaPacket>
    {
        public const string packetName = "BB";

        public string PacketName => packetName;

        public int Length => 1;

        public static BibikaPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual(stackalloc byte[1] { (byte)'_' })) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = stackalloc byte[1] { (byte)'_' };
            span.CopyTo(output);
            return span.Length;
        }
    }
}
