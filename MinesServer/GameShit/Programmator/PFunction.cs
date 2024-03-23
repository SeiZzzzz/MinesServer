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
        public (int x, int y) startoffset { get; set; }
        public string calledfrom { get; set; }
        public void Reset()
        {
            current = 0;
            startoffset = (0, 0);
        }
        public bool? state { get; set; }
        public ActionType? laststateaction { get; set; }
        public void Close()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var st = actions[i];
                st.father = this;
                actions[i] = st;
            }
        }
        public static PFunction operator +(PFunction a, PAction b)
        {
            a.actions.Add(b);
            return a;
        }
        public List<PAction> actions { get; set; } = new();
    }
}
