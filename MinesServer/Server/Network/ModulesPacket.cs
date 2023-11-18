using System;
using System.Linq;

namespace MinesServer.Network
{
    [Obsolete("This packet is no longer supported by the client.")]
    public readonly struct ModulesPacket : IDataPart<ModulesPacket>
    {
        public readonly string[] modules = Array.Empty<string>();

        public const string packetName = "PM";

        public ModulesPacket() { }

        public string PacketName => packetName;

        public int Length => 2;

        public static ModulesPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual("[]"u8.ToArray().AsSpan())) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            var span = "[]"u8.ToArray().AsSpan();
            span.CopyTo(output);
            return span.Length;
        }
    }
}
