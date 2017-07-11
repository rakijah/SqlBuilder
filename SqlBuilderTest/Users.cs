using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBuilder.Attributes;
using SqlBuilder;

namespace SqlBuilderTest
{
    [SqlTableName("users")]
    public class Users : SqlTable
    {
        [SqlColumnName("username")]
        public string Username { get; set; }

        [SqlColumnName("password")]
        public string Password { get; set; }

        [SqlColumnName("email")]
        public string Email { get; set; }

        [SqlColumnName("id")]
        public string Id { get; set; }

        [SqlColumnName("lastname")]
        public string LastName { get; set; }

        [SqlColumnName("firstname")]
        public string FirstName { get; set; }
    }
}
