﻿using MinesServer.Network.Constraints;

namespace MinesServer.Network.GUI
{
    public readonly record struct AutoDiggPacket(bool IsEnabled) : ITopLevelPacket, IDataPart<AutoDiggPacket>
    {
        public const string packetName = "BD";

        public string PacketName => packetName;

        public int Length => 1;

        public static AutoDiggPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'0']) && !decodeFrom.SequenceEqual([(byte)'1'])) throw new InvalidPayloadException("Payload does not match any of the expected values");
            return new(decodeFrom[0] == (byte)'1');
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = IsEnabled ? [(byte)'1'] : [(byte)'0'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
