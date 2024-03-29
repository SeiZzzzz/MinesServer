﻿using MinesServer.Network.Constraints;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBRemoveBotPacket(int BotId, int Block) : IHubPacket, IDataPart<HBRemoveBotPacket>
    {
        public const string packetName = "S";

        public string PacketName => packetName;

        public int Length => sizeof(ushort) + sizeof(int);

        public static HBRemoveBotPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int bid = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom));
            int block = MemoryMarshal.Read<int>(decodeFrom[2..]);
            return new(bid, block);
        }

        public int Encode(Span<byte> output)
        {
            var tmpbid = Convert.ToUInt16(BotId);
            MemoryMarshal.Write(output, in tmpbid);
            var bytesWritten = sizeof(ushort);
            var tmpblock = Block;
            MemoryMarshal.Write(output[2..], in tmpblock);
            bytesWritten += sizeof(int);
            return bytesWritten;
        }
    }
}
