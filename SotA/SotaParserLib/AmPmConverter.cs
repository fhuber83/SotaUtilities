using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class AmPmConverter
    {
        public enum AmPm
        {
            AM,
            PM
        }

        public static void Convert12To24(AmPm amPm, int hour12, out int hour24)
        {
            if (hour12 == 12)
            {
                if (amPm == AmPm.AM)
                {
                    hour24 = 0;
                }
                else
                {
                    hour24 = 12;
                }
            }
            else
            {
                if (amPm == AmPm.AM)
                {
                    hour24 = hour12;
                }
                else
                {
                    hour24 = hour12 + 12;
                }
            }
        }
    }
}
