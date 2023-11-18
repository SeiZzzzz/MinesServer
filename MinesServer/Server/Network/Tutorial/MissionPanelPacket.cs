using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.Tutorial
{
    public readonly record struct MissionPanelPacket(string url, int imgx, int imgy, string text, int progress) : IDataPart<MissionPanelPacket>
    {
        public const string packetName = "MM";

        public string PacketName => packetName;

        public int Length => 4 + Encoding.UTF8.GetByteCount(url) + imgx.Digits() + imgy.Digits() + progress.Digits() + Encoding.UTF8.GetByteCount(text);

        public static MissionPanelPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('#');
            if (parts.Length != 5) throw new InvalidPayloadException($"Expected {5} parts but got {parts.Length}");
            return new(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), parts[4], int.Parse(parts[3]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{url}#{imgx}#{imgy}#{progress}#{text}", output);
    }
}
