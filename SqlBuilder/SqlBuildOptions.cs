using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class SqlBuildOptions
    {
        public DatabaseProvider Provider { get; set; } = DatabaseProvider.Unknown;
        public string DotNetDateFormatString { get; set; } = "";
        public string InternalDateFormatString { get; set; } = "";
        public bool WrapFieldsInSquareBrackets { get; set; } = false;
    }
}
