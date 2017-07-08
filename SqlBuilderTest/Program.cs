using System;
using SqlBuilder;

namespace SqlBuilderTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string tableUsers = "users";
            string tableAddresses = "adresses";
            var selectSql = SqlBuild.Select()
                                         .AddColumns(tableUsers, "username", "email")
                                         .AddTable(tableUsers)
                                         .Join(tableUsers, "id", tableAddresses, "userid")
                                         .AddColumns(tableUsers, "postcode", "street")
                                         .Where()
                                            .BeginBlock()
                                            .AddCondition(tableUsers, "lastname", "Jones", "=", '"')
                                            .And()
                                            .AddCondition(tableAddresses, "state", "Ohio", ">")
                                            .EndBlock()
                                            .Or()
                                            .AddCondition(tableUsers, "id", "50", ">", '"')
                                            .Finish()
                                         .OrderBy()
                                            .SortBy(tableUsers, "firstname", SqlSortMode.DESCENDING)
                                            .Finish();
            Console.WriteLine(selectSql);
            Console.ReadLine();

            var insertSql = SqlBuild.Insert()
                                        .Into(tableUsers, "username", "password", "email")
                                        .AddValues()
                                            .AddValueFor("username", "rakijah", '"')
                                            .AddValueFor("password", "dGhlIGdhbWU=", '"')
                                            .AddValueFor("email", "rakijah@fakemail.com", '"')
                                            .Finish()
                                        .AddRow(SurroundWith("user2", '"'), SurroundWith("ZmVsbCBmb3IgaXQgYWdhaW4=", '"'), SurroundWith("user2@fakemail.com", '"'));

            Console.WriteLine(insertSql);
            Console.ReadLine();

            var deleteSql = SqlBuild.Delete()
                                        .From(tableUsers)
                                        .Where()
                                            .AddCondition(tableUsers, "id", "10", "<")
                                            .Finish();
            Console.WriteLine(deleteSql);
            Console.ReadLine();
        }

        private static string SurroundWith(string str, char surroundWith)
        {
            return $"{surroundWith}{str}{surroundWith}";
        }
    }
}
