using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct LoclPacket : IDataPart<LoclPacket>
    {
        public readonly string message;

        public const string packetName = "Locl";

        public string PacketName => packetName;

        public LoclPacket(string msg) => message = msg;

        public int Length => Encoding.UTF8.GetByteCount(message);

        public static LoclPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(message, output);
    }
}
