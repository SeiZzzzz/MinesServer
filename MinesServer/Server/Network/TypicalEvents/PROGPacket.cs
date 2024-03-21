using MinesServer.GameShit.Programmator;
using MinesServer.Network.Constraints;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PROGPacket : ITypicalPacket, IDataPart<PROGPacket>
    {
        // TODO: Perhaps chenge this to an actual prgram type?
        public readonly byte[] program;
        public readonly (int id,string source) prog
        {
            get
            {
                
                int id = BitConverter.ToInt32(program[4..8]);
                string source = Encoding.Default.GetString(program[8..(Length)]);
                source = source[source.IndexOf('$')..source.Length];
                return (id, source);
            }
        }

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
