namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PROGPacket : IDataPart<PROGPacket>
    {
        // TODO: Perhaps chenge this to an actual prgram type?
        public readonly byte[] program;

        public const string packetName = "PROG";

        public string PacketName => packetName;

        public PROGPacket(byte[] program) => this.program = program;

        public int Length => program.Length;

        public static PROGPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(decodeFrom.ToArray());

        public int Encode(Span<byte> output)
        {
            var span = program.AsSpan();
            span.CopyTo(output);
            return span.Length;
        }
    }
}
