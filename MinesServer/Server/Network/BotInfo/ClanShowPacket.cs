using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct ClanShowPacket(int Clan) : ITopLevelPacket, IDataPart<ClanShowPacket>
    {
        public const string packetName = "cS";

        public string PacketName => packetName;

        public int Length => Clan.Digits();

        public static ClanShowPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Clan.ToString(), output);
    }
}
