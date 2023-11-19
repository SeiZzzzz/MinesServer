using MinesServer.Utils;
using MoreLinq;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct InventoryFullPacket(Dictionary<int, int> grid, int selected) : IDataPart<InventoryFullPacket>
    {
        public const string packetName = "full";

        public string PacketName => packetName;

        public int Length => 1 + selected.Digits() + grid.Sum(x => x.Key.Digits() + 1 + x.Value.Digits()) + grid.Count - 1;

        public static InventoryFullPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(parts[1].Split('#').Batch(2).ToDictionary(x => int.Parse(x[0]), x => int.Parse(x[1])), int.Parse(parts[0]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{selected}:{string.Join("#", grid.Select(x => x.Key + "#" + x.Value))}", output);
    }
}
