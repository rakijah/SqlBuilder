using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SqlBuilder
{
    /// <summary>
    /// Fetches datasets from a database and provides them as instances of classes that use the SqlBuilder.Attributes attributes.
    /// </summary>
    public class EntityFetcher
    {
        private readonly DbConnection _connection;

        /// <summary>
        /// Creates a new EntityFetcher.<para />
        /// (Currently doesn't check whether the connection is valid or opened.)
        /// </summary>
        /// <param name="connection">The database connection to be used.</param>
        public EntityFetcher(DbConnection connection)
        {
            _connection = connection;
        }
        /*
        /// <summary>
        /// Returns a reader for the specified amount of rows for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <param name="amountOfRows">The amount of rows to be returned (maximum). Leave at -1 to query all rows.</param>
        /// <returns>A DbDataReader that has tried to query rows for entities of type T.</returns>
        internal DbDataReader GetReader<T>(BuiltSqlCondition condition = null, int amountOfRows = -1)
        {
            return GetReader(typeof(T), condition, amountOfRows);
        }

        /// <summary>
        /// Returns a reader for the specified amount of rows for the specified entity type.
        /// </summary>
        /// <param name="entityType">The type of entity to be queried.</param>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <param name="amountOfRows">The amount of rows to be returned (maximum). Leave at -1 to query all rows.</param>
        /// <returns>A DbDataReader that has tried to query rows for entities of type T.</returns>
        internal DbDataReader GetReader(Type entityType, BuiltSqlCondition condition = null, int amountOfRows = -1)
        {
            var select = SqlBuild.Select(entityType);

            if (condition != null)
                select.Condition = condition;

            if (amountOfRows > 0)
                select.Limit(amountOfRows);

            return select.GenerateCommand(_connection).ExecuteReader();
        }

        /// <summary>
        /// Returns a reader for the specified amount of rows for the specified entity type.
        /// </summary>
        /// <param name="entityType">The type of entity to be queried.</param>
        /// <param name="commandString">The command to execute to create the reader.</param>
        /// <returns>A DbDataReader that has tried to query rows for entities of type entityType.</returns>
        internal DbDataReader GetReader(Type entityType, string commandString)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = commandString;
                return command.ExecuteReader();
            }
        }
        */
        /// <summary>
        /// Fetches and initializes all available rows for entities of type T.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <returns>An enumerable of constructed entities of type T.</returns>
        public IEnumerable<T> All<T>(BuiltSqlCondition condition = null) where T : new()
        {
            var command = _connection.CreateCommand();
            command.CommandText = SqlBuild.Select<T>().GenerateStatement();
            condition?.GenerateCommand(command);

            var reader = command.ExecuteReader();
            List <T> resultSet = new List<T>();
            while (reader.Read())
            {
                resultSet.Add(BuildSingle<T>(reader));
            }
            command.Dispose();
            return resultSet;
        }

        /// <summary>
        /// Fetches and initializes the specified number of rows for entities of type T.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <param name="amountOfRows">The amount of rows to be queried (maximum).</param>
        /// <returns>An enumerable of constructed entities of type T.</returns>
        public IEnumerable<T> Fetch<T>(BuiltSqlCondition condition, int amountOfRows = -1) where T : new()
        {
            
            var select = SqlBuild.Select<T>();
            select.Condition = condition;
            if (amountOfRows != -1)
            {
                if (ProviderSpecific.SupportsLimit)
                {
                    select.Limit(amountOfRows);
                }
            }
                
            var command = select.GenerateCommand(_connection);

            var reader = command.ExecuteReader();
            List<T> resultSet = new List<T>();
            if (!ProviderSpecific.SupportsLimit && amountOfRows != -1)
            {
                int rows = 0;
                while (reader.Read() && rows < amountOfRows)
                {
                    resultSet.Add(BuildSingle<T>(reader));
                    rows++;
                }
            }
            else
            {
                while (reader.Read())
                {
                    resultSet.Add(BuildSingle<T>(reader));
                }
            }
            
            command.Dispose();
            return resultSet;

            //var reader = GetReader<T>(condition, amountOfRows);
            //List<T> resultSet = new List<T>();
            while (reader.Read())
            {
                resultSet.Add(BuildSingle<T>(reader));
            }
            return resultSet;
        }

        /// <summary>
        /// Fetches and initializes a single row for entities of type T.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <returns>The constructed entity of type T.</returns>
        public T Single<T>(BuiltSqlCondition condition = null) where T : new()
        {
            return Fetch<T>(condition, 1).FirstOrDefault();
        }

        /// <summary>
        /// Takes the current row that the reader is on and uses it to construct a new entity of type T.<para />
        /// Does not alter the reader's cursor at all.
        /// May return an incomplete entity in case of failure.
        /// </summary>
        /// <typeparam name="T">The type of entity to be constructed.</typeparam>
        /// <param name="reader">The reader to be used. Make sure .HasRows is true before passing this in.</param>
        /// <returns>The constructed entity of type T.</returns>
        internal T BuildSingle<T>(DbDataReader reader)
            where T : new()
        {

            return (T)BuildSingle(typeof(T), reader);
        }

        /// <summary>
        /// Takes the current row that the reader is on and uses it to construct a new entity of type T.<para />
        /// Does not alter the reader's cursor at all.
        /// May return an incomplete entity in case of failure.
        /// </summary>
        /// <param name="tableType">The type of entity to be constructed.</param>
        /// <param name="reader">The reader to be used. Make sure .HasRows is true before passing this in.</param>
        /// <returns>The constructed entity of type T.</returns>
        internal object BuildSingle(Type tableType, DbDataReader reader)
        {
            var propToCol = SqlTableHelper.PropertiesToColumnNames(tableType);
            var entity = Activator.CreateInstance(tableType);
            foreach (var property in propToCol)
            {

                var sqlColumnAttribute = property.Key.GetCustomAttribute<SqlColumn>();
                if (sqlColumnAttribute != null && property.Key.GetCustomAttribute<SqlForeignKey>() == null)
                {
                    switch (sqlColumnAttribute.Type)
                    {
                        case DbType.Int16:
                        case DbType.Int32:
                        case DbType.Int64:
                            property.Key.SetValue(entity, Convert.ToInt32(reader[property.Value]), null);
                            break;
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                            string value = reader[property.Value].ToString();
                            property.Key.SetValue(entity, value, null);
                            break;
                        case DbType.Date:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                            string stringValue = reader[property.Value].ToString();
                            DateTime dateValue = DateTime.ParseExact(stringValue, ProviderSpecific.DotNetDateFormatString, CultureInfo.InvariantCulture);
                            property.Key.SetValue(entity, dateValue, null);
                            break;
                        default:
                            throw new Exception("Unsupported column type.");
                    }
                }

                if (sqlColumnAttribute == null)
                    continue;
                var sqlForeignKeyAttributes = property.Key.GetCustomAttributes<SqlForeignKey>();
                foreach (var foreignKey in sqlForeignKeyAttributes)
                {
                    Type foreignTable = foreignKey.ForeignTable;
                    var properties = foreignTable.GetProperties();
                    foreach (var prop in properties)
                    {
                        var attrib = prop.GetCustomAttribute<SqlColumn>();
                        if (attrib == null)
                            continue;
                        if (attrib.ColumnName == foreignKey.ForeignTableColumnName)
                        {
                            var foreignSelect = SqlBuild.Select(foreignTable)
                                .Join(tableType, foreignTable, sqlColumnAttribute.ColumnName, foreignKey.ForeignTableColumnName)
                                .Where(
                                    new BuiltSqlCondition()
                                        .AddCondition(tableType, sqlColumnAttribute.ColumnName, "=", (string)reader[sqlColumnAttribute.ColumnName], sqlColumnAttribute.Type));
                            var foreignReader = foreignSelect.GenerateCommand(_connection).ExecuteReader();
                            if (!foreignReader.Read())
                                continue;

                            var foreignEntity = BuildSingle(foreignTable, foreignReader);
                            property.Key.SetValue(entity, foreignEntity);
                            break;
                        }
                    }
                }
            }
            return entity;
        }
    }
}