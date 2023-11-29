using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct THIDPacket(string Marker) : ITypicalPacket, IDataPart<THIDPacket>
    {
        public const string packetName = "THID";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Marker);

        public static THIDPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Marker, output);
    }
}
