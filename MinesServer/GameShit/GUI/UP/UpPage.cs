using MinesServer.Enums;
using MinesServer.Network;
using System.Text.RegularExpressions;

namespace MinesServer.GameShit.GUI.UP
{
    public readonly record struct UpPage(
        Dictionary<SkillType, bool>? SkillsToInstall,
        UpSkill[] Skills,
        int SlotAmount,
        int? SelectedSlot,
        Button? Button,
        string? Title,
        string? Text,
        Action? OnAdmin,
        Action<int>? OnDelete,
        Action<int>? OnSkill,
        Action<int, SkillType>? OnInstall,
        SkillType? SkillIcon
        ) : IPage
    {
        public bool ProcessButton(string action)
        {
            if (Button?.ProcessButton(action) == true)
                return true;

            if (OnDelete is not null)
            {
                var match = Regex.Match(action, @"^delete:(\d+)$");
                if (match.Success)
                {
                    var slot = int.Parse(match.Groups[1].Value);
                    if (slot > SlotAmount) throw new InvalidPayloadException("Tried to delete a skill from non-existent slot");
                    if (slot != SelectedSlot) throw new InvalidPayloadException("Tried to delete a skill from a slot that is not currently selected");
                    OnDelete(slot);
                    return true;
                }
            }

            if (OnSkill is not null)
            {
                var match = Regex.Match(action, @"^skill:(\d+)$");
                if (match.Success)
                {
                    var slot = int.Parse(match.Groups[1].Value);
                    if (slot > SlotAmount) throw new InvalidPayloadException("Tried to select a non-existent slot");
                    OnSkill(slot);
                    return true;
                }
            }

            if (OnInstall is not null)
            {
                var match = Regex.Match(action, @"^install:(.+)#(\d+)$");
                if (match.Success)
                {
                    var slot = int.Parse(match.Groups[2].Value);
                    if (slot > SlotAmount) throw new InvalidPayloadException("Tried to install a skill into non-existent slot");
                    if (slot != SelectedSlot) throw new InvalidPayloadException("Tried to install a skill into a slot that is not currently selected");
                    var skill = Mines3Enums.SkillFromCode(match.Groups[1].Value);
                    if (skill == SkillType.Unknown) throw new InvalidPayloadException("Invalid skill type: " + match.Groups[1].Value);
                    OnInstall(slot, skill);
                    return true;
                }
            }

            return false;
        }
    }
}
