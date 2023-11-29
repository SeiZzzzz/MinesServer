using MinesServer.Enums;

namespace MinesServer.GameShit.GUI.Horb
{
    public readonly record struct InventoryItem(int Id, string UpText, string DownText, uint? Amount, bool Faint, InventoryTextColor UpTextColor, InventoryTextColor DownTextColor)
    {
        public static InventoryItem Item(int code, string upText, string downText, bool faint = false, InventoryTextColor upTextColor = InventoryTextColor.Default, InventoryTextColor downTextColor = InventoryTextColor.Default) => new()
        {
            Id = code,
            UpText = upText,
            DownText = downText,
            Faint = faint,
            UpTextColor = upTextColor,
            DownTextColor = downTextColor
        };

        public static InventoryItem Item(int code, uint amount, bool faint = false) => new()
        {
            Id = code,
            Amount = amount,
            Faint = faint
        };

        public static InventoryItem Clan(byte code, string upText, string downText, bool faint = false, InventoryTextColor upTextColor = InventoryTextColor.Default, InventoryTextColor downTextColor = InventoryTextColor.Default) => new()
        {
            Id = 200 + code,
            UpText = upText,
            DownText = downText,
            Faint = faint,
            UpTextColor = upTextColor,
            DownTextColor = downTextColor
        };

        public static InventoryItem Clan(byte code, uint amount, bool faint = false) => new()
        {
            Id = 200 + code,
            Amount = amount,
            Faint = faint
        };

        public static InventoryItem Skill(SkillType skill, string upText, string downText, bool faint = false, InventoryTextColor upTextColor = InventoryTextColor.Default, InventoryTextColor downTextColor = InventoryTextColor.Default) => new()
        {
            Id = 2000 + (int)skill,
            UpText = upText,
            DownText = downText,
            Faint = faint,
            UpTextColor = upTextColor,
            DownTextColor = downTextColor
        };

        public static InventoryItem Skill(SkillType skill, uint amount, bool faint = false) => new()
        {
            Id = 2000 + (int)skill,
            Amount = amount,
            Faint = faint
        };

        public static InventoryItem Empty() => new()
        {
            Id = -1
        };
    }
}
