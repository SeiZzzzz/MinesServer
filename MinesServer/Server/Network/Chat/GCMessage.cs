using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct GCMessage(int Id, int Time, int ClanId, int UserId, string Nickname, string Text, int Color, string Realtxt = "") : IDataPart<GCMessage>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => 7 * 2 + Id.Digits() + Color.Digits() + ClanId.Digits() + Time.Digits() + Encoding.UTF8.GetByteCount(Nickname) + Encoding.UTF8.GetByteCount(Text) + UserId.Digits() + Encoding.UTF8.GetByteCount(Realtxt);

        public static GCMessage Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('±');
            if (parts.Length != 8) throw new InvalidPayloadException($"Expected {8} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[3]), int.Parse(parts[2]), int.Parse(parts[6]), parts[4], parts[5], int.Parse(parts[1]), parts[7]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{Id}±{Color}±{ClanId}±{Time}±{Nickname}±{Text}±{UserId}±{Realtxt}", output);
    }
}
