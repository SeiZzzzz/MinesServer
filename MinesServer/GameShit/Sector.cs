using MinesServer.GameShit.Generator;
using MinesServer.Network.TypicalEvents;

namespace MinesServer.GameShit
{
    public class Sector
    {
        private static Random r = new Random();
        public CellType[] GenerateInsides()
        {
            var gig = r.Next(1, 101) >= 80 ? true : false;
            if (types == null)
            {
                types = depth switch
                {
                    (< 500) => !gig ? [CellType.Green, CellType.YellowSand, CellType.Rock,CellType.DarkWhiteSand,CellType.HeavyRock] : [CellType.XBlue,CellType.Green, CellType.XGreen],
                    (< 1000) => !gig ? [CellType.Green, CellType.XGreen,CellType.XBlue] : [CellType.Blue, CellType.Green, CellType.XGreen] ,
                    (< 1500) => !gig ? [CellType.Blue, CellType.XGreen,CellType.Lava,CellType.Rock,CellType.BlueSand,CellType.Boulder1] : [CellType.Blue, CellType.Green, CellType.XBlue],
                    (< 2000) => !gig ? [CellType.Blue,CellType.Rock,CellType.Lava,CellType.XBlue] : [CellType.Blue, CellType.Green,CellType.XBlue,CellType.XGreen],
                    (< 3000) => !gig ? [CellType.Red, CellType.Blue,CellType.Rock,CellType.Lava] : [CellType.Blue, CellType.Red, CellType.Green],
                    (< 4000) => !gig ? [CellType.Red, CellType.Violet,CellType.Boulder1,CellType.XViolet] : [CellType.Blue, CellType.Red, CellType.Violet],
                    _ => [CellType.Cyan, CellType.Violet, CellType.Red, CellType.White, CellType.XCyan, CellType.XViolet, CellType.XRed]
                };
            }
            var len = r.Next(2, types.Length);
            var re = new CellType[0];
            for (int i = 0;i < len;i++)
            {
                var j = types[r.Next(0, types.Length)];
                if (!re.Contains(j))
                {
                    re = re.Append(j).ToArray();
                    continue;
                }
                i--;
            }
            return re;
        }
        public List<SectorCell> seccells;
        public List<Cell> cells;
        public int height;
        public int width;
        public int depth;
        public CellType[] types;
    }
}
