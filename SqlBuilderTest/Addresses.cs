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
        [SqlColumnName("userid")]
        public string UserID { get; set; }

        [SqlColumnName("postcode")]
        public string Postcode { get; set; }

        [SqlColumnName("state")]
        public string State { get; set; }

        [SqlColumnName("street")]
        public string Street { get; set; }
    }
}
