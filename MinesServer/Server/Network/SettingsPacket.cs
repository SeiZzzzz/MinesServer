using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinesServer.Network
{
    public readonly struct SettingsPacket : IDataPart<SettingsPacket>
    {
        public readonly Dictionary<string, string> settings;

        public const string packetName = "#S";

        public string PacketName => packetName;

        public SettingsPacket(Dictionary<string, string> settings) => this.settings = settings;

        public int Length => settings.Sum(x => 1 + Encoding.UTF8.GetByteCount(x.Key) + Encoding.UTF8.GetByteCount(x.Value)) + settings.Count;

        public static SettingsPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom).Split('#', System.StringSplitOptions.RemoveEmptyEntries).Batch(2).ToDictionary(x => x[0], x => x[1]));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes("#" + string.Join("#", settings.Select(x => x.Key + "#" + x.Value)), output);
    }
}
