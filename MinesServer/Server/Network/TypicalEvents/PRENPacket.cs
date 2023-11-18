using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PRENPacket : IDataPart<PRENPacket>
    {
        public readonly int id;

        public const string packetName = "PREN";

        public string PacketName => packetName;

        public PRENPacket(int id) => this.id = id;

        public int Length => id.Digits();

        public static PRENPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(id.ToString(), output);
    }
}
