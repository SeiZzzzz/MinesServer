using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.World
{
    public readonly struct WorldInfoPacket : IDataPart<WorldInfoPacket>
    {
        public readonly string name;
        public readonly string version;
        public readonly string update_url;
        public readonly string update_description;
        public readonly int width;
        public readonly int height;
        public readonly int v;

        public const string packetName = "CF";

        public string PacketName => packetName;

        public WorldInfoPacket(string name, int width, int height, int v, string version, string update_url, string update_desc)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.v = v;
            this.version = version;
            this.update_url = update_url;
            update_description = update_desc;
        }

        public int Length => 81 + Encoding.UTF8.GetByteCount(name) + width.Digits() + height.Digits() + v.Digits() + Encoding.UTF8.GetByteCount(version) + Encoding.UTF8.GetByteCount(update_url) + Encoding.UTF8.GetByteCount(update_description);

        public static WorldInfoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["name"], obj["width"], obj["height"], obj["v"], obj["version"], obj["update_url"], obj["update_desc"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"width":{{width}},"height":{{height}},"name":"{{name}}","v":{{v}},"version":"{{version}}","update_url":"{{update_url}}","update_desc":"{{update_description}}"}""", output);
    }
}
