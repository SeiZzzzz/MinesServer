using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct MoneyPacket(long money, long creds) : IDataPart<MoneyPacket>
    {
        public const string packetName = "P$";

        public string PacketName => packetName;

        public int Length => 19 + money.Digits() + creds.Digits();

        public static MoneyPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["money"], obj["creds"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"money":{{money}},"creds":{{creds}}}""", output);
    }
}
