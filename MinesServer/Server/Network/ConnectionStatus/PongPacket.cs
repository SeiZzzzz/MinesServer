using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.ConnectionStatus
{
    public readonly record struct PongPacket(int PongResponse, int CurrentTime) : ITopLevelPacket, IDataPart<PongPacket>, ITypicalPacket
    {
        public const string packetName = "PO";

        public string PacketName => packetName;

        public int Length => 1 + PongResponse.Digits() + CurrentTime.Digits();

        public static PongPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{PongResponse}:{CurrentTime}", output);
    }
}
