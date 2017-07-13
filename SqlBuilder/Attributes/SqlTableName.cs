using System;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlTableName : Attribute
    {
        public string TableName { get; set; }

        public SqlTableName(string tableName)
        {
            TableName = tableName;
        }
    }
}