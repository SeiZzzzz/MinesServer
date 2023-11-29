using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct XdigPacket(int Direction) : ITypicalPacket, IDataPart<XdigPacket>
    {
        public const string packetName = "Xdig";

        public string PacketName => packetName;

        public int Length => Direction.Digits();

        public static XdigPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom).Trim()));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Direction.ToString(), output);
    }
}
