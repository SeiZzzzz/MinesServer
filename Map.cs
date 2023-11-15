namespace MinesServer.GameShit
{
    public class Map
    {
        public Map(int w, int h)
        {
            width = w; height = h;
            mapmesh = new byte[2][] { new byte[w * h], new byte[w * h]/*, new byte[w * h] , new byte[w * h]*/ };
        }
        public void SaveMap()
        {
            var b = new byte[0];
            for (int i = 0; i < mapmesh.GetLength(0); i++)
            {
                b = b.Concat(mapmesh[i]).ToArray();
            }
            File.WriteAllBytes(World.W.name + ".map", b);
            Console.WriteLine("map saved");
        }
        public bool LoadMap()
        {
            if (File.Exists(World.W.name + ".map"))
            {
                var m = File.ReadAllBytes(World.W.name + ".map");
                var index = 0;
                var current = 0;
                for (int i = 0; i < 1; i++)
                {
                    for (; current < mapmesh[i].Length;)
                    {
                        mapmesh[i][current] = m[index];
                        current++;
                        index++;
                    }
                    current = 0;
                }
                Console.WriteLine("map loaded");
                return true;
            }
            return false;
        }
        public byte[][] mapmesh;
        public int width;
        public int height;
    }
}
