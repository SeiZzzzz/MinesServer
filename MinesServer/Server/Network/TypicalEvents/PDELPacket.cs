using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PDELPacket : IDataPart<PDELPacket>
    {
        public readonly int id;

        public const string packetName = "PDEL";

        public string PacketName => packetName;

        public PDELPacket(int id) => this.id = id;

        public int Length => id.Digits();

        public static PDELPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(id.ToString(), output);
    }
}
