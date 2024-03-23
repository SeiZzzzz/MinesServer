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
            var context = data.Substring(index + 1);
            for (int i = 0; i < context.Length; i++
                )
            {
                int next;
                switch (context[i])
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
                        next = context[(i + 1)..].IndexOf('<');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.RunIfFalse, context[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '!':
                        i++;
                        if (context[i] == '?')
                        {
                            next = context[(i + 1)..].IndexOf('<');
                            if (next != -1)
                            {
                                next++;
                                functions[currentFunc] += new PAction(ActionType.RunIfTrue, context[i..][1..next]);
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = context[i..].IndexOf(']');
                        var option = context[i..][..next];
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
                        switch (context[i])
                        {
                            case 'S':
                                functions[currentFunc] += new PAction(ActionType.Stop);
                                break;
                            case 'E':
                                functions[currentFunc] += new PAction(ActionType.Start);
                                break;
                            case 'R':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunOnRespawn, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunSub, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunFunction, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunState, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                            case 'n':
                                functions[currentFunc] += new PAction(ActionType.IsNotEmpty);
                                break;
                            case 'e':
                                functions[currentFunc] += new PAction(ActionType.IsEmpty);
                                break;
                            case 'f':
                                functions[currentFunc] += new PAction(ActionType.IsFalling);
                                break;
                            case 'c':
                                functions[currentFunc] += new PAction(ActionType.IsCrystal);
                                break;
                            case 'a':
                                functions[currentFunc] += new PAction(ActionType.IsLivingCrystal);
                                break;
                            case 'b':
                                functions[currentFunc] += new PAction(ActionType.IsBoulder);
                                break;
                            case 's':
                                functions[currentFunc] += new PAction(ActionType.IsSand);
                                break;
                            case 'k':
                                functions[currentFunc] += new PAction(ActionType.IsBreakableRock);
                                break;
                            case 'd':
                                functions[currentFunc] += new PAction(ActionType.IsUnbreakable);
                                break;
                            case 'A':
                                functions[currentFunc] += new PAction(ActionType.IsAcid);
                                break;
                            case 'B':
                                functions[currentFunc] += new PAction(ActionType.IsRedRock);
                                break;
                            case 'K':
                                functions[currentFunc] += new PAction(ActionType.IsBlackRock);
                                break;
                            case 'g':
                                functions[currentFunc] += new PAction(ActionType.IsGreenBlock);
                                break;
                            case 'y':
                                functions[currentFunc] += new PAction(ActionType.IsYellowBlock);
                                break;
                            case 'r':
                                functions[currentFunc] += new PAction(ActionType.IsRedRock);
                                break;
                            case 'o':
                                functions[currentFunc] += new PAction(ActionType.IsPillar);
                                break;
                            case 'q':
                                functions[currentFunc] += new PAction(ActionType.IsQuadBlock);
                                break;
                            case 'R':
                                functions[currentFunc] += new PAction(ActionType.IsRoad);
                                break;
                            case 'x':
                                functions[currentFunc] += new PAction(ActionType.IsBox);
                                break;
                        }

                        break;
                    case '>':
                        next = context[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.GoTo, context[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = context[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            currentFunc = context[i..][1..next];
                            functions[currentFunc] = new PFunction();
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (context[i])
                        {
                            case '|':
                                functions[currentFunc] += new PAction(ActionType.Return);
                                break;
                            case '-':
                                i++;
                                if (context[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (context[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnState);
                                }

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (context[i])
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
                        var currentdata = context[i..];
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
