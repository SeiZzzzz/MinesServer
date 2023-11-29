using MinesServer.Network.Constraints;
using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct BotInfoPacket(string Nickname, int X, int Y, int BotID) : ITopLevelPacket, IDataPart<BotInfoPacket>
    {
        public const string packetName = "BI";

        public string PacketName => packetName;

        public int Length => 27 + Encoding.UTF8.GetByteCount(Nickname) + X.Digits() + Y.Digits() + BotID.Digits();

        public static BotInfoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["name"], obj["x"], obj["y"], obj["id"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"x":{{X}},"y":{{Y}},"id":{{BotID}},"name":"{{Nickname}}"}""", output);
    }
}
