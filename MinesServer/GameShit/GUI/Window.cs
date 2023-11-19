using MinesServer.Enums;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.GUI.UP;
using MinesServer.Network;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinesServer.GameShit.GUI
{
    public class Window
    {
        Tab[] _tabs;

        public string Title { get; init; } = "!!!!! NO TITLE !!!!!";

        public Tab CurrentTab { get; private set; }

        public required Tab[] Tabs
        {
            get => _tabs;
            init
            {
                _tabs = value;
                CurrentTab = _tabs[0];
            }
        }

        public bool ShowTabs { get; init; } = true;

        public void AdminButton()
        {
            var act = CurrentTab.History.Peek().OnAdmin ?? throw new InvalidPayloadException("Admin button is not supported in this state");
            act();
        }

        public bool ProcessButton(string action)
        {
            if (action is "exit" or "exit:0")
                throw new InvalidOperationException("Exit action is not supported here. Handle this action in the packet layer.");

            // Open tab
            var targetTab = FindTab(action);
            if (targetTab is not null)
            {
                if (targetTab.IsHidden)
                    throw new InvalidPayloadException("Tried to open a hidden tab");
                CurrentTab = targetTab;
                CurrentTab.ClearHistory();
                return true;
            }

            // Pass the action to the current tab
            return CurrentTab.ProcessButton(action);
        }

        public void OpenTab(string tabAction)
        {
            var targetTab = FindTab(tabAction) ?? throw new ArgumentException("Нет такой вкладки. Смотри что пишешь. Переделывай :)");
            CurrentTab = targetTab;
            CurrentTab.ClearHistory();
        }

        public Tab? FindTab(string tabAction) => Tabs.FirstOrDefault(x => x.Action == tabAction);

        public override string ToString()
        {
            if (CurrentTab.InitialPage is null)
                throw new ArgumentNullException(nameof(CurrentTab.InitialPage), "Ты забыл указать страницу для вкладки. Переделывай :)");
            var obj = new JSONObject();
            var page = CurrentTab.History.Peek();
            obj["title"] = page.Title ?? CurrentTab.Title ?? Title;
            if (ShowTabs)
            {
                obj["tabs"] = new JSONArray();
                foreach (var t in Tabs)
                    if (CurrentTab == t)
                    {
                        obj["tabs"].Add(t.Label);
                        obj["tabs"].Add("");
                    }
                    else if (!t.IsHidden)
                    {
                        obj["tabs"].Add(t.Label);
                        obj["tabs"].Add(t.Action);
                    }
            }
            obj["buttons"] = new JSONArray();
            obj["buttons"].Add("ВЫЙТИ");
            obj["buttons"].Add("exit");
            if (CurrentTab.History.Count <= 1) obj["back"] = false;
            else
            {
                obj["buttons"].Add("НАЗАД");
                obj["buttons"].Add("<" + CurrentTab.Action);
            }
            if (page.OnAdmin is not null) obj["admin"] = true;
            if (page is Page horb)
            {
                foreach (var b in horb.Buttons)
                {
                    obj["buttons"].Add(b.Label);
                    obj["buttons"].Add(b.ActionFormat);
                }
                if (horb.Text is not null) obj["text"] = horb.Text;
                if (horb.CrystalConfig is not null)
                {
                    obj["crys_left"] = horb.CrystalConfig?.LeftText;
                    obj["crys_right"] = horb.CrystalConfig?.RightText;
                    if (horb.CrystalConfig.Value.BuyMode) obj["crys_buy"] = true;
                    obj["crys_lines"] = new JSONArray();
                    foreach (var c in horb.CrystalConfig.Value.Lines)
                        obj["crys_lines"].Add($"{c.LeftMin}:{c.RightMin}:{c.Denominator}:{c.CurrentValue}:{c.Label}");
                }
                if (horb.List is not null)
                {
                    obj["list"] = new JSONArray();
                    foreach (var i in horb.List!)
                    {
                        obj["list"].Add(i.Label);
                        obj["list"].Add(i.Button?.Label);
                        obj["list"].Add(i.Button?.ActionFormat);
                    }
                }
                if (horb.RichList is not null)
                {
                    if (horb.RichList!.Value.NoScroll) obj["rich_no_scroll"] = true;
                    obj["rich_list"] = new JSONArray();
                    foreach (var i in horb.RichList!.Value.Entries)
                    {
                        obj["rich_list"].Add(i.SerializedLabel);
                        obj["rich_list"].Add(i.Type switch
                        {
                            RichListEntryType.Text => "text",
                            RichListEntryType.Boolean => "bool",
                            RichListEntryType.UInt32 => "uint",
                            RichListEntryType.DropDown => "drop",
                            RichListEntryType.Fill => "fill",
                            RichListEntryType.Button => "button",
                            RichListEntryType.Card => "3card",
                            _ => ""
                        });
                        obj["rich_list"].Add(i.Values);
                        obj["rich_list"].Add(i.SerializedAction);
                        obj["rich_list"].Add(i.SerializedValue);
                    }
                }
                if (horb.ClanList is not null)
                {
                    obj["clanlist"] = new JSONArray();
                    foreach (var i in horb.ClanList!)
                    {
                        obj["clanlist"].Add(i.ClanId);
                        obj["clanlist"].Add(i.Button.Label);
                        obj["clanlist"].Add(i.RightText);
                        obj["clanlist"].Add(i.Button.ActionFormat);
                    }
                }
                if (horb.Input is not null)
                {
                    if (!string.IsNullOrEmpty(horb.Input!.Value.Placeholder)) obj["input_place"] = horb.Input!.Value.Placeholder!;
                    else obj["input_place"] = " ";
                    if (horb.Input!.Value.MaxLength is not null) obj["input_len"] = horb.Input!.Value.MaxLength!;
                    if (horb.Input!.Value.IsConsole) obj["input_console"] = true;
                }
                if (horb.Style is not null)
                {
                    var style = horb.Style!.Value;
                    var elements = new List<string>();
                    if (style.Canvas.CellHeight is not null) elements.Add("canv-ch=" + style.Canvas.CellHeight);
                    if (style.Canvas.Height is not null) elements.Add("canv-h=" + style.Canvas.Height);
                    if (style.Canvas.Width is not null) elements.Add("canv-w=" + style.Canvas.Width);
                    if (style.Inventory.CellHeight is not null) elements.Add("inv-ch=" + style.Inventory.CellHeight);
                    if (style.Inventory.Height is not null) elements.Add("inv-h=" + style.Inventory.Height);
                    if (style.Inventory.Width is not null) elements.Add("inv-w=" + style.Inventory.Width);
                    if (style.FixScrollTag is not null) elements.Add("fixScroll=" + style.FixScrollTag);
                    if (style.InventoryButtonAction is not null) elements.Add("invButton=" + style.InventoryButtonAction);
                    if (style.ScrollHeight is not null) elements.Add("scrollh=" + style.ScrollHeight);
                    if (style.Space is not null) elements.Add("space=" + style.Space);
                    if (style.LargeInput) elements.Add("biginput=1");
                    if (style.DisableKeyboard) elements.Add("keysOff=1");
                    obj["css"] = string.Join(";", elements);
                }
                if (horb.Card is not null)
                {
                    var card = horb.Card!.Value;
                    obj["card"] = $"{card.ImageType switch
                    {
                        CardImageType.Skill => "s",
                        CardImageType.Item => "i",
                        CardImageType.Clan => "c",
                        CardImageType.WebImage => "w",
                        _ => ""
                    }}{card.ImageURI}:{card.Text}";
                }
                if (horb.Canvas is not null)
                {
                    obj["canvas"] = new JSONArray();
                    foreach (var i in horb.Canvas!)
                    {
                        StringBuilder content = new();
                        if (i.Alpha is not null) content.Append(i.Alpha + "A");
                        if (i.IsBlinking) content.Append('B');
                        if (i.SizeX is not null) content.Append(i.SizeX + "L");
                        if (i.OffsetX is not null) content.Append(i.OffsetX + "X");
                        if (i.OffsetY is not null) content.Append(i.OffsetY + "Y");
                        if (i.Pivot != CanvasElementPivot.Default) content.Append(i.Pivot == CanvasElementPivot.Right ? "r" : "l");
                        if (i.Height is not null) content.Append(i.Height + "h");
                        if (i.Width is not null) content.Append(i.Width + "w");
                        if (i.OriginX is not null) content.Append(i.OriginX + "x");
                        if (i.OriginY is not null) content.Append(i.OriginY + "y");
                        content.Append($"={i.Type switch
                        {
                            CanvasElementType.Image => "I",
                            CanvasElementType.Button => "B",
                            CanvasElementType.Rect => "R",
                            CanvasElementType.Line => "L",
                            CanvasElementType.TextField => "T",
                            CanvasElementType.TPButton => "t",
                            CanvasElementType.MicroButton => "b",
                            _ => ""
                        }}#{i.Content?.Label}");
                        obj["canvas"].Add(content.ToString());
                        if (i.Type is CanvasElementType.Button or CanvasElementType.TPButton or CanvasElementType.MicroButton) obj["canvas"].Add(i.Content?.ActionFormat);
                    }
                }
                return "horb:" + obj.ToString();
            }
            else if (page is UpPage up)
            {
                if (up.Text is not null) obj["txt"] = up.Text;
                obj["k"] = string.Join("#", up.Skills.Select(x => $"{x.Type.GetCode()}:{x.Level}:{x.Slot}:{(x.CanUpgrade ? "1" : "0")}"));
                obj["s"] = up.SlotAmount;
                if (up.Button is not null)
                {
                    obj["b"] = up.Button!.Value.Label;
                    obj["ba"] = up.Button!.Value.ActionFormat;
                }
                if (up.SkillsToInstall is not null) obj["i"] = string.Join(":", up.SkillsToInstall!.Select(x => (x.Value ? "" : "_") + x.Key));
                if (up.OnDelete is not null) obj["del"] = 1;
                if (up.SelectedSlot is not null) obj["sl"] = up.SelectedSlot!.Value;
                else obj["sl"] = -1;
                obj["si"] = up.SkillIcon.GetCode();
                return "up:" + obj.ToString();
            }
            return "unknown-report-this-to-the-developer:" + obj.ToString();
        }
    }
}
