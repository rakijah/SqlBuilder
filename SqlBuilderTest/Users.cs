using SqlBuilder.Attributes;

namespace SqlBuilderTest
{
    [SqlTable("users")]
    public class Users
    {
        [SqlColumn("id", SqlColumnType.Integer)]
        public string Id { get; set; }

        [SqlColumn("username", SqlColumnType.String)]
        public string Username { get; set; }

        [SqlColumn("password", SqlColumnType.String)]
        public string Password { get; set; }

        [SqlColumn("email", SqlColumnType.String)]
        public string Email { get; set; }
        
        [SqlColumn("lastname", SqlColumnType.String)]
        public string LastName { get; set; }

        [SqlColumn("firstname", SqlColumnType.String)]
        public string FirstName { get; set; }
    }
}
