using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.Auth
{
    public readonly struct AHPacket : IDataPart<AHPacket>
    {
        public readonly bool isBad;
        public readonly int? user_id;
        public readonly string? user_hash;

        public const string packetName = "AH";

        public string PacketName => packetName;

        public AHPacket() => isBad = true;

        public AHPacket(int user_id, string user_hash)
        {
            isBad = false;
            this.user_id = user_id;
            this.user_hash = user_hash;
        }

        public int Length => isBad ? 3 : 1 + user_id!.Value.Digits() + user_hash!.Length;

        public static AHPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var str = Encoding.UTF8.GetString(decodeFrom);
            if (str == "BAD") return new();
            var parts = str.Split('_');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), parts[1]);
        }

        public int Encode(Span<byte> output)
        {
            if (isBad)
            {
                Span<byte> span = [(byte)'B', (byte)'A', (byte)'D'];
                span.CopyTo(output);
                return span.Length;
            }
            return Encoding.UTF8.GetBytes(user_id + "_" + user_hash, output);
        }
    }
}
