using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trempoline.Helpers
{
    public static class ExtensionMethods
    {
        public static Nullable<DateTime> ToFrenchDate(this string date)
        {

            if (!String.IsNullOrEmpty(date))
            {
                DateTime result;

                if( DateTime.TryParseExact(date,
                                            "dd-mm-yyyy",
                                            new System.Globalization.CultureInfo("fr-FR"),
                                            System.Globalization.DateTimeStyles.None, out result))
                {
                    return result;
                }

                return null;
            }
            else
                return null;

        }

        public static string Trimmed(this string str)
        {
            return str != null ? str.Trim() : null;
        }
    }
}