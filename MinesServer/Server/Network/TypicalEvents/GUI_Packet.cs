using SimpleJSON;
using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct GUI_Packet : IDataPart<GUI_Packet>
    {
        public readonly string button;

        public const string packetName = "GUI_";

        public string PacketName => packetName;

        public GUI_Packet(string btn) => button = btn;

        public int Length => 8 + Encoding.UTF8.GetByteCount(button);

        public static GUI_Packet Decode(ReadOnlySpan<byte> decodeFrom) => new(JSON.Parse(Encoding.UTF8.GetString(decodeFrom))["b"]);

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"b":"{{button}}"}""", output);
    }
}
