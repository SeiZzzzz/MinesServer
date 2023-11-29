using MinesServer.Network.Constraints;
using MoreLinq;
using System.Text;

namespace MinesServer.Network
{
    public readonly record struct SettingsPacket(Dictionary<string, string> Settings) : ITopLevelPacket, IDataPart<SettingsPacket>
    {
        public const string packetName = "#S";

        public string PacketName => packetName;

        public int Length => Settings.Sum(x => 1 + Encoding.UTF8.GetByteCount(x.Key) + Encoding.UTF8.GetByteCount(x.Value)) + Settings.Count;

        public static SettingsPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split('#', System.StringSplitOptions.RemoveEmptyEntries).Batch(2).ToDictionary(x => x[0], x => x[1]));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes("#" + string.Join("#", Settings.Select(x => x.Key + "#" + x.Value)), output);
    }
}
