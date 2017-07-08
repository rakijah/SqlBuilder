using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public static class SqlBuild
    {
        /// <summary>
        /// Creates a new SELECT command.
        /// </summary>
        public static BuiltSelectCommand Select()
        {
            return new BuiltSelectCommand();
        }

        /// <summary>
        /// Creates a new INSERT command.
        /// </summary>
        public static BuiltInsertCommand Insert()
        {
            return new BuiltInsertCommand();
        }

        /// <summary>
        /// Creates a new DELETE command.
        /// </summary>
        public static BuiltDeleteCommand Delete()
        {
            return new BuiltDeleteCommand();
        }

        /// <summary>
        /// Converts a DateTime object to a string using the specified provider's format.
        /// </summary>
        /// <param name="dateTime">The DateTime object to be parsed.</param>
        /// <param name="format">The provider whose format is to be used.</param>
        /// <returns></returns>
        public static string DateToString(DateTime dateTime, SqlDateFormat format)
        {
            switch(format)
            {
                case SqlDateFormat.Oracle:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                case SqlDateFormat.SQL:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return "";
        }
    }

    public enum SqlDateFormat
    {
        Oracle,
        SQL
    }
}
