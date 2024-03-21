using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator
{
    public struct pActionMatrix
    {
        #region shit
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(pActionMatrix f1, pActionMatrix f2) { return false; }
        public static bool operator !=(pActionMatrix f1, pActionMatrix f2) { return false; }
        #endregion
        const int pages = 12 + 1;
        const int rows = 16 + 1;
        const int cols = 16 + 1;
        public pActionMatrix()
        {
            matrix = new ProgAction[rows, cols, pages];
        }
        private ProgAction[,,] matrix;
        public ProgAction this[int x,int y,int page]
        {
            get => matrix[x, y,page];
            set => matrix[x, y,page] = value;
        }

        //fix speed
        public ProgAction Next()
        {
            start:
            if (matrix == null)
                matrix = new ProgAction[rows, cols, pages];
            var v = this[x, y, page];
            if (x < rows - 1)
            {
                x++;
            }
            else if (y < cols - 1)
            {
                x = 0; y++;
            }
            else if (page < pages - 1)
            {
                page++;x = 0; y = 0;
            }
            else
            {
                page = 0;y = 0;x = 0;
            }
            if (v.type == ActionType.None)
            {
                goto start;
            }
            return v;
        }
        public (int X, int Y,int PAGE) IndexOf(string label)
        {
            for (var page = 0; page < matrix.GetLength(2); page++)
            {
                for (var x = 0; x < matrix.GetLength(0); x++)
                {
                    for (var y = 0; y < matrix.GetLength(1); y++)
                    {
                        var action = matrix[x, y, page];
                        if (action.label == label && action.type == ActionType.Label)
                        {
                            return (x, y,page);
                        }
                    }
                }
            }
            return (0, 0,0);
        }
        private int x, y, page;
    }
}
