﻿using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Movement
{
    public readonly record struct TPPacket(int X, int Y) : ITopLevelPacket, IDataPart<TPPacket>
    {
        public const string packetName = "@T";

        public string PacketName => packetName;

        public int Length => 1 + X.Digits() + Y.Digits();

        public static TPPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(X + ":" + Y, output);
    }
}
