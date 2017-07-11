using System;
using SqlBuilder;
using System.IO;

namespace SqlBuilderTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SqlBuild.Configure(DatabaseProvider.OracleBefore10G, true);
            
            var selectSql = SqlBuild.Select<Users>(false)
                                         .AddColumns<Users>("username", "email")
                                         .Join<Addresses, Users>("userid", "id")
                                         .AddColumns<Addresses>("postcode", "street")
                                         .Where()
                                            .BeginBlock()
                                            .AddCondition<Users>("lastname", "=", "Jones", '"')
                                            .And()
                                            .AddCondition<Addresses>("state", "=", "Ohio", '"')
                                            .EndBlock()
                                            .Or()
                                            .AddCondition<Users>("id", ">", "50", '"')
                                            .Finish()
                                         .OrderBy()
                                            .SortBy<Users>("firstname", SqlSortMode.DESCENDING)
                                            .Finish();
            Console.WriteLine(selectSql + Environment.NewLine);

            var insertSql = SqlBuild.Insert()
                                        .Into<Users>("username", "password", "email")
                                        .AddValues()
                                            .AddValueFor("username", "rakijah", '"')
                                            .AddValueFor("password", "dGhlIGdhbWU=", '"')
                                            .AddValueFor("email", "rakijah@fakemail.com", '"')
                                            .Finish()
                                        .AddRow(SurroundWith("user2", '"'), SurroundWith("ZmVsbCBmb3IgaXQgYWdhaW4=", '"'), SurroundWith("user2@fakemail.com", '"'));

            Console.WriteLine(insertSql + Environment.NewLine);

            var deleteSql = SqlBuild.Delete()
                                        .From<Users>()
                                        .Where()
                                            .AddCondition<Users>("id", "<", "10")
                                            .Finish();
            Console.WriteLine(deleteSql + Environment.NewLine);

            var alterTableSql = SqlBuild.AlterTable<Users>()
                                                    .Add("firstname", "varchar(50)", true)
                                                    .Add("lastname", "varchar(50)", true)
                                                    .Drop("fullname")
                                                    .ChangeColumnType("username", "VARCHAR(255)");
            
            Console.WriteLine(alterTableSql + Environment.NewLine);
            Console.ReadLine();
        }

        private static string SurroundWith(string str, char surroundWith)
        {
            return $"{surroundWith}{str}{surroundWith}";
        }
    }
}
