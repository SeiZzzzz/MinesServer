using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace MinesServer.Network.GUI
{
    public readonly record struct InventoryPacket(IDataPartBase data) : IDataPart<InventoryPacket>
    {
        public const string packetName = "IN";

        public string PacketName => packetName;

        public string EventType => data.PacketName;

        public int Length => EventType.Length + 1 + data.Length;

        private static PacketDecoder GetDecoder(string packetName) => packetName switch
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
            return new(GetDecoder(eventType)(decodeFrom[(ind + 1)..]));
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = stackalloc byte[data.Length];
            data.Encode(span);
            return Encoding.UTF8.GetBytes($"{EventType}:{Encoding.UTF8.GetString(span)}", output);
        }
    }
}
