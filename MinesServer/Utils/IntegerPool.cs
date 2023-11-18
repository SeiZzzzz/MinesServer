using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.Utils
{
    public class IntegerPool
    {
        public void Free(int index) => isFree[index] = true;

        public int GetFree(out int index)
        {
            int i;
            for (i = 0; i < use_size; i++)
                if (isFree[i])
                {
                    index = i;
                    isFree[index] = false;
                    return i;
                }
            if (i >= max_size)
            {
                index = -1;
                return -1;
            }
            index = i;
            isFree[index] = false;
            use_size = index + 1;
            return i;
        }

        public IntegerPool(int count)
        {
            max_size = count;
            isFree = new bool[max_size];
            use_size = 0;
        }

        private bool[] isFree;

        public int max_size;

        private int use_size;
    }
}
