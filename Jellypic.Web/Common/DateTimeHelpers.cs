using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class DateTimeHelpers
    {
        public static int ToEpoch(this DateTime time)
        {
            var span = time - new DateTime(1970, 1, 1);
            return (int)span.TotalSeconds;
        }
    }
}
