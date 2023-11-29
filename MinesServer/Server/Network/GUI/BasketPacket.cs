using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct BasketPacket(long G, long R, long B, long V, long W, long C, long Capacity) : ITopLevelPacket, IDataPart<BasketPacket>
    {
        public const string packetName = "@B";

        public string PacketName => packetName;

        public int Length => 6 + G.Digits() + R.Digits() + B.Digits() + V.Digits() + W.Digits() + C.Digits() + Capacity.Digits();

        public static BasketPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 7) throw new InvalidPayloadException($"Expected {7} parts but got {parts.Length}");
            return new(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]), long.Parse(parts[3]), long.Parse(parts[4]), long.Parse(parts[5]), long.Parse(parts[6]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{G}:{R}:{B}:{V}:{W}:{C}:{Capacity}", output);
    }
}
