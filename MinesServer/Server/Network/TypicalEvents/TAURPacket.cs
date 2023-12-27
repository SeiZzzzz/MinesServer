using MinesServer.Network.Constraints;
using MinesServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.Server.Network.TypicalEvents
{
    public readonly struct TAURPacket : ITypicalPacket, IDataPart<TAURPacket>
    {
        public const string packetName = "TAUR";

        public string PacketName => packetName;

        public int Length => 1;

        public static TAURPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'_'])) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = [(byte)'_'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
