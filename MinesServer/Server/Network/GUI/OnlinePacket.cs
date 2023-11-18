using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct OnlinePacket(int online, int prog) : IDataPart<OnlinePacket>
    {
        public const string packetName = "ON";

        public string PacketName => packetName;

        public int Length => 1 + online.Digits() + prog.Digits();

        public static OnlinePacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(online + ":" + prog, output);
    }
}
