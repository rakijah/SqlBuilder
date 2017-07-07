using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBuilder;
using System.IO;

namespace SqlBuildTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string tableUsers = "users";
            string tableAddresses = "adresses";
            var selectSql = SqlBuild.Select()
                                         .AddColumns(tableUsers, "username", "email")
                                         .AddTable(tableUsers)
                                         .AddJoin(tableUsers, "id", tableAddresses, "userid")
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
            Console.ReadKey();

            var insertSql = SqlBuild.Insert()
                                        .Into(tableUsers, "username", "password", "email")
                                        .CreateValues()
                                            .AddValueFor("username", "rakijah", '"')
                                            .AddValueFor("password", "dGhlIGdhbWU=", '"')
                                            .AddValueFor("email", "rakijah@fakemail.com")
                                            .Finish();
            Console.WriteLine(insertSql);
            Console.ReadKey();

            var deleteSql = SqlBuild.Delete()
                                        .From(tableUsers)
                                        .Where()
                                            .AddCondition(tableUsers, "id", "10", "<")
                                            .Finish();
            Console.WriteLine(deleteSql);
            Console.ReadKey();
        }
    }
}
