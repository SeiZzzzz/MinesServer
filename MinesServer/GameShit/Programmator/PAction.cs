namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public PFunction father;
        public PAction(ActionType t)
        {
            type = t;
        }
        public PAction(ActionType t, string label)
        {
            this.label = label; type = t;
        }
        public void SetFather(PFunction f) => father = f;
        public int delay;
        public string label;
        public ActionType type;
        private void Check(ActionType t)
        {

        }
        public object? Execute(Player p, ref bool? template)
        {
            switch (type)
            {
                case ActionType.MoveDown:
                    delay = p.pause / 100;
                    if (p.Move(p.x, p.y + 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveUp:
                    delay = p.pause / 100;
                    if (p.Move(p.x, p.y - 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveRight:
                    delay = p.pause / 100;
                    if (p.Move(p.x + 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveLeft:
                    delay = p.pause / 100;
                    if (p.Move(p.x - 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveForward:
                    delay = 25;
                    //callmov
                    break;
                case ActionType.ReturnState:
                    return father.state;
                case ActionType.RunIfTrue or ActionType.RunIfFalse:
                    var resultif = template;
                    template = null;
                    return resultif;
                case ActionType.Or or ActionType.And:
                    father.laststateaction = type;
                    break;
                case ActionType.GoTo:
                    return label;
            }
            return null;
        }
    }
}
