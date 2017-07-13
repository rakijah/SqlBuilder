using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilderTest
{
    [SqlTableName("addresses")]
    public class Addresses
    {
        [SqlColumn("userid", SqlColumnType.Integer)]
        public string UserID { get; set; }

        [SqlColumn("postcode", SqlColumnType.String)]
        public string Postcode { get; set; }

        [SqlColumn("state", SqlColumnType.String)]
        public string State { get; set; }

        [SqlColumn("street", SqlColumnType.String)]
        public string Street { get; set; }
    }
}
