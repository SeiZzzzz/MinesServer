namespace MinesServer.GameShit.Programmator
{
    public class PFunction
    {
        public int current { get; set; } = 0;
        public PAction Next
        {
            get
            {
                var c = actions[current];
                current++;
                if (actions.Count <= current)
                {
                    current = 0;
                }
                return c;
            }
        }
        public string calledfrom { get; set; }
        public void Reset() => current = 0;
        public bool? state { get; set; }
        public ActionType? laststateaction { get; set; }
        public void Close()
        {
            foreach (var i in actions)
                i.SetFather(this);
        }
        public static PFunction operator +(PFunction a, PAction b)
        {
            a.actions.Add(b);
            return a;
        }
        public List<PAction> actions { get; set; } = new();
    }
}
