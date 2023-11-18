using System;
using System.Linq;

namespace MinesServer.Network.BotInfo
{
    public readonly struct ClanHidePacket : IDataPart<ClanHidePacket>
    {
        public const string packetName = "cH";

        public string PacketName => packetName;

        public int Length => 1;

        public static ClanHidePacket Decode(ReadOnlySpan<byte> decodeFrom)
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
