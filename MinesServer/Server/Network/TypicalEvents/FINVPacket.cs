using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct FINVPacket : IDataPart<FINVPacket>
    {
        public readonly int index;

        public const string packetName = "FINV";

        public string PacketName => packetName;

        public FINVPacket(int ind) => index = ind;

        public int Length => index.Digits();

        public static FINVPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(index.ToString(), output);
    }
}
