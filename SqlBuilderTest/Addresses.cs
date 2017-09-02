using System.Data;
using SqlBuilder.Attributes;

namespace SqlBuilderTest
{
    [SqlTable("addresses")]
    public class Addresses
    {
        [SqlColumn("userid", DbType.Int32)]
        public string UserID { get; set; }

        [SqlColumn("postcode", DbType.String)]
        public string Postcode { get; set; }

        [SqlColumn("state", DbType.String)]
        public string State { get; set; }

        [SqlColumn("street", DbType.String)]
        public string Street { get; set; }
    }
}
