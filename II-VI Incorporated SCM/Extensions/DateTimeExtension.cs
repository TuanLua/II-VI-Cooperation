using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Extensions
{
    public static class DateTimeExtension
    {
        public static string GetDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MMM-yy");
        }
    }
}