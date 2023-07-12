namespace MinesServer.GameShit.Generator
{
    public class VulcanoHeart : Cell
    {
        public VulcanoHeart(int x, int y, float id) : base(x, y, 31)
        {
            cells = new List<VulcanoCell>();
            cells.Add(new VulcanoCell(x + 1, y, this, 1));
            cells.Add(new VulcanoCell(x - 1, y, this, 1));
            cells.Add(new VulcanoCell(x, y + 1, this, 1));
            cells.Add(new VulcanoCell(x, y - 1, this, 1));

        }
        public float id;
        public bool stopSpreading;
        public override void Update()
        {
            if (stopSpreading)
            {
                return;
            }
            var l = new List<VulcanoCell>();
            for (int i = 1; i < cells.Count; i++)
            {
                l = l.Concat(cells[i].HeatTerritory()).ToList();
            }
            cells = cells.Concat(l).ToList();
        }
        public List<VulcanoCell> cells;

    }
}
