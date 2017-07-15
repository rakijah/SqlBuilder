using System;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlColumn : Attribute
    {
        public string ColumnName { get; set; }
        public SqlColumnType Type { get; set; }

        public SqlColumn(string columnName, SqlColumnType type)
        {
            ColumnName = columnName;
            Type = type;
        }

        /// <summary>
        /// Formats a value to conform to this columns type.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        public string FormatValueFor(object value)
        {
            switch (Type)
            {
                case SqlColumnType.Integer:
                    return value.ToString();

                case SqlColumnType.String:
                    return WrapString(value.ToString());

                case SqlColumnType.Date:
                    return ProviderSpecific.DateInsertFunction(value.ToString());

                default:
                    return ColumnName;
            }
        }

        private string WrapString(string value)
        {
            switch (SqlBuild.Options.Provider)
            {
                case DatabaseProvider.Oracle10GOrLater:
                    return $"'{value}'";

                default:
                    return $"\"{value}\"";
            }
        }
    }
}