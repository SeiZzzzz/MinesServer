using MinesServer.Network.Constraints;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using System.Text;

namespace MinesServer.Network.World
{
    public readonly struct HBPacket : ITopLevelPacket, IDataPart<HBPacket>
    {
        const int eventTypeLength = sizeof(byte);

        public readonly IHubPacket[] data;

        public const string packetName = "HB";

        public string PacketName => packetName;

        private static PacketDecoder? GetDecoder(string packetName) => packetName switch
        {
            HBBotsPacket.packetName => x => HBBotsPacket.Decode(x),
            HBChatPacket.packetName => x => HBChatPacket.Decode(x),
            HBDirectedFXPacket.packetName => x => HBDirectedFXPacket.Decode(x),
            HBFXPacket.packetName => x => HBFXPacket.Decode(x),
            HBLeavePacket.packetName => x => HBLeavePacket.Decode(x),
            HBMapPacket.packetName => x => HBMapPacket.Decode(x),
            HBBotPacket.packetName => x => HBBotPacket.Decode(x),
            HBGunPacket.packetName => x => HBGunPacket.Decode(x),
            HBRemoveBotPacket.packetName => x => HBRemoveBotPacket.Decode(x),
            HBPacksPacket.packetName => x => HBPacksPacket.Decode(x),
            _ => null
        };

        public static HBPacket Decode(ReadOnlySpan<byte> input)
        {
            var caret = 0;
            var result = new List<IHubPacket>();
            while (caret < input.Length)
            {
                var eventType = Convert.ToChar(input[caret]);
                var decoder = GetDecoder(eventType.ToString()) ?? throw new InvalidPayloadException($"Invalid HB event type: {eventType}");
                caret++;
                var data = decoder(input[caret..]);
                result.Add((IHubPacket)data);
                caret += data.Length;
            }

            return new(result.ToArray());
        }

        public HBPacket(IHubPacket[] data)
        {
            var invalidPacket = data.FirstOrDefault(x => GetDecoder(x.PacketName) is null);
            if (invalidPacket is not null) throw new InvalidPayloadException($"Invalid event type: {invalidPacket.PacketName}");
            this.data = data;
        }

        public int Encode(Span<byte> output)
        {
            var caret = 0;
            foreach (var entry in data)
            {
                caret += Encoding.UTF8.GetBytes(entry.PacketName, output[caret..]);
                var length = entry.Encode(output[caret..]);
                caret += length;
            }
            return caret;
        }

        public int Length => eventTypeLength * data.Length + data.Sum(x => x.Length);
    }
}
