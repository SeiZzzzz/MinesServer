using System;
using System.Text;
using MinesServer.Utils;
using SimpleJSON;

namespace MinesServer.Network.Programmator
{
    public readonly record struct OpenProgrammatorPacket(int id, string title, string source) : IDataPart<OpenProgrammatorPacket>
    {
        public const string packetName = "#P";

        public string PacketName => packetName;

        public int Length => 30 + id.Digits() + Encoding.UTF8.GetByteCount(title) + source.Length;

        public static OpenProgrammatorPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["id"], obj["title"], obj["source"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"id":{{id}},"title":"{{title}}","source":"{{source}}"}""", output);
    }
}
