using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TwentyFiftyMaterialsCore
{
    public class ValueConverter
    {
        public static CultureInfo DataCultureInfo => GetCultureInfo();

        private static CultureInfo GetCultureInfo()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            return culture;
        }

        public static double StringToDouble(string valueStr)
        {
            double.TryParse(valueStr, NumberStyles.Any, DataCultureInfo, out double val);
            return val;
        }

        public static string RemoveBetween(string sourceString, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            string parsed = regex.Replace(sourceString, startTag + endTag);
            parsed = parsed.Replace(startTag, "");
            parsed = parsed.Replace(endTag, "");

            return parsed;
        }

    }
}
