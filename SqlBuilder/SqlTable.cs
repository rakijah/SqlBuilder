using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SqlBuilder
{
    /// <summary>
    /// A helper class to extract attribute values from table classes.
    /// </summary>
    public static class SqlTable
    {
        //Caching to reduce the performance impact of reflection.
        private static Dictionary<Type, string> _tableNames = new Dictionary<Type, string>();

        private static Dictionary<Type, List<string>> _tableColumns = new Dictionary<Type, List<string>>();
        private static Dictionary<Type, Map<PropertyInfo, string>> _tablePropertiesAndColumns = new Dictionary<Type, Map<PropertyInfo, string>>();

        /// <summary>
        /// Return the name of the table provided by the SqlTableName attribute.
        /// </summary>
        /// <typeparam name="T">The type whose table name should be returned.</typeparam>
        public static string GetTableName<T>()
        {
            var type = typeof(T);
            if (_tableNames.ContainsKey(type))
                return _tableNames[type];

            var tableNameAttributes = (SqlTableName[])type.GetCustomAttributes(typeof(SqlTableName), true);
            if (tableNameAttributes.Length == 0)
                throw new Exception($"Type \"{type.FullName}\" does not have a {typeof(SqlTableName).FullName} attribute.");

            var tableName = tableNameAttributes[0].TableName;
            _tableNames.Add(type, tableName);
            return tableName;
        }

        /// <summary>
        /// Checks whether the specified type contains all columns whose names are in the provided string array.
        /// </summary>
        /// <typeparam name="T">The type to check for columns.</typeparam>
        /// <param name="columns">The columns that are supposed to be on the type T.</param>
        public static bool ContainsAllColumns<T>(params string[] columns)
        {
            var tableColumns = GetColumnNames<T>();
            foreach (var c in columns)
            {
                if (!tableColumns.Contains(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether a type contains a column with the specified name.
        /// </summary>
        /// <typeparam name="T">The type to be checked.</typeparam>
        /// <param name="column">The name of the column.</param>
        /// <returns></returns>
        public static bool ContainsColumn<T>(string column)
        {
            return GetColumnNames<T>().Contains(column);
        }

        /// <summary>
        /// Returns a dictionary that maps the type's properties to their SQL column names.
        /// </summary>
        /// <typeparam name="T">The type whose properties should be inspected.</typeparam>
        public static Dictionary<PropertyInfo, string> PropertiesToColumnNames<T>()
        {
            var type = typeof(T);
            if (_tablePropertiesAndColumns.ContainsKey(type))
                return _tablePropertiesAndColumns[type].ForwardD;

            Dictionary<PropertyInfo, string> lookupTable = new Dictionary<PropertyInfo, string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] columnNameAttributes = propertyInfo.GetCustomAttributes(typeof(SqlColumn), true);
                if (columnNameAttributes.Length > 0)
                {
                    SqlColumn columnNameAttribute = (SqlColumn)columnNameAttributes[0];
                    lookupTable.Add(propertyInfo, columnNameAttribute.ColumnName);
                }
            }
            _tablePropertiesAndColumns.Add(type, Map<PropertyInfo, string>.FromDictionary(lookupTable));
            return lookupTable;
        }

        /// <summary>
        /// Returns a dictionary that maps the SQL column names to the type's properties.
        /// </summary>
        /// <typeparam name="T">The type whose properties should be inspected.</typeparam>
        public static Dictionary<string, PropertyInfo> ColumnNamesToProperties<T>()
        {
            var type = typeof(T);
            if (_tablePropertiesAndColumns.ContainsKey(type))
                return _tablePropertiesAndColumns[type].ReverseD;

            Dictionary<string, PropertyInfo> lookupTable = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] columnNameAttributes = propertyInfo.GetCustomAttributes(typeof(SqlColumn), true);
                if (columnNameAttributes.Length > 0)
                {
                    SqlColumn columnNameAttribute = (SqlColumn)columnNameAttributes[0];
                    lookupTable.Add(columnNameAttribute.ColumnName, propertyInfo);
                }
            }
            _tablePropertiesAndColumns.Add(type, Map<PropertyInfo, string>.FromDictionaryReverse(lookupTable));
            return lookupTable;
        }

        /// <summary>
        /// Returns a list with all the column names of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be inspected.</typeparam>
        public static List<string> GetColumnNames<T>()
        {
            Type type = typeof(T);
            if (_tableColumns.ContainsKey(type))
                return _tableColumns[type];

            List<string> columnNames = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] columnNameAttributes = propertyInfo.GetCustomAttributes(typeof(SqlColumn), true);
                if (columnNameAttributes.Length > 0)
                {
                    SqlColumn columnNameAttribute = (SqlColumn)columnNameAttributes[0];
                    columnNames.Add(columnNameAttribute.ColumnName);
                }
            }
            _tableColumns.Add(type, columnNames);
            return columnNames;
        }

        /// <summary>
        /// Returns a list of SqlBuilder.Attributes.SqlColumn instances that are attached to the types properties.
        /// </summary>
        /// <typeparam name="T">The type to be inspected.</typeparam>
        public static List<SqlColumn> GetColumnAttributes<T>()
        {
            List<SqlColumn> columnAttributes = new List<SqlColumn>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] columnNameAttributes = propertyInfo.GetCustomAttributes(typeof(SqlColumn), true);
                if (columnNameAttributes.Length > 0)
                {
                    columnAttributes.Add((SqlColumn)columnNameAttributes[0]);
                }
            }
            return columnAttributes;
        }
    }
}