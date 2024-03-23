using MinesServer.Network.Constraints;
using MinesServer.Utils;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.World
{
    public readonly record struct WorldInfoPacket(string Name, int Width, int Height, int VersionCode, string VersionName, string UpdateUrl, string UpdateDescription) : ITopLevelPacket, IDataPart<WorldInfoPacket>
    {

        public const string packetName = "cf";

        public string PacketName => packetName;

        public int Length => 81 + Encoding.UTF8.GetByteCount(Name) + Width.Digits() + Height.Digits() + VersionCode.Digits() + Encoding.UTF8.GetByteCount(VersionName) + Encoding.UTF8.GetByteCount(UpdateUrl) + Encoding.UTF8.GetByteCount(UpdateDescription);

        public static WorldInfoPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["name"], obj["width"], obj["height"], obj["v"], obj["version"], obj["update_url"], obj["update_desc"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"width":{{Width}},"height":{{Height}},"name":"{{Name}}","v":{{VersionCode}},"version":"{{VersionName}}","update_url":"{{UpdateUrl}}","update_desc":"{{UpdateDescription}}"}""", output);
    }

    public readonly record struct WorldInfoPacket2(string Name, int Width, int Height, int VersionCode, string VersionName, string UpdateUrl, string UpdateDescription) : ITopLevelPacket, IDataPart<WorldInfoPacket2>
    {

        public const string packetName = "CF";

        public string PacketName => packetName;

        public int Length => 81 + Encoding.UTF8.GetByteCount(Name) + Width.Digits() + Height.Digits() + VersionCode.Digits() + Encoding.UTF8.GetByteCount(VersionName) + Encoding.UTF8.GetByteCount(UpdateUrl) + Encoding.UTF8.GetByteCount(UpdateDescription);

        public static WorldInfoPacket2 Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            return new(obj["name"], obj["width"], obj["height"], obj["v"], obj["version"], obj["update_url"], obj["update_desc"]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($$"""{"width":{{Width}},"height":{{Height}},"name":"{{Name}}","v":{{VersionCode}},"version":"{{VersionName}}","update_url":"{{UpdateUrl}}","update_desc":"{{UpdateDescription}}"}""", output);
    }
}
