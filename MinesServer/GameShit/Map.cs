namespace MinesServer.GameShit
{
    public class Map
    {
        public Map(int w, int h)
        {
            width = w; height = h;
            mapmesh = new byte[2, w * h];
        }
        public byte[,] mapmesh;
        public int width;
        public int height;
    }
}
