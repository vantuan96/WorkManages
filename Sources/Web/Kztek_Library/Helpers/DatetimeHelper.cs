using System;
using System.Globalization;

namespace Kztek_Library.Helpers
{
    public class DatetimeHelper
    {
        public static DateTime ConvertString_DDMMYYYY_ToDate(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public static DateTime ConvertString_DDMMYYYYHHmm_ToDate(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }

        public static DateTime ConvertString_MMDDYYYY_ToDate(string date)
        {
            return DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
