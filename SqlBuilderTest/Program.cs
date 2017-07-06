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
            var builtSql = SqlBuild.Build()
                                         .AddColumns(tableUsers, "username", "email")
                                         .AddTable(tableUsers)
                                         .AddJoin(tableUsers, "id", tableAddresses, "userid")
                                         .AddColumns(tableUsers, "postcode", "street")
                                         .CreateCondition()
                                            .BeginBlock()
                                            .AddCondition(tableUsers, "lastname", "Jones", "=", '"')
                                            .And()
                                            .AddCondition(tableAddresses, "state", "Ohio", ">")
                                            .EndBlock()
                                            .Or()
                                            .AddCondition(tableUsers, "id", "50", ">", '"')
                                            .Finish()
                                         .CreateSort()
                                            .SortBy(tableUsers, "firstname", SqlSortMode.DESCENDING)
                                            .Finish();
            string sql = builtSql.ToString();
            Console.WriteLine(sql);
            Console.ReadKey();
        }
    }
}
