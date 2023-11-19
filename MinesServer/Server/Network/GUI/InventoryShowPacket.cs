using MinesServer.Utils;
using MoreLinq;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct InventoryShowPacket(Dictionary<int, int> grid, int selected, int all) : IDataPart<InventoryShowPacket>
    {
        public const string packetName = "show";

        public string PacketName => packetName;

        public int Length => 2 + selected.Digits() + all.Digits() + (grid.Count > 0 ? grid.Sum(x => x.Key.Digits() + 1 + x.Value.Digits()) : 1) + grid.Count - 1;

        public static InventoryShowPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 3) throw new InvalidPayloadException($"Expected {3} parts but got {parts.Length}");
            return new(parts[2].Split('#').Batch(2).ToDictionary(x => int.Parse(x[0]), x => int.Parse(x[1])), int.Parse(parts[1]), int.Parse(parts[0]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{all}:{selected}:{string.Join("#", grid.Select(x => x.Key + "#" + x.Value))}", output);
    }
}
