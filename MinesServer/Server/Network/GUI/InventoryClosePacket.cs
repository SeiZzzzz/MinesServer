using System;

namespace MinesServer.Network.GUI
{
    public readonly struct InventoryClosePacket : IDataPart<InventoryClosePacket>
    {
        public const string packetName = "close";

        public string PacketName => packetName;

        public int Length => 0;

        public static InventoryClosePacket Decode(ReadOnlySpan<byte> decodeFrom) => new();

        public int Encode(Span<byte> output) => 0;
    }
}
