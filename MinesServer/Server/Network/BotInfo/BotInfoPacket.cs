using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct BotInfoPacket(string nickname, int x, int y, int bid) : IDataPart<BotInfoPacket>
    {
        public const string packetName = "BI";

        public string PacketName => packetName;

        public int Length => 27 + Encoding.UTF8.GetByteCount(nickname) + x.Digits() + y.Digits() + bid.Digits();

        public static BotInfoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["name"], obj["x"], obj["y"], obj["id"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"x":{{x}},"y":{{y}},"id":{{bid}},"name":"{{nickname}}"}""", output);
    }
}
