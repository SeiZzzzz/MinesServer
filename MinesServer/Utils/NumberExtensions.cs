namespace MinesServer.Utils
{
    public static class NumberExtensions
    {
        public static int Digits(this short n)
        {
            if (n >= 0)
            {
                if (n < 10) return 1;
                if (n < 100) return 2;
                if (n < 1000) return 3;
                if (n < 10000) return 4;
                return 5;
            }
            else
            {
                if (n > -10) return 2;
                if (n > -100) return 3;
                if (n > -1000) return 4;
                if (n > -10000) return 5;
                return 6;
            }
        }

        public static int Digits(this ushort n)
        {
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            return 5;
        }

        public static int Digits(this int n)
        {
            if (n >= 0)
            {
                if (n < 10) return 1;
                if (n < 100) return 2;
                if (n < 1000) return 3;
                if (n < 10000) return 4;
                if (n < 100000) return 5;
                if (n < 1000000) return 6;
                if (n < 10000000) return 7;
                if (n < 100000000) return 8;
                if (n < 1000000000) return 9;
                return 10;
            }
            else
            {
                if (n > -10) return 2;
                if (n > -100) return 3;
                if (n > -1000) return 4;
                if (n > -10000) return 5;
                if (n > -100000) return 6;
                if (n > -1000000) return 7;
                if (n > -10000000) return 8;
                if (n > -100000000) return 9;
                if (n > -1000000000) return 10;
                return 11;
            }
        }

        public static int Digits(this uint n)
        {
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            if (n < 100000) return 5;
            if (n < 1000000) return 6;
            if (n < 10000000) return 7;
            if (n < 100000000) return 8;
            if (n < 1000000000) return 9;
            return 10;
        }

        public static int Digits(this long n)
        {
            if (n >= 0)
            {
                if (n < 10L) return 1;
                if (n < 100L) return 2;
                if (n < 1000L) return 3;
                if (n < 10000L) return 4;
                if (n < 100000L) return 5;
                if (n < 1000000L) return 6;
                if (n < 10000000L) return 7;
                if (n < 100000000L) return 8;
                if (n < 1000000000L) return 9;
                if (n < 10000000000L) return 10;
                if (n < 100000000000L) return 11;
                if (n < 1000000000000L) return 12;
                if (n < 10000000000000L) return 13;
                if (n < 100000000000000L) return 14;
                if (n < 1000000000000000L) return 15;
                if (n < 10000000000000000L) return 16;
                if (n < 100000000000000000L) return 17;
                if (n < 1000000000000000000L) return 18;
                return 19;
            }
            else
            {
                if (n > -10L) return 2;
                if (n > -100L) return 3;
                if (n > -1000L) return 4;
                if (n > -10000L) return 5;
                if (n > -100000L) return 6;
                if (n > -1000000L) return 7;
                if (n > -10000000L) return 8;
                if (n > -100000000L) return 9;
                if (n > -1000000000L) return 10;
                if (n > -10000000000L) return 11;
                if (n > -100000000000L) return 12;
                if (n > -1000000000000L) return 13;
                if (n > -10000000000000L) return 14;
                if (n > -100000000000000L) return 15;
                if (n > -1000000000000000L) return 16;
                if (n > -10000000000000000L) return 17;
                if (n > -100000000000000000L) return 18;
                if (n > -1000000000000000000L) return 19;
                return 20;
            }
        }

        public static int Digits(this ulong n)
        {
            if (n < 10UL) return 1;
            if (n < 100UL) return 2;
            if (n < 1000UL) return 3;
            if (n < 10000UL) return 4;
            if (n < 100000UL) return 5;
            if (n < 1000000UL) return 6;
            if (n < 10000000UL) return 7;
            if (n < 100000000UL) return 8;
            if (n < 1000000000UL) return 9;
            if (n < 10000000000UL) return 10;
            if (n < 100000000000UL) return 11;
            if (n < 1000000000000UL) return 12;
            if (n < 10000000000000UL) return 13;
            if (n < 100000000000000UL) return 14;
            if (n < 1000000000000000UL) return 15;
            if (n < 10000000000000000UL) return 16;
            if (n < 100000000000000000UL) return 17;
            if (n < 1000000000000000000UL) return 18;
            return 19;
        }
    }
}
