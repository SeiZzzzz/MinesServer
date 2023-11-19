using MinesServer.Utils;
using MoreLinq;
using System.Text;

namespace MinesServer.Network.World
{
    public readonly struct BadCellsPacket : IDataPart<BadCellsPacket>
    {
        public readonly (int x, int y)[] cells;

        public const string packetName = "BC";

        public string PacketName => packetName;

        public BadCellsPacket((int x, int y)[] cells) => this.cells = cells;

        public int Length => cells.Sum(x => 1 + x.x.Digits() + x.y.Digits()) + cells.Length - 1;

        public static BadCellsPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split(':', System.StringSplitOptions.RemoveEmptyEntries).Batch(2).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToArray());

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join(":", cells.Select(x => x.x + ":" + x.y)), output);
    }
}
