using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct XbldPacket : IDataPart<XbldPacket>
    {
        public readonly int direction;
        public readonly string blockType;

        public const string packetName = "Xbld";

        public string PacketName => packetName;

        public XbldPacket(int dir, string block)
        {
            direction = dir;
            blockType = block;
        }

        public int Length => direction.Digits() + blockType.Length;

        public static XbldPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var data = Encoding.UTF8.GetString(decodeFrom);
            var dir = int.Parse(data[..^1]);
            var blockType = data[^1..];
            return new(dir, blockType);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(direction + blockType, output);
    }
}
