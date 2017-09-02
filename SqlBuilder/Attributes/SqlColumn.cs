using System;
using System.Data;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlColumn : Attribute
    {
        public string ColumnName { get; set; }
        public DbType Type { get; set; }

        public SqlColumn(string columnName, DbType type)
        {
            ColumnName = columnName;
            Type = type;
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