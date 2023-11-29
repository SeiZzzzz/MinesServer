using MinesServer.Network.Constraints;
using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.Programmator
{
    public readonly record struct UpdateProgrammatorPacket(int Id, string Title, string Source) : ITopLevelPacket, IDataPart<UpdateProgrammatorPacket>
    {
        public const string packetName = "#p";

        public string PacketName => packetName;

        public int Length => 30 + Id.Digits() + Encoding.UTF8.GetByteCount(Title) + Source.Length;

        public static UpdateProgrammatorPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["id"], obj["title"], obj["source"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"id":{{Id}},"title":"{{Title}}","source":"{{Source}}"}""", output);
    }
}
