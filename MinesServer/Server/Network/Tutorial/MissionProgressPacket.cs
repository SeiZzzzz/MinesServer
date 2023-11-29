using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Tutorial
{
    public readonly record struct MissionProgressPacket(int Experience, int Max) : ITopLevelPacket, IDataPart<MissionProgressPacket>
    {
        public const string packetName = "MP";

        public string PacketName => packetName;

        public int Length => 1 + Experience.Digits() + Max.Digits();

        public static MissionProgressPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(Experience + ":" + Max, output);
    }
}
