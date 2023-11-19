using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct SettPacket : IDataPart<SettPacket>
    {
        // Because myachin
        public readonly bool isOpenPacket;

        public readonly string key;
        public readonly string value;

        public const string packetName = "Sett";

        public string PacketName => packetName;

        public SettPacket()
        {
            isOpenPacket = true;
        }

        public SettPacket(string key, string value)
        {
            this.key = key;
            this.value = value;
            isOpenPacket = false;
        }

        public int Length => isOpenPacket ? 1 : Encoding.UTF8.GetByteCount(key) + 1 + Encoding.UTF8.GetByteCount(value);

        public static SettPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (decodeFrom.SequenceEqual(stackalloc byte[1] { (byte)'_' })) return new();
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(parts[0], parts[1]);
        }

        public int Encode(Span<byte> output)
        {
            if (isOpenPacket)
            {
                Span<byte> span = stackalloc byte[1] { (byte)'_' };
                span.CopyTo(output);
                return span.Length;
            }
            return Encoding.UTF8.GetBytes($"{key}:{value}", output);
        }
    }
}
