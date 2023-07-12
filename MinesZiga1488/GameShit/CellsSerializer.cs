using Newtonsoft.Json;

namespace MinesServer.GameShit
{
    public static class CellsSerializer
    {
        public static void Load()
        {
            var cellspath = "cells.json";
            if (File.Exists(cellspath))
            {
                cells = JsonConvert.DeserializeObject<Cell[]>(File.ReadAllText(cellspath));
            }
            else
            {
                cells = new Cell[126];
                for (int i = 0; i < 126; i++)
                {
                    cells[i] = new Cell((byte)i);
                }
                File.WriteAllText(cellspath, JsonConvert.SerializeObject(cells, Formatting.Indented));
            }
        }
        public static Cell[] cells;
    }
}
