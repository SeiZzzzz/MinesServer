using System;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Packs
{
    public readonly record struct HBPack(char code, int x, int y, int clan, int off) : IDataPart<HBPack>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => sizeof(char) + sizeof(byte) + sizeof(ushort) * 3;

        public static HBPack Decode(ReadOnlySpan<byte> decodeFrom)
        {
            char code = Convert.ToChar(decodeFrom[0]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[1..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[3..]));
            int clan = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[5..]));
            int off = Convert.ToInt32(decodeFrom[7]);
            return new(code, x, y, clan, off);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(code);
            var bytesWritten = sizeof(char);
            var tmpx = Convert.ToUInt16(x);
            var tmpy = Convert.ToUInt16(y);
            var tmpclan = Convert.ToUInt16(clan);
            MemoryMarshal.Write(output[1..], ref tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[3..], ref tmpy);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[5..], ref tmpclan);
            bytesWritten += sizeof(ushort);
            output[7] = Convert.ToByte(off);
            return ++bytesWritten;
        }
    }
}
