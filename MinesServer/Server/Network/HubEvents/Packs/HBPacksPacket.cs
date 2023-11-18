using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Packs
{
    public readonly record struct HBPacksPacket(int block, HBPack[] packs) : IDataPart<HBPacksPacket>
    {
        public const string packetName = "O";

        public string PacketName => packetName;

        public int Length => sizeof(int) + sizeof(ushort) + packs.Sum(x => x.Length);

        public static HBPacksPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int block = MemoryMarshal.Read<int>(decodeFrom);
            int amount = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[4..]));
            var packs = new HBPack[amount];
            var caret = 0;
            for (int i = 0; i < amount; i++)
            {
                var pack = HBPack.Decode(decodeFrom[(6 + caret)..]);
                packs[i] = pack;
                caret += pack.Length;
            }
            return new(block, packs);
        }

        public int Encode(Span<byte> output)
        {
            var tmpblock = block;
            MemoryMarshal.Write(output, ref tmpblock);
            var bytesWritten = sizeof(int);
            var tmplen = Convert.ToUInt16(packs.Length);
            MemoryMarshal.Write(output[4..], ref tmplen);
            bytesWritten += sizeof(ushort);
            var caret = 0;
            for (int i = 0; i < packs.Length; i++)
            {
                var length = packs[i].Encode(output[(6 + caret)..]);
                caret += length;
            }
            bytesWritten += caret;
            return bytesWritten;
        }
    }
}
