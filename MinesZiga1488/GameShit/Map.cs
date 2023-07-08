namespace MinesServer.GameShit
{
    public class Map
    {
        public Map(int w, int h)
        {
            width = w; height = h;
            mapmesh = new Cell[w * h];
        }
        public Cell[] mapmesh;
        public int width;
        public int height;
    }
}
