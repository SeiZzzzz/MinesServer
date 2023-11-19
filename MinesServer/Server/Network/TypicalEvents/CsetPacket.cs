namespace MinesServer.Network.TypicalEvents
{
    public readonly struct CsetPacket : IDataPart<CsetPacket>
    {
        public const string packetName = "Cset";

        public string PacketName => packetName;

        public int Length => 1;

        public static CsetPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
