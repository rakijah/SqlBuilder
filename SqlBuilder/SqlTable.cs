using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SqlBuilder
{
    public class SqlTable
    {
        public static string GetTableName<T>()
        {
            var type = typeof(T);
            var tableNameAttributes = (SqlTableName[])type.GetCustomAttributes(typeof(SqlTableName), true);
            if (tableNameAttributes.Length == 0)
                throw new Exception($"Type \"{type.FullName}\" does not have a {typeof(SqlTableName).FullName} attribute.");

            return tableNameAttributes[0].TableName;
        }

        public static bool ContainsAllColumns<T>(params string[] columns)
        {
            var tableColumns = GetColumnNames<T>();
            foreach(var c in columns)
            {
                if (!tableColumns.Contains(c))
                    return false;
            }

            return true;
        }
        
        public static bool ContainsColumn<T>(string column)
        {
            return GetColumnNames<T>().Contains(column);
        }

        public static List<string> GetColumnNames<T>()
        {
            List<string> columnNames = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] columnNameAttributes = propertyInfo.GetCustomAttributes(typeof(SqlColumnName), true);
                if (columnNameAttributes.Length > 0)
                {
                    SqlColumnName columnNameAttribute = (SqlColumnName)columnNameAttributes[0];
                    columnNames.Add(columnNameAttribute.ColumnName);
                }
            }
            return columnNames;
        }
    }
}
