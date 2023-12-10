using System.Runtime.InteropServices;

namespace MinesServer.Network.HubEvents.Packs
{
    public readonly record struct HBPack(char Code, int X, int Y, int ClanId, int Off) : IDataPart<HBPack>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => sizeof(char) + sizeof(ushort) * 2 + sizeof(byte) * 2;

        public static HBPack Decode(ReadOnlySpan<byte> decodeFrom)
        {
            char code = Convert.ToChar(decodeFrom[0]);
            int x = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[1..]));
            int y = Convert.ToInt32(MemoryMarshal.Read<ushort>(decodeFrom[3..]));
            byte clan = decodeFrom[6];
            byte off = decodeFrom[7];
            return new(code, x, y, clan, off);
        }

        public int Encode(Span<byte> output)
        {
            output[0] = Convert.ToByte(Code);
            var bytesWritten = sizeof(char);
            var tmpx = Convert.ToUInt16(X);
            var tmpy = Convert.ToUInt16(Y);
            MemoryMarshal.Write(output[1..], in tmpx);
            bytesWritten += sizeof(ushort); 
            MemoryMarshal.Write(output[3..], in tmpy);
            bytesWritten += sizeof(ushort);
            output[5] = Convert.ToByte(ClanId);
            bytesWritten++;
            output[7] = Convert.ToByte(Off);
            bytesWritten++;
            return bytesWritten;
        }
    }
}
