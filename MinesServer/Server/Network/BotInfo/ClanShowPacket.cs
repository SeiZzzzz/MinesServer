using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct ClanShowPacket(int clan) : IDataPart<ClanShowPacket>
    {
        public const string packetName = "cS";

        public string PacketName => packetName;

        public int Length => clan.Digits();

        public static ClanShowPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(clan.ToString(), output);
    }
}
