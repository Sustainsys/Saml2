using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    static class DateTimeHelper
    {
        internal static DateTime? EarliestTime(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || 
                value1.HasValue && value2.HasValue && value1.Value > value2.Value)
            {
                return value2;
            }

            return value1;
        }
    }
}
