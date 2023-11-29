using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Packs
{
    public readonly record struct HBPack(char Code, int X, int Y, int ClanId, int Off) : IDataPart<HBPack>
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
            output[0] = Convert.ToByte(Code);
            var bytesWritten = sizeof(char);
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            var tmpclan = Convert.ToUInt16(ClanId);
            MemoryMarshal.Write(output[1..], in tmpx);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[3..], in tmpy);
            bytesWritten += sizeof(ushort);
            MemoryMarshal.Write(output[5..], in tmpclan);
            bytesWritten += sizeof(ushort);
            output[7] = Convert.ToByte(Off);
            return ++bytesWritten;
        }
    }
}
