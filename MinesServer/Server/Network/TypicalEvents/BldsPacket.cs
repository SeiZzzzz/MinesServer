namespace MinesServer.Network.TypicalEvents
{
    public readonly struct BldsPacket : IDataPart<BldsPacket>
    {
        public const string packetName = "Blds";

        public string PacketName => packetName;

        public int Length => 1;

        public static BldsPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual(stackalloc byte[1] { (byte)'_' })) throw new InvalidPayloadException("Invalid payload");
            return new();
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = stackalloc byte[1] { (byte)'_' };
            span.CopyTo(output);
            return span.Length;
        }
    }
}
