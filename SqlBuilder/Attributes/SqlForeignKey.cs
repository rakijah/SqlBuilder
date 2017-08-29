using System;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlForeignKey : Attribute
    {
        public string ForeignTableColumnName { get; set; }
        public Type ForeignTable { get; set; }

        public SqlForeignKey(string foreignTableColumnName, Type foreignTable)
        {
            if (!SqlTable.IsSqlTable(foreignTable))
            {
                throw new Exception("Cannot define foreign-keys of non-SqlTable type.");
            }

            ForeignTableColumnName = foreignTableColumnName;
            ForeignTable = foreignTable;
        }
    }
}