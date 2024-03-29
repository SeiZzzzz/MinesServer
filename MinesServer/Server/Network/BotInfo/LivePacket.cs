﻿using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.BotInfo
{
    public readonly record struct LivePacket(int CurrentHP, int MaxHP) : ITopLevelPacket, IDataPart<LivePacket>
    {
        public const string packetName = "@L";

        public string PacketName => packetName;

        public int Length => 1 + CurrentHP.Digits() + MaxHP.Digits();

        public static LivePacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 2) throw new InvalidPayloadException($"Expected {2} parts but got {parts.Length}");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(CurrentHP + ":" + MaxHP, output);
    }
}
