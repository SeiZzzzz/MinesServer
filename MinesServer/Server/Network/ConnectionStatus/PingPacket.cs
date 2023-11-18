using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.ConnectionStatus
{
    public readonly record struct PingPacket(int pongResponse, int clientTimeStart, string pingText) : IDataPart<PingPacket>
    {
        public const string packetName = "PI";

        public string PacketName => packetName;

        public int Length => 2 + Encoding.UTF8.GetByteCount(pingText) + pongResponse.Digits() + clientTimeStart.Digits();

        public static PingPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 3) throw new InvalidPayloadException($"Expected {3} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]), parts[2]);
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{pongResponse}:{clientTimeStart}:{pingText}", output);
    }
}
