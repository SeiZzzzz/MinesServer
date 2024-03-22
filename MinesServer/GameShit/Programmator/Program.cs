namespace MinesServer.GameShit.Programmator
{
    public class Program
    {
        private Program()
        {

        }
        public Program(Player P,string name,string data)
        {
            owner = P;
            this.name = name;this.data = data;
        }
        public int id { get; set; }
        public string name { get; set; }
        public string data { get; set; }
        public Player owner { get; set; }
        public Dictionary<string, PFunction> programm
        {
            get
            {
                _programm ??= Parse();
                return _programm;
            }
        }
        private Dictionary<string, PFunction> Parse()
        {
            Dictionary<string, PFunction> functions = new();
            functions[""] = new PFunction();
            string currentFunc = "";
            var index = data.IndexOf("$");
            data = data.Substring(index + 1);
            for (int i = 0; i < data.Length; i++
                )
            {
                int next;
                switch (data[i])
                {
                    case 'w':
                        functions[currentFunc] += new PAction(ActionType.RotateUp);
                        break;
                    case 'a':
                        functions[currentFunc] += new PAction(ActionType.RotateLeft);
                        break;
                    case 's':
                        functions[currentFunc] += new PAction(ActionType.RotateDown);
                        break;
                    case 'd':
                        functions[currentFunc] += new PAction(ActionType.RotateRight);
                        break;
                    case 'z':
                        functions[currentFunc] += new PAction(ActionType.Dig);
                        break;
                    case 'b':
                        functions[currentFunc] += new PAction(ActionType.BuildBlock);
                        break;
                    case 'q':
                        functions[currentFunc] += new PAction(ActionType.BuildPillar);
                        break;
                    case 'r':
                        functions[currentFunc] += new PAction(ActionType.BuildRoad);
                        break;
                    case 'g':
                        functions[currentFunc] += new PAction(ActionType.Geology);
                        break;
                    case 'h':
                        functions[currentFunc] += new PAction(ActionType.Heal);
                        break;
                    case ',':
                        functions[currentFunc] += new PAction(ActionType.NextRow);
                        break;
                    case '?':
                        next = data[(i + 1)..].IndexOf('<');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.RunIfFalse, data[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '!':
                        i++;
                        if (data[i] == '?')
                        {
                            next = data[(i + 1)..].IndexOf('<');
                            if (next != -1)
                            {
                                next++;
                                functions[currentFunc] += new PAction(ActionType.RunIfTrue, data[i..][1..next]);
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = data[i..].IndexOf(']');
                        var option = data[i..][..next];
                        i += next;
                        switch (option)
                        {
                            case "W":
                                functions[currentFunc] += new PAction(ActionType.CheckUp);
                                break;
                            case "A":
                                functions[currentFunc] += new PAction(ActionType.CheckLeft);
                                break;
                            case "S":
                                functions[currentFunc] += new PAction(ActionType.CheckDown);
                                break;
                            case "D":
                                functions[currentFunc] += new PAction(ActionType.CheckRight);
                                break;
                            case "w":
                                functions[currentFunc] += new PAction(ActionType.ShiftUp);
                                break;
                            case "a":
                                functions[currentFunc] += new PAction(ActionType.ShiftLeft);
                                break;
                            case "s":
                                functions[currentFunc] += new PAction(ActionType.ShiftDown);
                                break;
                            case "d":
                                functions[currentFunc] += new PAction(ActionType.ShiftRight);
                                break;
                            case "AS":
                                functions[currentFunc] += new PAction(ActionType.CheckDownLeft);
                                break;
                            case "WA":
                                functions[currentFunc] += new PAction(ActionType.CheckUpLeft);
                                break;
                            case "DW":
                                functions[currentFunc] += new PAction(ActionType.CheckUpRight);
                                break;
                            case "SD":
                                functions[currentFunc] += new PAction(ActionType.CheckDownRight);
                                break;
                            case "F":
                                functions[currentFunc] += new PAction(ActionType.CheckForward);
                                break;
                            case "f":
                                functions[currentFunc] += new PAction(ActionType.ShiftForward);
                                break;
                            case "r":
                                functions[currentFunc] += new PAction(ActionType.CheckRightRelative);
                                break;
                            case "l":
                                functions[currentFunc] += new PAction(ActionType.CheckLeftRelative);
                                break;
                        }

                        break;
                    case '#':
                        i++;
                        switch (data[i])
                        {
                            case 'S':
                                functions[currentFunc] += new PAction(ActionType.Stop);
                                break;
                            case 'E':
                                functions[currentFunc] += new PAction(ActionType.Start);
                                break;
                            case 'R':
                                next = data[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunOnRespawn, data[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (data[i])
                        {
                            case '>':
                                next = data[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunSub, data[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (data[i])
                        {
                            case '>':
                                next = data[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunFunction, data[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (data[i])
                        {
                            case '>':
                                next = data[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunState, data[i..][1..next]);
                                    i += next;
                                }

                                break;
                            case 'n':
                                Console.WriteLine("is not empty");
                                break;
                            case 'e':
                                Console.WriteLine("is empty");
                                break;
                            case 'f':
                                Console.WriteLine("is falling");
                                break;
                            case 'c':
                                Console.WriteLine("is crystal");
                                break;
                            case 'a':
                                Console.WriteLine("is living crystal");
                                break;
                            case 'b':
                                Console.WriteLine("is boulder");
                                break;
                            case 's':
                                Console.WriteLine("is sand");
                                break;
                            case 'k':
                                Console.WriteLine("is breakablerock");
                                break;
                            case 'd':
                                Console.WriteLine("is unbreakablerock");
                                break;
                            case 'A':
                                Console.WriteLine("is acid");
                                break;
                            case 'B':
                                Console.WriteLine("is redrock");
                                break;
                            case 'K':
                                Console.WriteLine("is blackrock");
                                break;
                            case 'g':
                                Console.WriteLine("is green block");
                                break;
                            case 'y':
                                Console.WriteLine("is yellow block");
                                break;
                            case 'r':
                                Console.WriteLine("is red block");
                                break;
                            case 'o':
                                Console.WriteLine("is pillar");
                                break;
                            case 'q':
                                Console.WriteLine("is quadro");
                                break;
                            case 'R':
                                Console.WriteLine("is road");
                                break;
                            case 'x':
                                Console.WriteLine("is box");
                                break;
                        }

                        break;
                    case '>':
                        next = data[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.GoTo, data[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = data[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            currentFunc = data[i..][1..next];
                            functions[currentFunc] = new PFunction();
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (data[i])
                        {
                            case '|':
                                functions[currentFunc] += new PAction(ActionType.Return);
                                break;
                            case '-':
                                i++;
                                if (data[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (data[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnState);
                                }

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (data[i])
                        {
                            case 'W':
                                functions[currentFunc] += new PAction(ActionType.MoveUp);
                                break;
                            case 'A':
                                functions[currentFunc] += new PAction(ActionType.MoveLeft);
                                break;
                            case 'S':
                                functions[currentFunc] += new PAction(ActionType.MoveDown);
                                break;
                            case 'D':
                                functions[currentFunc] += new PAction(ActionType.MoveRight);
                                break;
                            case 'F':
                                functions[currentFunc] += new PAction(ActionType.MoveForward);
                                break;
                        }

                        break;
                    default:
                        var currentdata = data[i..];
                        if (currentdata.StartsWith("CCW;"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.RotateLeftRelative);
                        }
                        else if (currentdata.StartsWith("CW;"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.RotateRightRelative);
                        }
                        else if (currentdata.StartsWith("RAND;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.RotateRandom);
                        }
                        else if (currentdata.StartsWith("VB;"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.BuildMilitaryBlock);
                        }
                        else if (currentdata.StartsWith("DIGG;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosDig);
                        }
                        else if (currentdata.StartsWith("BUILD;"))
                        {
                            i += 5;
                            functions[currentFunc] += new PAction(ActionType.MacrosBuild);
                        }
                        else if (currentdata.StartsWith("HEAL;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosHeal);
                        }
                        else if (currentdata.StartsWith("MINE;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosMine);
                        }
                        else if (currentdata.StartsWith("FLIP;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.Flip);
                        }
                        else if (currentdata.StartsWith("BEEP;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.Beep);
                        }
                        else if (currentdata.StartsWith("OR"))
                        {
                            i += 1;
                            functions[currentFunc] += new PAction(ActionType.Or);
                        }
                        else if (currentdata.StartsWith("AND"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.And);
                        }
                        else if (currentdata.StartsWith("AUT+"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.EnableAutoDig);
                        }
                        else if (currentdata.StartsWith("AUT-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.DisableAutoDig);
                        }
                        else if (currentdata.StartsWith("ARG+"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.EnableAgression);
                        }
                        else if (currentdata.StartsWith("ARG-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.DisableAgression);
                        }
                        else if (currentdata.StartsWith("=hp-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.IsHpLower100);
                        }
                        else if (currentdata.StartsWith("=hp50"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.IsHpLower50);
                        }
                        break;
                }
            }
            return functions;
        }
        private Dictionary<string, PFunction> _programm;
    }
}
