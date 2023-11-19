using MinesServer.Enums;

namespace MinesServer.GameShit.GUI.UP
{
    public readonly record struct UpSkill(int Slot, int Level, bool CanUpgrade, SkillType Type = SkillType.Unknown);
}
