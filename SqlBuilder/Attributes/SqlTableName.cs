using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
