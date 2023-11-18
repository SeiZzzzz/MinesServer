using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct XmovPacket : IDataPart<XmovPacket>
    {
        public readonly int direction;

        public const string packetName = "Xmov";

        public string PacketName => packetName;

        public XmovPacket(int dir) => direction = dir;

        public int Length => direction.Digits();

        public static XmovPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(direction.ToString(), output);
    }
}
