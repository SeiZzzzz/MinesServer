using System;
using System.Text;
using MinesServer.Utils;
using SimpleJSON;

namespace MinesServer.Network.Programmator
{
    public readonly record struct UpdateProgrammatorPacket(int id, string title, string source) : IDataPart<UpdateProgrammatorPacket>
    {
        public const string packetName = "#p";

        public string PacketName => packetName;

        public int Length => 30 + id.Digits() + Encoding.UTF8.GetByteCount(title) + source.Length;

        public static UpdateProgrammatorPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["id"], obj["title"], obj["source"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"id":{{id}},"title":"{{title}}","source":"{{source}}"}""", output);
    }
}
