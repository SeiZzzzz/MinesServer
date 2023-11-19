using System;

namespace MinesServer.Enums
{
    public static class Mines3Enums
    {
        static readonly string[] crystalCodes = new[] { "g", "b", "r", "v", "w", "c" };
        
        static readonly string[] skillCodes = new[] { 
            "a",  "k",  "j",  "U",  "B",  "G",  "D",  "x",  "y", "z",   // 0 - 9
            "u",  "E",  "d",  "l",  "m",  "R",  "L",  "Q",  "q", "M",   // 10 - 19
            "Y",  "P",  "F",  "C",  "t",  "*U", "Z",  "h",  "V", "p",   // 20 - 29
            "b",  "c",  "v",  "*M", "J",  "S",  "X",  "W",  "r", "w",   // 30 - 39
            "g",  "o",  "e",  "*D", "i",  "f",  "H",  "O",  "A", "*B",  // 40 - 49
            "*L", "*A", "*T", "*u", "*J", "*I", "*a", "*d", "*g"        // 50 - 58
        };
        
        static readonly string[] itemCodes = new[] { "" };

        public static string GetCode(this CrystalType crystal) => crystal == CrystalType.Unknown ? "" : crystalCodes[(int)crystal];

        public static CrystalType CrystalFromCode(string code) => (CrystalType)Array.IndexOf(crystalCodes, code);

        public static string GetCode(this SkillType skill) => skill == SkillType.Unknown ? "" : skillCodes[(int)skill];

        public static SkillType SkillFromCode(string code) => (SkillType)Array.IndexOf(skillCodes, code);
    }
}
