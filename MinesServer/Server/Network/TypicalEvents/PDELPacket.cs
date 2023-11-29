using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct PDELPacket(int Id) : ITypicalPacket, IDataPart<PDELPacket>
    {
        public const string packetName = "PDEL";

        public string PacketName => packetName;

        public int Length => Id.Digits();

        public static PDELPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Id.ToString(), output);
    }
}
