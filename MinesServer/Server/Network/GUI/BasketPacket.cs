using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct BasketPacket(long g, long r, long b, long v, long w, long c, long capacity) : IDataPart<BasketPacket>
    {
        public const string packetName = "@B";

        public string PacketName => packetName;

        public int Length => 6 + g.Digits() + r.Digits() + b.Digits() + v.Digits() + w.Digits() + c.Digits() + capacity.Digits();

        public static BasketPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 7) throw new InvalidPayloadException($"Expected {7} parts but got {parts.Length}");
            return new(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]), long.Parse(parts[3]), long.Parse(parts[4]), long.Parse(parts[5]), long.Parse(parts[6]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{g}:{r}:{b}:{v}:{w}:{c}:{capacity}", output);
    }
}
