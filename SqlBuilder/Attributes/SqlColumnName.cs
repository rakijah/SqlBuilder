using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlColumnName : Attribute
    {
        public string ColumnName { get; set; }
        public SqlColumnName(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
