using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct CpriPacket : IDataPart<CpriPacket>
    {
        public readonly int user_id;

        public const string packetName = "Cpri";

        public string PacketName => packetName;

        public CpriPacket(int id) => user_id = id;

        public int Length => user_id.Digits();

        public static CpriPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(user_id.ToString(), output);
    }
}
