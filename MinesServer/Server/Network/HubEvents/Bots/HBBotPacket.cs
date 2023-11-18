using System;
using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Bots
{
    public readonly record struct HBBotPacket(int id, int x, int y, int dir, int skin, int cid, int tail) : IDataPart<HBBotPacket>
    {
        public const string packetName = "X";

        public string PacketName => packetName;

        public int Length => sizeof(byte) * 3 + sizeof(ushort) * 4;

        public static HBBotPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            int dir = Convert.ToInt32(decodeFrom[0]);
            int skin = Convert.ToInt32(decodeFrom[1]);
            int tail = Convert.ToInt32(decodeFrom[2]);
            int id = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[3..]));
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[5..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[7..]));
            int cid = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[9..]));
            return new(id, x, y, dir, skin, cid, tail);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(dir);
            output[1] = Convert.ToByte(skin);
            output[2] = Convert.ToByte(tail);
            var bytesWritten = 3;
            var tmpid = Convert.ToUInt16(id);
            MemoryMarshal.Write(output[3..], in tmpid);
            bytesWritten += sizeof(ushort);
            var tmpx = Convert.ToUInt16(x);
            MemoryMarshal.Write(output[5..], in tmpx);
            bytesWritten += sizeof(ushort);
            var tmpy = Convert.ToUInt16(y);
            MemoryMarshal.Write(output[7..], in tmpy);
            bytesWritten += sizeof(ushort);
            var tmpcid = Convert.ToUInt16(cid);
            MemoryMarshal.Write(output[9..], in tmpcid);
            bytesWritten += sizeof(ushort);
            return bytesWritten;
        }
    }
}
