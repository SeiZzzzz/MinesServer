﻿using Newtonsoft.Json;

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
                for (int i = 0; i < 126; i++)
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
        public Cell SetCellProp(Cell cell)
        {
            foreach (var i in GetType().GetFields())
            {
                var x = cell.GetType().GetFields().FirstOrDefault(x => i.Name == x.Name);
                if (x != default)
                {
                    var type = i.FieldType;
                    x.SetValue(cell, i.GetValue(this));
                }
            }
            return cell;
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
