using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct XmovPacket(int Direction) : ITypicalPacket, IDataPart<XmovPacket>
    {
        public const string packetName = "Xmov";

        public string PacketName => packetName;

        public int Length => Direction.Digits();

        public static XmovPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Direction.ToString(), output);
    }
}
