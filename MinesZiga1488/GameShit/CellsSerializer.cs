using MinesServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit
{
    public static class CellsSerializer
    {
        public static void Load()
        {
            var cellspath = "cells.json";
            if (File.Exists(cellspath))
            {
                cells = JsonConvert.DeserializeObject<ShadowCell[]>(File.ReadAllText(cellspath));
            }
            else
            {
                cells = new ShadowCell[126];
                for (int i = 0; i < 126;i++)
                {
                    cells[i] = new ShadowCell((byte)i);
                }
                File.WriteAllText(cellspath, JsonConvert.SerializeObject(cells, Formatting.Indented));
            }
        }
        public static ShadowCell[] cells;
    }
    public class ShadowCell
    {
        public ShadowCell(byte type)
        {
            name = "";
            this.type = type;
        }
        public void CreateNormalCell(Cell cell)
        {
            foreach (var i in GetType().GetFields())
            {
                var x = cell.GetType().GetFields().FirstOrDefault(x => i.Name == x.Name);
                if (x != default)
                {
                    var type = i.FieldType;
                    x.SetValue(cell,i.GetValue(this));
                }
            }
        }
        public bool isEmpty;
        public string name;
        public bool isFallable;
        public bool isCry;
        public int durability;
        public int damage;
        public byte type;
    }
}
