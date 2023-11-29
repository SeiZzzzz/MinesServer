using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct WhoiPacket(int[] BotIds) : ITypicalPacket, IDataPart<WhoiPacket>
    {
        public const string packetName = "Whoi";

        public string PacketName => packetName;

        public int Length => BotIds.Sum(x => x.Digits()) + BotIds.Length - 1;

        public static WhoiPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom)
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray());

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join(",", BotIds), output);
    }
}
