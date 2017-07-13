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

        /// <summary>
        /// Formats the provided column name according to SqlBuilder.UseSquareBrackets.
        /// </summary>
        /// <param name="column">The column name to be formatted.</param>
        public static string FormatSQL(string column)
        {
            if (!SqlBuild.Initialized)
                throw new Exception("Configure() must be called before building commands.");

            if (SqlBuild.UseSquareBrackets)
                return $"[{column}]";
            else
                return column;
        }

        /// <summary>
        /// Formats the provided table and column name according to SqlBuilder.UseSquareBrackets.
        /// </summary>
        /// <param name="table">The table name to be formatted.</param>
        /// <param name="column">The column name to be formatted.</param>
        public static string FormatSQL(string table, string column)
        {
            if (!SqlBuild.Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return $"{FormatSQL(table)}.{FormatSQL(column)}";
        }
    }
}