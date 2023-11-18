using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PCOPPacket : IDataPart<PCOPPacket>
    {
        public readonly int id;

        public const string packetName = "PCOP";

        public string PacketName => packetName;

        public PCOPPacket(int id) => this.id = id;

        public int Length => id.Digits();

        public static PCOPPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(id.ToString(), output);
    }
}
