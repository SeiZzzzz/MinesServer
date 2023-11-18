using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBRemoveBotPacket(int bid, int block) : IDataPart<HBRemoveBotPacket>
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
            var tmpbid = Convert.ToUInt16(bid);
            MemoryMarshal.Write(output, ref tmpbid);
            var bytesWritten = sizeof(ushort);
            var tmpblock = block;
            MemoryMarshal.Write(output[2..], ref tmpblock);
            bytesWritten += sizeof(int);
            return bytesWritten;
        }
    }
}
