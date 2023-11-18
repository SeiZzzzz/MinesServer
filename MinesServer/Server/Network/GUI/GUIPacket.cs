using System;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct GUIPacket(string window) : IDataPart<GUIPacket>
    {
        public const string packetName = "GU";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(window);

        public static GUIPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(window, output);
    }
}
