using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct XbldPacket(int Direction, string BlockType) : ITypicalPacket, IDataPart<XbldPacket>
    {
        public const string packetName = "Xbld";

        public string PacketName => packetName;

        public int Length => Direction.Digits() + BlockType.Length;

        public static XbldPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var data = Encoding.UTF8.GetString(decodeFrom);
            var dir = int.Parse(data[..^1]);
            var blockType = data[^1..];
            return new(dir, blockType);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Direction + BlockType, output);
    }
}
