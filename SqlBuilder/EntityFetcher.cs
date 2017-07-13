using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace SqlBuilder
{
    /// <summary>
    /// Fetches datasets from a database and provides them as instances of classes that use the SqlBuilder.Attributes attributes.
    /// </summary>
    public class EntityFetcher
    {
        private DbConnection _connection;

        /// <summary>
        /// Creates a new EntityFetcher.<para />
        /// (Currently doesn't check whether the connection is valid or opened.)
        /// </summary>
        /// <param name="connection">The database connection to be used.</param>
        public EntityFetcher(DbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Returns a reader for the specified amount of rows for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <param name="amountOfRows">The amount of rows to be returned (maximum). Leave at -1 to query all rows.</param>
        /// <returns>A DbDataReader that has tried to query rows for entities of type T.</returns>
        internal DbDataReader GetReader<T>(BuiltSqlCondition condition = null, int amountOfRows = -1)
        {
            var select = SqlBuild.Select<T>();

            if (condition != null)
                select.Condition = condition;

            if (amountOfRows > 0)
                select.Limit(amountOfRows);

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = select.Generate();
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Fetches and initializes all available rows for entities of type T.
        /// </summary>
        /// <typeparam name="T">The type of entity to be queried.</typeparam>
        /// <param name="condition">An optional condition to allow querying for specific rows. Leave at null to query all rows.</param>
        /// <returns>An enumerable of constructed entities of type T.</returns>
        public IEnumerable<T> All<T>(BuiltSqlCondition condition = null) where T : new()
        {
            var reader = GetReader<T>(condition);
            List<T> resultSet = new List<T>();
            while (reader.Read())
            {
                resultSet.Add(BuildSingle<T>(reader));
            }

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
            var reader = GetReader<T>(condition, amountOfRows);
            List<T> resultSet = new List<T>();
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
            var reader = GetReader<T>(condition, 1);
            if (!reader.HasRows)
                return new T();

            return BuildSingle<T>(reader);
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
            var propToCol = SqlTable.PropertiesToColumnNames<T>();
            T entity = new T();
            try
            {
                foreach (var property in propToCol)
                {
                    switch (property.Key.GetCustomAttribute<SqlColumn>().Type)
                    {
                        case SqlColumnType.Integer:
                            property.Key.SetValue(entity, Convert.ToInt32(reader[property.Value]), null);
                            break;

                        case SqlColumnType.String:
                            string value = reader[property.Value].ToString();
                            property.Key.SetValue(entity, value, null);
                            break;

                        case SqlColumnType.Date:
                            string stringValue = reader[property.Value].ToString();
                            DateTime dateValue = DateTime.ParseExact(stringValue, ProviderSpecific.DotNetDateFormatString, CultureInfo.InvariantCulture);
                            property.Key.SetValue(entity, dateValue, null);
                            break;

                        default:
                            throw new Exception("Unsupported column type.");
                    }
                }
            }
            catch (Exception) { }
            return entity;
        }
    }
}