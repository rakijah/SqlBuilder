using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    internal static class Util
    {

        /// <summary>
        /// Combines all entries in the list by putting inBetween between each entry.
        /// Example:
        /// { "abc", "def" }.Zip(", ") => "abc, def"
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="inBetween"></param>
        public static string Zip(this List<string> strings, string inBetween)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                sb.Append(strings[i]);
                if (i < strings.Count - 1)
                    sb.Append(inBetween);
            }
            return sb.ToString();
        }

        public static string FormatSQL(string column)
        {
            if (!SqlBuild.Initialized)
                throw new Exception("Configure() must be called before building commands.");

            if (SqlBuild.UseSquareBrackets)
                return $"[{column}]";
            else
                return column;
        }

        public static string FormatSQL(string table, string column)
        {
            if (!SqlBuild.Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return $"{FormatSQL(table)}.{FormatSQL(column)}";
        }

        /*
        /// <summary>
        /// Converts a DateTime object to a string using the specified provider's format.
        /// </summary>
        /// <param name="dateTime">The DateTime object to be parsed.</param>
        /// <param name="format">The provider whose format is to be used.</param>
        public static string DateToString(DateTime dateTime, DatabaseProvider format)
        {
            switch (format)
            {
                case DatabaseProvider.Oracle:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                case DatabaseProvider.SqlServer:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }*/
    }
}
