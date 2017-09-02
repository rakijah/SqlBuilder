using System.Data;
using SqlBuilder.Attributes;

namespace SqlBuilderTest
{
    [SqlTable("users")]
    public class Users
    {
        [SqlColumn("id", DbType.Int32)]
        public string Id { get; set; }

        [SqlColumn("username", DbType.String)]
        public string Username { get; set; }

        [SqlColumn("password", DbType.String)]
        public string Password { get; set; }

        [SqlColumn("email", DbType.String)]
        public string Email { get; set; }
        
        [SqlColumn("lastname", DbType.String)]
        public string LastName { get; set; }

        [SqlColumn("firstname", DbType.String)]
        public string FirstName { get; set; }
    }
}
