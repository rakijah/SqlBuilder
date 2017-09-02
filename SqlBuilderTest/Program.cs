using SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SqlBuilderTest
{
    internal class Program
    {
        private static void Main()
        {
            SqlBuild.Configure(
                    new SqlBuildOptions
                    {
                        Provider = DatabaseProvider.Oracle10GOrLater
                    }
                );

            var createTableSql = SqlBuild.CreateTable<Users>();
            Console.WriteLine(createTableSql.GenerateStatement() + Environment.NewLine);

            var selectSql = SqlBuild.Select<Users>(false)
                                         .AddColumns<Users>("username", "email")
                                         .Join<Addresses, Users>("userid", "id")
                                         .AddColumns<Addresses>("postcode", "street");
            selectSql.Where(new BuiltSqlCondition()
                            .BeginBlock()
                            .AddCondition<Users>("lastname", "=", "Jones", DbType.String)
                            .And()
                            .AddCondition<Addresses>("state", "=", "Ohio", DbType.String)
                            .EndBlock()
                            .Or()
                            .AddCondition<Users>("id", ">", "50", DbType.Int32))
                        .OrderBy()
                        .SortBy<Users>("firstname", SqlSortMode.DESCENDING)
                        .Finish();
            Console.WriteLine(selectSql + Environment.NewLine);

            var insertSql = SqlBuild.InsertInto<Users>("username", "password", "email")
                                        .AddValues(
                                            new BuiltValueList<Users>(new List<string> {"username", "password", "email"})
                                            .AddValueFor("username", "rakijah")
                                            .AddValueFor("password", "dGhlIGdhbWU=")
                                            .AddValueFor("email", "rakijah@fakemail.com")
                                         )
                                        .AddRow("user2", "ZmVsbCBmb3IgaXQgYWdhaW4=", "user2@fakemail.com");

            Console.WriteLine(insertSql + Environment.NewLine);

            var deleteSql = SqlBuild.Delete()
                                        .From<Users>()
                                        .Where(new BuiltSqlCondition().AddCondition<Users>("id", "<", "10", DbType.Int32));
            
            Console.WriteLine(deleteSql + Environment.NewLine);

            var alterTableSql = SqlBuild.AlterTable<Users>()
                                                    .Add("firstname", "varchar(50)")
                                                    .Add("lastname", "varchar(50)")
                                                    .Drop("fullname")
                                                    .ChangeColumnType("username", "VARCHAR(255)");

            Console.WriteLine(alterTableSql + Environment.NewLine);
            
            Console.ReadLine();

            /*
            SqlConnection connection = new SqlConnection("Data Source=Example;User Id=admin;password=password;");
            connection.Open();
            var cmd = selectSql.GenerateCommand(connection);
            Console.WriteLine($"CommandText: {cmd.CommandText}");
            foreach (DbParameter param in cmd.Parameters)
            {
                Console.WriteLine($"Name: {param.ParameterName} Value: {param.Value}, Type: {param.DbType}");
            }
            Console.ReadLine();
            
            EntityFetcher fetcher = new EntityFetcher(connection);
            var usersBetween20And40 = fetcher.All<Users>(
                    new BuiltSqlCondition()
                                .AddCondition<Users>("id", ">", "20")
                                .And()
                                .AddCondition<Users>("id", "<", "40")
                );

            foreach(var user in usersBetween20And40)
            {
                Console.WriteLine($"Name: {user.FirstName} {user.LastName}, Email: {user.Email}");
            }
            Console.ReadLine();
            //*/
        }
    }
}