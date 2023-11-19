using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct WhoiPacket : IDataPart<WhoiPacket>
    {
        public readonly int[] botIds;

        public const string packetName = "Whoi";

        public string PacketName => packetName;

        public WhoiPacket(int[] bots) => botIds = bots;

        public int Length => botIds.Sum(x => x.Digits()) + botIds.Length - 1;

        public static WhoiPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom)
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray());

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join(",", botIds), output);
    }
}
