using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.GUI
{
    public readonly record struct InventoryChoosePacket(string Hint, bool[,] Grid, int DX, int DY, int Distance) : IInventoryPacket, IDataPart<InventoryChoosePacket>
    {
        public const string packetName = "choose";

        public string PacketName => packetName;

        public int Length => 6 + Encoding.UTF8.GetByteCount(Hint) + DX.Digits() + DY.Digits() + Distance.Digits() + Grid.GetLength(0).Digits() + Grid.GetLength(1).Digits() + Grid.GetLength(0) * Grid.GetLength(1);

        public static InventoryChoosePacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split(':');
            if (parts.Length != 7) throw new InvalidPayloadException($"Expected {7} parts but got {parts.Length}");
            var hint = parts[0];
            var distance = int.Parse(parts[1]);
            var dx = int.Parse(parts[2]);
            var dy = int.Parse(parts[3]);
            var w = int.Parse(parts[4]);
            var h = int.Parse(parts[5]);
            if (w * h != parts[6].Length) throw new InvalidPayloadException($"width({w}) and height({h}) does not match the payload length. {w * h}!={parts[6].Length}");
            var grid = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    grid[x, y] = parts[6][x + y * w] == '1';
            return new(hint, grid, dx, dy, distance);
        }

        public int Encode(Span<byte> output)
        {
            var data = "";
            var w = Grid.GetLength(0);
            var h = Grid.GetLength(1);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    data += Grid[x, y] ? "1" : "0";
            return Encoding.UTF8.GetBytes($"{Hint}:{Distance}:{DX}:{DY}:{w}:{h}:{data}", output);
        }
    }
}
