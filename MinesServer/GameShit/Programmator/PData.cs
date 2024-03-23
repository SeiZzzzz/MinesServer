
using MoreLinq;
using System.ComponentModel.Design;

namespace MinesServer.GameShit.Programmator
{
    public class PData
    {
        public PData(Player p)
        {
            ProgRunning = false;
            this.p = p;
        }
        Player p;
        public int checkX;
        public int checkY;
        public int shiftX;
        public int shiftY;
        public bool ProgRunning { 
            get; 
            set; 
        }
        public Dictionary<string, PFunction> currentprog { get; set; }
        public DateTime delay;
        private string cFunction;
        private Program? selected;
        private PFunction current
        {
            get => currentprog[cFunction];
        }
        public void Run(Program p)
        {
            selected = p;
            cFunction = "";
            currentprog = p.programm;
            foreach (var i in currentprog)
            {
                Console.WriteLine($"{i.Key} - {string.Join(' ', i.Value.actions.Select(i => i.type))}");
            }
            foreach (var i in currentprog.Values)
                i.Close();
            delay = DateTime.Now;
            ProgRunning = true;
        }
        public void Run()
        {
            if (ProgRunning || selected == null)
            {
                ProgRunning = false;
                return;
            }
            Run(selected);
        }
        private void Next()
        {
            var i = currentprog.Keys.ToList().IndexOf(cFunction);
            if (currentprog.Count > i + 1)
                cFunction = currentprog.ElementAt(i + 1).Key;
            else
                cFunction = currentprog.First().Key;
        }
        public void IncreaseDelay(int ms) => delay = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        private bool? temp = null;
        public void Step()
        {
            if (current == null || DateTime.Now < delay)
            {
                return;
            }
            PAction action;
            if (current.actions.Count <= 0)
            {
                Next();
                return;
            }
            action = current.Next;
            object result = action.Execute(p, ref temp)!;
            switch (result)
            {
                case string label:
                    switch (action.type)
                    {
                        case ActionType.GoTo:
                            current.Reset();
                            currentprog[label].calledfrom = cFunction;
                            cFunction = label;
                            break;
                        case ActionType.RunSub or ActionType.RunFunction:
                            if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                                currentprog[label].startoffset = (shiftX + checkX, shiftY + checkY);
                            currentprog[label].calledfrom = cFunction;
                            cFunction = label;
                            break;
                        case ActionType.RunIfTrue or ActionType.RunIfFalse:
                            if (label == "")
                            {
                                break;
                            }
                            current.Reset();
                            cFunction = label;
                            break;
                    }
                    break;
                case bool state:
                    switch (action.type)
                    {
                        case ActionType.ReturnFunction:
                            current.Reset();
                            current.startoffset = (0, 0);
                            cFunction = current.calledfrom;
                            current.state = state;
                            current.startoffset = (0, 0);
                            break;
                    }
                    break;
                case null:
                    switch (action.type)
                    {
                        case ActionType.CheckDown or ActionType.CheckUp or ActionType.CheckRight or ActionType.CheckLeft
                        or ActionType.CheckDownLeft or ActionType.CheckDownRight or ActionType.CheckUpLeft or ActionType.CheckUpRight
                        or ActionType.ShiftUp or ActionType.ShiftLeft or ActionType.ShiftDown or ActionType.ShiftRight or ActionType.ShiftForward:
                            if (current.startoffset != default)
                            {
                                current.startoffset = (0, 0);
                            }
                                break;
                        case ActionType.Return:
                            current.Reset();
                            cFunction = current.calledfrom;
                            break;
                    }
                    break;
            }
            IncreaseDelay(action.delay);
        }
    }
}
