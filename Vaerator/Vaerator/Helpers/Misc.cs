using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Helpers
{
    public class Misc
    {
        public static int RoundToEven(float x)
        {
            return (int)(2 * Math.Ceiling(x / 2));
        }

        public static int RoundToOdd(float x)
        {
            return (int)((2 * Math.Floor(x / 2)) + 1);
        }
    }
}
