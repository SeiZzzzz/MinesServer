using MinesServer.Network.Constraints;
using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.Programmator
{
    public readonly record struct OpenProgrammatorPacket(int Id, string Title, string Source) : ITopLevelPacket, IDataPart<OpenProgrammatorPacket>
    {
        public const string packetName = "#P";

        public string PacketName => packetName;

        public int Length => 30 + Id.Digits() + Encoding.UTF8.GetByteCount(Title) + Encoding.UTF8.GetByteCount(Source);

        public static OpenProgrammatorPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["id"], obj["title"], obj["source"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"id":{{Id}},"title":"{{Title}}","source":"{{Source}}"}""", output);
    }
}
