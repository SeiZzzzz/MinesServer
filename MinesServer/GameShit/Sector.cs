using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.GameShit.Generator;

namespace MinesServer.GameShit
{
    public class Sector
    {
        public CellType[] GenerateInsides()
        {
            if (types == null)
            {
                types = depth switch
                {
                    (< 500) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Green } : new CellType[] { CellType.Green, CellType.XGreen },
                    (< 1000) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Green, CellType.XGreen } : new CellType[] { CellType.Blue, CellType.Green, CellType.XGreen },
                    (< 1500) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Blue, CellType.XGreen } : new CellType[] { CellType.Blue, CellType.Green, CellType.XBlue },
                    (< 2000) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Blue } : new CellType[] { CellType.Blue, CellType.Green },
                    (< 3000) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Red, CellType.Blue } : new CellType[] { CellType.Blue, CellType.Red, CellType.Green },
                    (< 4000) => new Random().Next(1, 101) > 80 ? new CellType[] { CellType.Red, CellType.Violet } : new CellType[] { CellType.Blue, CellType.Red, CellType.Violet },
                    _ => new CellType[] { CellType.Cyan, CellType.Violet, CellType.Red, CellType.White, CellType.XCyan, CellType.XViolet, CellType.XRed }
                };
            }
            var re = new CellType[0];
            return re.Concat(types).ToArray();
        }
        public List<SectorCell> seccells;
        public List<Cell> cells;
        public int height;
        public int width;
        public int depth;
        public CellType[] types;
    }
}
