using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct FINVPacket(int Index) : ITypicalPacket, IDataPart<FINVPacket>
    {
        public const string packetName = "FINV";

        public string PacketName => packetName;

        public int Length => Index.Digits();

        public static FINVPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Index.ToString(), output);
    }
}
