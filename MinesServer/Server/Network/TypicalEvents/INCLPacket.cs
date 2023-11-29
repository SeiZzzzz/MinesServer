using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct INCLPacket : ITypicalPacket, IDataPart<INCLPacket>
    {
        // Because myachin
        public readonly bool isSelectPacket;

        public readonly int? selection;

        public const string packetName = "INCL";

        public string PacketName => packetName;

        public INCLPacket() => isSelectPacket = false;

        public INCLPacket(int selection)
        {
            this.selection = selection;
            isSelectPacket = true;
        }

        public int Length => isSelectPacket ? selection.Value.Digits() : 1;

        public static INCLPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (decodeFrom.SequenceEqual([(byte)'_'])) return new();
            return new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));
        }

        public int Encode(Span<byte> output)
        {
            if (isSelectPacket) return Encoding.UTF8.GetBytes(selection.ToString(), output);
            Span<byte> span = [(byte)'_'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
