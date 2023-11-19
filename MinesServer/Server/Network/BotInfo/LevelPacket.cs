using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct LevelPacket(int level) : IDataPart<LevelPacket>
    {
        public const string packetName = "LV";

        public string PacketName => packetName;

        public int Length => level.Digits();

        public static LevelPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(level.ToString(), output);
    }
}
