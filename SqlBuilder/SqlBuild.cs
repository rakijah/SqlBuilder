using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public static class SqlBuild
    {
        public static BuiltSelectCommand Select()
        {
            return new BuiltSelectCommand();
        }

        public static BuiltInsertCommand Insert()
        {
            return new BuiltInsertCommand();
        }

        public static BuiltDeleteCommand Delete()
        {
            return new BuiltDeleteCommand();
        }

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
