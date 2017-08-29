using System;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlTable : Attribute
    {
        public string TableName { get; set; }

        public SqlTable(string tableName)
        {
            TableName = tableName;
        }

        public static bool IsSqlTable(Type type)
        {
            return type.GetCustomAttributes(typeof(SqlTable), false).Length != 0;
        }
    }
}