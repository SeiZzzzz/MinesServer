using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct GDonPacket(string Method) : ITypicalPacket, IDataPart<GDonPacket>
    {
        public const string packetName = "GDon";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Method);

        public static GDonPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Method, output);
    }
}
