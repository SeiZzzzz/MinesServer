using MinesServer.Network.Constraints;
using MinesServer.Utils;
using MoreLinq;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct SkillsPacket(Dictionary<string, int> Skills) : ITopLevelPacket, IDataPart<SkillsPacket>
    {
        public const string packetName = "@S";

        public string PacketName => packetName;

        public int Length => Math.Max(Skills.Count, 1) + Skills.Sum(x => x.Key.Length + 1 + x.Value.Digits());

        public static SkillsPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split('#', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(':')).ToDictionary(x => x[0], x => int.Parse(x[1])));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join("#", Skills.Select(x => x.Key + ":" + x.Value)) + "#", output);
    }
}
