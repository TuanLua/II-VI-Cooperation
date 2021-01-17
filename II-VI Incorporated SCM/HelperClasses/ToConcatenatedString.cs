using System;
using System.Collections.Generic;

namespace II_VI_Incorporated_SCM.Library.ExtentionMethods
{
    public static class ExtentionMethods
    {
        public static string ToConcatenatedString(
            this List<string> list
            , string separator)
        {
            return String.Join(separator, list);
        }

        public static string ToConcatenatedString(
            this string[] list
            , string separator)
        {
            return String.Join(separator, list);
        }
    }
}