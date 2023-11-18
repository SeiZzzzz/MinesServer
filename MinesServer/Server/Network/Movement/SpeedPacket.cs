using MinesServer.Utils;
using System;
using System.Text;

namespace MinesServer.Network.Movement
{
    public readonly record struct SpeedPacket(int XY_PAUSE, int ROAD_PAUSE, int DEPTH) : IDataPart<SpeedPacket>
    {
        public const string packetName = "sp";

        public string PacketName => packetName;

        public int Length => 2 + XY_PAUSE.Digits() + ROAD_PAUSE.Digits() + DEPTH.Digits();

        public static SpeedPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 3) throw new InvalidPayloadException($"Expected {3} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(XY_PAUSE + ":" + ROAD_PAUSE + ":" + DEPTH, output);
    }
}
