using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct InventoryPacket(IInventoryPacket Data) : ITopLevelPacket, IDataPart<InventoryPacket>
    {
        public const string packetName = "IN";

        public string PacketName => packetName;

        public string EventType => Data.PacketName;

        public int Length => EventType.Length + 1 + Data.Length;

        private static PacketDecoder? GetDecoder(string packetName) => packetName switch
        {
            InventoryShowPacket.packetName => x => InventoryShowPacket.Decode(x),
            InventoryFullPacket.packetName => x => InventoryFullPacket.Decode(x),
            InventoryClosePacket.packetName => x => InventoryClosePacket.Decode(x),
            InventoryChoosePacket.packetName => x => InventoryChoosePacket.Decode(x),
            _ => null
        };

        public static InventoryPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var ind = decodeFrom.IndexOf((byte)':');
            var eventType = Encoding.UTF8.GetString(decodeFrom[..ind]);
            var decoder = GetDecoder(eventType) ?? throw new InvalidPayloadException($"Invalid event type: {eventType}");
            return new((IInventoryPacket)decoder(decodeFrom[(ind + 1)..]));
        }

        public int Encode(Span<byte> output)
        {
            if (GetDecoder(EventType) is null) throw new NotImplementedException($"Event type {EventType} is not implemented");
            Span<byte> span = stackalloc byte[Data.Length];
            Data.Encode(span);
            return Encoding.UTF8.GetBytes($"{EventType}:{Encoding.UTF8.GetString(span)}", output);
        }

        public static InventoryPacket Choose(string hint, bool[,] grid, int dx, int dy, int distance) => new(new InventoryChoosePacket(hint, grid, dx, dy, distance));

        public static InventoryPacket Close() => new(new InventoryClosePacket());

        public static InventoryPacket Full(Dictionary<int, int> grid, int selected) => new(new InventoryFullPacket(grid, selected));

        public static InventoryPacket Show(Dictionary<int, int> grid, int selected, int all) => new(new InventoryShowPacket(grid, selected, all));
    }
}
