using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct GCMessage(int id, int time, int cid, int gid, string nick, string text, int color, string realtxt = "") : IDataPart<GCMessage>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => 7 * 2 + id.Digits() + color.Digits() + cid.Digits() + time.Digits() + Encoding.UTF8.GetByteCount(nick) + Encoding.UTF8.GetByteCount(text) + gid.Digits() + Encoding.UTF8.GetByteCount(realtxt);

        public static GCMessage Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('±');
            if (parts.Length != 8) throw new InvalidPayloadException($"Expected {8} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[3]), int.Parse(parts[2]), int.Parse(parts[6]), parts[4], parts[5], int.Parse(parts[1]), parts[7]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{id}±{color}±{cid}±{time}±{nick}±{text}±{gid}±{realtxt}", output);
    }
}
