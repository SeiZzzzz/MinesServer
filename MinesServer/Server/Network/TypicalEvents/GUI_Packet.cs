using MinesServer.Network.Constraints;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct GUI_Packet(string Button) : ITypicalPacket, IDataPart<GUI_Packet>
    {
        public const string packetName = "GUI_";

        public string PacketName => packetName;

        public int Length => 8 + Encoding.UTF8.GetByteCount(Button);

        public static GUI_Packet Decode(ReadOnlySpan<byte> decodeFrom) => new(JSON.Parse(Encoding.UTF8.GetString(decodeFrom))["b"]);

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"b":"{{Button}}"}""", output);
    }
}
