using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct GDonPacket : IDataPart<GDonPacket>
    {
        public readonly string method;

        public const string packetName = "GDon";

        public string PacketName => packetName;

        public GDonPacket(string method) => this.method = method;

        public int Length => Encoding.UTF8.GetByteCount(method);

        public static GDonPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(method, output);
    }
}
