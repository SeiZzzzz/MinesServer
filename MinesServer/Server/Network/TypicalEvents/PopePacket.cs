using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct PopePacket(string Source) : ITypicalPacket, IDataPart<PopePacket>
    {
        public const string packetName = "Pope";

        public string PacketName => packetName;

        public int Length => Source.Length;

        public static PopePacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Source, output);
    }
}
