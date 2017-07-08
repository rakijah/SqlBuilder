using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    internal static class Extensions
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

    }
}
