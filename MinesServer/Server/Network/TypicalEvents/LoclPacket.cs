using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct LoclPacket(string Message) : ITypicalPacket, IDataPart<LoclPacket>
    {
        public const string packetName = "Locl";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(Message);

        public static LoclPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Message, output);
    }
}
