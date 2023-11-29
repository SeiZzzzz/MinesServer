using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct CpriPacket(int UserId) : ITypicalPacket, IDataPart<CpriPacket>
    {

        public const string packetName = "Cpri";

        public string PacketName => packetName;

        public int Length => UserId.Digits();

        public static CpriPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(UserId.ToString(), output);
    }
}
