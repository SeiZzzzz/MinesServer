using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct PRENPacket(int Id) : ITypicalPacket, IDataPart<PRENPacket>
    {
        public const string packetName = "PREN";

        public string PacketName => packetName;

        public int Length => Id.Digits();

        public static PRENPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Id.ToString(), output);
    }
}
