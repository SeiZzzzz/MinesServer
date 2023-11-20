using MinesServer.Enums;

namespace MinesServer.GameShit.GUI.Horb.List.Rich
{
    public readonly record struct RichListEntry(RichListEntryType Type, string Label, string Values, string InitialValue, string Action, Button? Button = null, RichCard[] Cards = null)
    {
        public string SerializedLabel => Type switch
        {
            RichListEntryType.Card => string.Join("&", Cards!.Select(x => x.Button.Label)),
            _ => Label
        };

        public string SerializedValue => Type switch
        {
            RichListEntryType.Button => Button!.Value.Label,
            RichListEntryType.Card => string.Join("&", Cards.Select(x => $"{x.ImageURI}%{x.ImageWidth}%{x.ImageHeight}")),
            _ => InitialValue
        };

        public string SerializedAction => Type switch
        {
            RichListEntryType.Button => Button!.Value.ActionFormat,
            RichListEntryType.Card => string.Join("&", Cards!.Select(x => x.Button.ActionFormat)),
            _ => Action
        };

        public static RichListEntry Text(string text) => new()
        {
            Label = text,
            Type = RichListEntryType.Text,
            Values = "",
            Action = "",
            InitialValue = ""
        };

        public static RichListEntry Bool(string label, string id, bool value) => new()
        {
            Label = label,
            Type = RichListEntryType.Boolean,
            Values = "0",
            Action = id,
            InitialValue = value ? "1" : "0"
        };

        public static RichListEntry UInt32(string label, string id, uint value) => new()
        {
            Label = label,
            Type = RichListEntryType.UInt32,
            Values = "0",
            Action = id,
            InitialValue = value.ToString()
        };

        public static RichListEntry DropDown(string label, string id, string[] values, int index)
        {
            if (values.Any(x => x.Contains(':'))) throw new ArgumentException("Ты рукожоп! Нельзя в элементах списка использовать двоеточие. Всё хуйня, переделывай.", nameof(values));
            return new()
            {
                Label = label,
                Type = RichListEntryType.DropDown,
                Values = string.Join("#", values.Select((x, i) => i + ":" + x)),
                Action = id,
                InitialValue = index.ToString()
            };
        }

        public static RichListEntry Fill(string label, string barLabel, int percent, CrystalType crystal, string action100, string action1000, string actionMax)
        {
            if (percent < 0 || percent > 100) throw new ArgumentException("Ты рукожоп! Процент это между 0 и 100. Всё хуйня, переделывай.", nameof(percent));
            return new()
            {
                Label = label,
                Type = RichListEntryType.Fill,
                Values = $"{percent}#{barLabel}#{(int)crystal}#{action100}#{action1000}#{actionMax}",
                Action = "",
                InitialValue = ""
            };
        }

        public static RichListEntry Fill(string label, int current, int max, CrystalType crystal, string action100, string action1000, string actionMax)
        {
            if (max < 0) throw new ArgumentException($"Ты рукожоп! Максимальное число должно быть положительным. Всё хуйня, переделывай.", nameof(max));
            if (current < 0 || current > max) throw new ArgumentException($"Ты рукожоп! Текущее количество это между 0 и {max}. Всё хуйня, переделывай.", nameof(current));
            var per = Math.Round((decimal)current / (max / 100));
            return new()
            {
                Label = label,
                Type = RichListEntryType.Fill,
                Values = $"{per}#{current}/{max}#{(int)crystal}#{action100}#{action1000}#{actionMax}",
                Action = "",
                InitialValue = ""
            };
        }

        public static RichListEntry ButtonLine(string label, Button button) => new()
        {
            Label = label,
            Button = button
        };

        public static RichListEntry CardLine(RichCard[] cards) => new()
        {
            Cards = cards
        };
    }
}
