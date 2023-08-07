using MinesServer.Server;
using Newtonsoft.Json;
using System.Dynamic;

namespace MinesServer.GameShit.GUI
{
    public class Builder
    {
        public dynamic _horb = new ExpandoObject();
        public void Send(Session s)
        {
            s.Send("GU", $"horb:{JsonConvert.SerializeObject(_horb, Formatting.None)}");
        }
        public Builder AddButton(string text, string command)
        {
            if (!FieldExists("buttons"))
            {
                _horb.buttons = new List<string>();
            }

            _horb.buttons.Add(text);
            _horb.buttons.Add(command);
            return this;
        }
        public Builder AddTab(string text, string command)
        {
            if (!FieldExists("tabs"))
            {
                _horb.tabs = new List<string>();
            }

            _horb.tabs.Add(text);
            _horb.tabs.Add(command);
            return this;
        }
        public Builder Admin()
        {
            _horb.admin = true;
            return this;
        }
        public Builder SetTitle(string text)
        {
            _horb.title = text;
            return this;
        }
        public Builder AddTextLine(string text)
        {
            if (!FieldExists("text"))
            {
                _horb.text = text + '\n';
                return this;
            }
            _horb.text += text + '\n';
            return this;
        }
        public Builder AddTextLines(params string[] textLines)
        {
            foreach (var textLine in textLines)
            {
                AddTextLine(textLine);
            }

            return this;
        }
        public Builder AddIConsole()
        {
            _horb.input_console = true;
            return this;
        }
        public Builder AddIConsolePlace(string text)
        {
            _horb.input_place = text;
            return this;
        }
        public Builder AddCrysRight(string text)
        {
            _horb.crys_right = text;
            return this;
        }

        public Builder AddCrysLeft(string text)
        {
            _horb.crys_left = text;
            return this;
        }

        public Builder AddCrysBuy()
        {
            _horb.crys_buy = true;
            return this;
        }
        private bool FieldExists(string field)
        {
            return (_horb as IDictionary<string, object>).ContainsKey(field);
        }
    }
}

