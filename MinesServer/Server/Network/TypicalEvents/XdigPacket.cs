using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct XdigPacket : IDataPart<XdigPacket>
    {
        public readonly int direction;

        public const string packetName = "Xdig";

        public string PacketName => packetName;

        public XdigPacket(int dir) => direction = dir;

        public int Length => direction.Digits();

        public static XdigPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom).Trim()));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(direction.ToString(), output);
    }
}
