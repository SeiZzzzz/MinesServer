using MinesServer.Utils;
using MoreLinq;
using System.Text;

namespace MinesServer.Network.World
{
    public readonly struct NickListPacket : IDataPart<NickListPacket>
    {
        public readonly Dictionary<int, string> nicks;

        public const string packetName = "NL";

        public string PacketName => packetName;

        public NickListPacket(Dictionary<int, string> nicks) => this.nicks = nicks;

        public int Length => nicks.Count - 1 + nicks.Sum(x => x.Key.Digits() + 1 + Encoding.UTF8.GetByteCount(x.Value));

        public static NickListPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split(',').Select(x => x.Split(':')).ToDictionary(x => int.Parse(x[0]), x => x[1]));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(string.Join(",", nicks.Select(x => x.Key + ":" + x.Value)), output);
    }
}
