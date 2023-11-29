using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct RndmPacket : ITypicalPacket, IDataPart<RndmPacket>
    {
        public readonly string hash;

        public const string packetName = "Rndm";

        public string PacketName => packetName;

        public RndmPacket(string hash) => this.hash = hash;

        public int Length => 5 + hash.Length;

        public static RndmPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom[5..]));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes("hash=" + hash, output);
    }
}
