using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct LevelPacket(int Level) : ITopLevelPacket, IDataPart<LevelPacket>
    {
        public const string packetName = "LV";

        public string PacketName => packetName;

        public int Length => Level.Digits();

        public static LevelPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Level.ToString(), output);
    }
}
