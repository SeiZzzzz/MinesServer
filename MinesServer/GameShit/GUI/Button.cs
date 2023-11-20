using System.Text.RegularExpressions;

namespace MinesServer.GameShit.GUI
{
    public readonly record struct Button(string Label, string ActionFormat, Action<ActionArgs> Handler = null)
    {
        private static readonly string[] _macros = new string[] { ActionMacros.Input, ActionMacros.RichList, ActionMacros.PaintGrid, ActionMacros.CrystalSliders };

        public bool ProcessButton(string action)
        {
            var pattern = ActionFormat;
            foreach (var macro in _macros)
                pattern = pattern.Replace(macro, "(.*)");
            pattern = $"^{pattern}$";
            var match = Regex.Match(action, pattern);
            if (match.Success)
            {
                var temp = ActionFormat;
                var output = "";
                var i = temp.Length;
                while ((i = _macros.Min(x =>
                {
                    var y = temp.IndexOf(x);
                    return y == -1 ? temp.Length : y;
                })) != temp.Length)
                {
                    output += temp[i..(i + 3)];
                    temp = temp[(i + 3)..];
                }
                var args = new ActionArgs();
                var ind = 1;
                while (output.Length > 0)
                {
                    switch (output[..3])
                    {
                        case ActionMacros.Input:
                            args.Input = match.Groups[ind++].Value;
                            break;
                        case ActionMacros.CrystalSliders:
                            args.CrystalSliders = match.Groups[ind++].Value.Split(':').Select(long.Parse).ToArray();
                            break;
                        case ActionMacros.PaintGrid:
                            args.PaintGrid = match.Groups[ind++].Value.Select(x => x != '0').ToArray();
                            break;
                        case ActionMacros.RichList:
                            args.RichList = match.Groups[ind++].Value.Split('#').Select(x => x.Split(':')).ToDictionary(x => x[0], x => x[1]);
                            break;
                        default: throw new ArgumentOutOfRangeException("macros", $"После добавления макроса в {nameof(_macros)} нужно добавить его и в этот свитч тоже. Переделывай :)");
                    }
                    output = output[3..];
                }
                Handler?.Invoke(args);
                return true;
            }

            return false;
        }
    }
}
