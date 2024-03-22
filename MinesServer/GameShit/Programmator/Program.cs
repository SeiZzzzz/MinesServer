
using MinesServer.GameShit.prog2try;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace MinesServer.GameShit.Programmator
{
    public class Program
    {
        private Program() { }
        public void Print()
        {
            Dictionary<string,IFunction> functions = new();
            functions["main"] = new BaseFunction();
            string currentFunc = "main";
            var index = progtext.IndexOf("$");
            progtext = progtext.Substring(index + 1);
            for(int i = 0;i < progtext.Length; i++
                )
            {
                Console.WriteLine("starting main");
                int next;
                switch (progtext[i])
                {
                    case 'w':
                        functions[currentFunc] += new PAction(ActionType.RotateUp);
                        break;
                    case 'a':
                        functions[currentFunc] += new PAction(ActionType.RotateLeft);
                        break;
                    case 's':
                        Console.WriteLine("rotatedown");
                        break;
                    case 'd':
                        Console.WriteLine("rotateright");
                        break;
                    case 'z':
                        Console.WriteLine("dig");
                        break;
                    case 'b':
                        Console.WriteLine("buildblock");
                        break;
                    case 'q':
                        Console.WriteLine("buildpillar");
                        break;
                    case 'r':
                        Console.WriteLine("buildRoad");
                        break;
                    case 'g':
                        Console.WriteLine("geology");
                        break;
                    case 'h':
                        Console.WriteLine("heal");
                        break;
                    case ',':
                        Console.WriteLine("nextrow");
                        break;
                    case '?':
                        next = progtext[(i + 1)..].IndexOf('<');
                        if (next != -1)
                        {
                            next++;
                            Console.WriteLine($"runiffalse{progtext[i..][1..next]}");
                            i += next;
                        }

                        break;
                    case '!':
                        i++;
                        if (progtext[i] == '?')
                        {
                            next = progtext[(i + 1)..].IndexOf('<');
                            if (next != -1)
                            {
                                next++;
                                Console.WriteLine($"runiftrue{progtext[i..][1..next]}");
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = progtext[i..].IndexOf(']');
                        var option = progtext[i..][..next];
                        i += next;
                        switch (option)
                        {
                            case "W":
                                Console.WriteLine($"checkup");
                                break;
                            case "A":
                                Console.WriteLine($"checkleft");
                                break;
                            case "S":
                                Console.WriteLine($"checkdown");
                                break;
                            case "D":
                                Console.WriteLine($"checkright");
                                break;
                            case "w":
                                Console.WriteLine($"shiftup");
                                break;
                            case "a":
                                Console.WriteLine($"shiftleft");
                                break;
                            case "s":
                                Console.WriteLine($"shiftdown");
                                break;
                            case "d":
                                Console.WriteLine($"shiftright");
                                break;
                            case "AS":
                                Console.WriteLine($"check down left");
                                break;
                            case "WA":
                                Console.WriteLine($"check up left");
                                break;
                            case "DW":
                                Console.WriteLine($"check up right");
                                break;
                            case "SD":
                                Console.WriteLine($"check down right");
                                break;
                            case "F":
                                Console.WriteLine($"check forward");
                                break;
                            case "f":
                                Console.WriteLine($"shift forward");
                                break;
                            case "r":
                                Console.WriteLine($"check right relative");
                                break;
                            case "l":
                                Console.WriteLine($"check left relative");
                                break;
                        }

                        break;
                    case '#':
                        i++;
                        switch (progtext[i])
                        {
                            case 'S':
                                Console.WriteLine("start");
                                break;
                            case 'E':
                                Console.WriteLine("stop");
                                break;
                            case 'R':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    Console.WriteLine($"runonrespawn {progtext[i..][1..next]}");
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    Console.WriteLine($"runSub {progtext[i..][1..next]}");
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    Console.WriteLine($"runFunction {progtext[i..][1..next]}");
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    Console.WriteLine($"RunState {progtext[i..][1..next]}");
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
                        next = progtext[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.GoTo, progtext[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = progtext[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.CreateFunction, progtext[i..][1..next]);
                            currentFunc = progtext[i..][1..next];
                            functions[currentFunc] = new BaseFunction();
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (progtext[i])
                        {
                            case '|':
                                functions[currentFunc] += new PAction(ActionType.Return);
                                break;
                            case '-':
                                i++;
                                if (progtext[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (progtext[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnState);
                                }

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (progtext[i])
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
                        var currentprogtext = progtext[i..];
                        if (currentprogtext.StartsWith("CCW;"))
                        {
                            i += 3;
                            Console.WriteLine("rotate left relative");
                        }
                        else if (currentprogtext.StartsWith("CW;"))
                        {
                            i += 2;
                            Console.WriteLine("rotate right relative");
                        }
                        else if (currentprogtext.StartsWith("RAND;"))
                        {
                            i += 4;
                            Console.WriteLine("rotate random");
                        }
                        else if (currentprogtext.StartsWith("VB;"))
                        {
                            i += 2;
                            Console.WriteLine("build VB");
                        }
                        else if (currentprogtext.StartsWith("DIGG;"))
                        {
                            i += 4;
                            Console.WriteLine("macros DIG");
                        }
                        else if (currentprogtext.StartsWith("BUILD;"))
                        {
                            i += 5;
                            Console.WriteLine("macros BUILD");
                        }
                        else if (currentprogtext.StartsWith("HEAL;"))
                        {
                            i += 4;
                            Console.WriteLine("macros HEAL");
                        }
                        else if (currentprogtext.StartsWith("MINE;"))
                        {
                            i += 4;
                            Console.WriteLine("macros MINE");
                        }
                        else if (currentprogtext.StartsWith("FLIP;"))
                        {
                            i += 4;
                            Console.WriteLine("macros FLIP");
                        }
                        else if (currentprogtext.StartsWith("BEEP;"))
                        {
                            i += 4;
                            Console.WriteLine("BEEP");
                        }
                        else if (currentprogtext.StartsWith("OR"))
                        {
                            i += 4;
                            Console.WriteLine("OR");
                        }
                        else if (currentprogtext.StartsWith("AND"))
                        {
                            i += 4;
                            Console.WriteLine("AND");
                        }
                        else if (currentprogtext.StartsWith("AUT+"))
                        {
                            i += 4;
                            Console.WriteLine("AUTO DIG ENABLE");
                        }
                        else if (currentprogtext.StartsWith("AUT-"))
                        {
                            i += 4;
                            Console.WriteLine("AUTO DIG DISABLE");
                        }
                        else if (currentprogtext.StartsWith("ARG+"))
                        {
                            i += 4;
                            Console.WriteLine("AGR ENABLE");
                        }
                        else if (currentprogtext.StartsWith("ARG-"))
                        {
                            i += 4;
                            Console.WriteLine("AGR DISABLE");
                        }
                        else if (currentprogtext.StartsWith("=hp-"))
                        {
                            i += 3;
                            Console.WriteLine("is hp lower 100");
                        }
                        else if (currentprogtext.StartsWith("=hp50"))
                        {
                            i += 4;
                            Console.WriteLine("is hp lower 50");
                        }

                        break;
                }
            }
            Console.WriteLine("function list");
            foreach (var i in functions)
            {
                Console.WriteLine($"[FUNC] {i.Key}");
                Console.WriteLine($"[content] {string.Join(" ",i.Value.actions.Select(i => i.type.ToString()))}");
            }
        }
        public void Decode()
        {
            Print();
            prog = new();
            var index = progtext.IndexOf("$");
            progtext = progtext.Substring(index + 1);
            int x = 0, y = 0, page = 0;
            for (var i = 0; i < progtext.Length; i++)
            {
                int next;
                switch (progtext[i])
                {
                    case 'w':
                        prog[x, y, page] = new ProgAction(ActionType.RotateUp);
                        break;
                    case 'a':
                        prog[x, y, page] = new ProgAction(ActionType.RotateLeft);
                        break;
                    case 's':
                        prog[x, y, page] = new ProgAction(ActionType.RotateDown);
                        break;
                    case 'd':
                        prog[x, y, page] = new ProgAction(ActionType.RotateRight);
                        break;
                    case 'z':
                        prog[x, y, page] = new ProgAction(ActionType.Dig);
                        break;
                    case 'b':
                        prog[x, y, page] = new ProgAction(ActionType.BuildBlock);
                        break;
                    case 'q':
                        prog[x, y, page] = new ProgAction(ActionType.BuildPillar);
                        break;
                    case 'r':
                        prog[x, y, page] = new ProgAction(ActionType.BuildRoad);
                        break;
                    case 'g':
                        prog[x, y, page] = new ProgAction(ActionType.Geology);
                        break;
                    case 'h':
                        prog[x, y, page] = new ProgAction(ActionType.Heal);
                        break;
                    case ',':
                        prog[x, y, page] = new ProgAction(ActionType.NextRow);
                        break;
                    case '?':
                        next = progtext[(i + 1)..].IndexOf('<');
                        if (next != -1)
                        {
                            next++;
                            prog[x, y, page] = new ProgAction(ActionType.RunIfFalse,
                                progtext[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '!':
                        i++;
                        if (progtext[i] == '?')
                        {
                            next = progtext[(i + 1)..].IndexOf('<');
                            if (next != -1)
                            {
                                next++;
                                prog[x, y, page] = new ProgAction(ActionType.RunIfTrue,
                                    progtext[i..][1..next]);
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = progtext[i..].IndexOf(']');
                        var option = progtext[i..][..next];
                        i += next;
                        switch (option)
                        {
                            case "W":
                                prog[x, y, page] = new ProgAction(ActionType.CheckUp);
                                break;
                            case "A":
                                prog[x, y, page] = new ProgAction(ActionType.CheckLeft);
                                break;
                            case "S":
                                prog[x, y, page] = new ProgAction(ActionType.CheckDown);
                                break;
                            case "D":
                                prog[x, y, page] = new ProgAction(ActionType.CheckRight);
                                break;
                            case "w":
                                prog[x, y, page] = new ProgAction(ActionType.ShiftUp);
                                break;
                            case "a":
                                prog[x, y, page] = new ProgAction(ActionType.ShiftLeft);
                                break;
                            case "s":
                                prog[x, y, page] = new ProgAction(ActionType.ShiftDown);
                                break;
                            case "d":
                                prog[x, y, page] = new ProgAction(ActionType.ShiftRight);
                                break;
                            case "AS":
                                prog[x, y, page] = new ProgAction(ActionType.CheckDownLeft);
                                break;
                            case "WA":
                                prog[x, y, page] = new ProgAction(ActionType.CheckUpLeft);
                                break;
                            case "DW":
                                prog[x, y, page] = new ProgAction(ActionType.CheckUpRight);
                                break;
                            case "SD":
                                prog[x, y, page] = new ProgAction(ActionType.CheckDownRight);
                                break;
                            case "F":
                                prog[x, y, page] = new ProgAction(ActionType.CheckForward);
                                break;
                            case "f":
                                prog[x, y, page] = new ProgAction(ActionType.ShiftForward);
                                break;
                            case "r":
                                prog[x, y, page] = new ProgAction(ActionType.CheckRightRelative);
                                break;
                            case "l":
                                prog[x, y, page] = new ProgAction(ActionType.CheckLeftRelative);
                                break;
                        }

                        break;
                    case '#':
                        i++;
                        switch (progtext[i])
                        {
                            case 'S':
                                prog[x, y, page] = new ProgAction(ActionType.Start);
                                break;
                            case 'E':
                                prog[x, y, page] = new ProgAction(ActionType.Stop);
                                break;
                            case 'R':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    prog[x, y, page] =
                                        new ProgAction(ActionType.RunOnRespawn, progtext[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    prog[x, y, page] = new ProgAction(ActionType.RunSub, progtext[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    prog[x, y, page] =
                                        new ProgAction(ActionType.RunFunction, progtext[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (progtext[i])
                        {
                            case '>':
                                next = progtext[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    prog[x, y, page] = new ProgAction(ActionType.RunState, progtext[i..][1..next]);
                                    i += next;
                                }

                                break;
                            case 'n':
                                prog[x, y, page] = new ProgAction(ActionType.IsNotEmpty);
                                break;
                            case 'e':
                                prog[x, y, page] = new ProgAction(ActionType.IsEmpty);
                                break;
                            case 'f':
                                prog[x, y, page] = new ProgAction(ActionType.IsFalling);
                                break;
                            case 'c':
                                prog[x, y, page] = new ProgAction(ActionType.IsCrystal);
                                break;
                            case 'a':
                                prog[x, y, page] = new ProgAction(ActionType.IsLivingCrystal);
                                break;
                            case 'b':
                                prog[x, y, page] = new ProgAction(ActionType.IsBoulder);
                                break;
                            case 's':
                                prog[x, y, page] = new ProgAction(ActionType.IsSand);
                                break;
                            case 'k':
                                prog[x, y, page] = new ProgAction(ActionType.IsBreakableRock);
                                break;
                            case 'd':
                                prog[x, y, page] = new ProgAction(ActionType.IsUnbreakable);
                                break;
                            case 'A':
                                prog[x, y, page] = new ProgAction(ActionType.IsAcid);
                                break;
                            case 'B':
                                prog[x, y, page] = new ProgAction(ActionType.IsRedRock);
                                break;
                            case 'K':
                                prog[x, y, page] = new ProgAction(ActionType.IsBlackRock);
                                break;
                            case 'g':
                                prog[x, y, page] = new ProgAction(ActionType.IsGreenBlock);
                                break;
                            case 'y':
                                prog[x, y, page] = new ProgAction(ActionType.IsYellowBlock);
                                break;
                            case 'r':
                                prog[x, y, page] = new ProgAction(ActionType.IsRedBlock);
                                break;
                            case 'o':
                                prog[x, y, page] = new ProgAction(ActionType.IsSupport);
                                break;
                            case 'q':
                                prog[x, y, page] = new ProgAction(ActionType.IsQuadBlock);
                                break;
                            case 'R':
                                prog[x, y, page] = new ProgAction(ActionType.IsRoad);
                                break;
                            case 'x':
                                prog[x, y, page] = new ProgAction(ActionType.IsBox);
                                break;
                        }

                        break;
                    case '>':
                        next = progtext[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            prog[x, y, page] = new ProgAction(ActionType.GoTo, progtext[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = progtext[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            prog[x, y, page] = new ProgAction(ActionType.CreateFunction, progtext[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (progtext[i])
                        {
                            case '|':
                                prog[x, y, page] = new ProgAction(ActionType.Return);

                                break;
                            case '-':
                                i++;
                                if (progtext[i] == '|')
                                {
                                    prog[x, y, page] = new ProgAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (progtext[i] == '|')
                                {
                                    prog[x, y, page] = new ProgAction(ActionType.ReturnState);
                                }

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (progtext[i])
                        {
                            case 'W':
                                prog[x, y, page] = new ProgAction(ActionType.MoveUp);
                                break;
                            case 'A':
                                prog[x, y, page] = new ProgAction(ActionType.MoveLeft);
                                break;
                            case 'S':
                                prog[x, y, page] = new ProgAction(ActionType.MoveDown);
                                break;
                            case 'D':
                                prog[x, y, page] = new ProgAction(ActionType.MoveRight);
                                break;
                            case 'F':
                                prog[x, y, page] = new ProgAction(ActionType.MoveForward);
                                break;
                        }

                        break;
                    default:
                        var currentprogtext = progtext[i..];
                        if (currentprogtext.StartsWith("CCW;"))
                        {
                            i += 3;
                            prog[x, y, page] = new ProgAction(ActionType.RotateLeftRelative);
                        }
                        else if (currentprogtext.StartsWith("CW;"))
                        {
                            i += 2;
                            prog[x, y, page] = new ProgAction(ActionType.RotateRightRelative);
                        }
                        else if (currentprogtext.StartsWith("RAND;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.RotateRandom);
                        }
                        else if (currentprogtext.StartsWith("VB;"))
                        {
                            i += 2;
                            prog[x, y, page] = new ProgAction(ActionType.BuildMilitaryBlock);
                        }
                        else if (currentprogtext.StartsWith("DIGG;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.MacrosDig);
                        }
                        else if (currentprogtext.StartsWith("BUILD;"))
                        {
                            i += 5;
                            prog[x, y, page] = new ProgAction(ActionType.MacrosBuild);
                        }
                        else if (currentprogtext.StartsWith("HEAL;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.MacrosHeal);
                        }
                        else if (currentprogtext.StartsWith("MINE;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.MacrosMine);
                        }
                        else if (currentprogtext.StartsWith("FLIP;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.Flip);
                        }
                        else if (currentprogtext.StartsWith("BEEP;"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.Beep);
                        }
                        else if (currentprogtext.StartsWith("OR"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.Or);
                        }
                        else if (currentprogtext.StartsWith("AND"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.And);
                        }
                        else if (currentprogtext.StartsWith("AUT+"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.EnableAutoDig);
                        }
                        else if (currentprogtext.StartsWith("AUT-"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.DisableAutoDig);
                        }
                        else if (currentprogtext.StartsWith("ARG+"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.EnableAgression);
                        }
                        else if (currentprogtext.StartsWith("ARG-"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.DisableAgression);
                        }
                        else if (currentprogtext.StartsWith("=hp-"))
                        {
                            i += 3;
                            prog[x, y, page] = new ProgAction(ActionType.IsHpLower100);
                        }
                        else if (currentprogtext.StartsWith("=hp50"))
                        {
                            i += 4;
                            prog[x, y, page] = new ProgAction(ActionType.IsHpLower50);
                        }

                        break;
                }

                x++;
                if (page == 12)
                {
                    break;
                }
                else if (y == 16)
                {
                    page++;
                    y = 0;
                    x = 0;
                }
                else if (x == 16)
                {
                    x = 0;
                    y++;
                }
                else if (progtext[i] == '\n' && y < 16)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    y = 0;
                    x = 0;
                    page++;
                }
            }
        }
        public Program(Player owner, string progtext, string title)
        {
            this.owner = owner; this.progtext = progtext; this.title = title;
        }
        public pActionMatrix prog;
        public int id { get; set; }
        public string progtext { get; set; }
        public string title { get; set; }
        public Player owner { get; set; }
    }
}
