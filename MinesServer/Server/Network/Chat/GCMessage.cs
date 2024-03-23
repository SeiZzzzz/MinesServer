using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct GCMessage(int Time, int ClanId, int UserId, string Nickname, string Text, int Color) : IDataPart<GCMessage>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => 6 * 2 + Color.Digits() + ClanId.Digits() + Time.Digits() + Encoding.UTF8.GetByteCount(Nickname) + Encoding.UTF8.GetByteCount(Text) + UserId.Digits();

        public static GCMessage Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('±');
            if (parts.Length != 7) throw new InvalidPayloadException($"Expected {7} parts but got {parts.Length}");
            return new(int.Parse(parts[3]), int.Parse(parts[2]), int.Parse(parts[6]), parts[4], parts[5], int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"±{Color}±{ClanId}±{Time}±{Nickname}±{Text}±{UserId}", output);
    }
}
