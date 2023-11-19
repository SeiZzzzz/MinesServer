using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly struct StatePanelPacket : IDataPart<StatePanelPacket>
    {
        public readonly bool isClear;

        public readonly string tag;
        public readonly string[] text;
        public readonly int? color;

        public const string packetName = "SP";

        public string PacketName => packetName;

        public StatePanelPacket()
        {
            isClear = true;
        }

        public StatePanelPacket(string tag, string[] text, int color)
        {
            this.tag = tag;
            this.text = text;
            this.color = color;
            isClear = false;
        }

        public int Length => isClear ? 7 : 2 + Encoding.UTF8.GetByteCount(tag) + text.Length - 1 + text.Sum(Encoding.UTF8.GetByteCount) + color.Value.Digits();

        public static StatePanelPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (decodeFrom.SequenceEqual(stackalloc byte[7] { (byte)'C', (byte)'L', (byte)'E', (byte)'A', (byte)'R', (byte)'#', (byte)'#' })) return new();
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('#', StringSplitOptions.RemoveEmptyEntries);
            return new(parts[0], parts[1].Split('~').ToArray(), int.Parse(parts[2]));
        }

        public int Encode(Span<byte> output)
        {
            if (isClear)
            {
                Span<byte> span = stackalloc byte[7] { (byte)'C', (byte)'L', (byte)'E', (byte)'A', (byte)'R', (byte)'#', (byte)'#' };
                span.CopyTo(output);
                return span.Length;
            }
            return Encoding.UTF8.GetBytes(tag + "#" + string.Join("~", text) + "#" + color, output);
        }
    }
}
