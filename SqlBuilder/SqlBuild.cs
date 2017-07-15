using System;

namespace SqlBuilder
{
    public static class SqlBuild
    {
        public static bool Initialized { get; internal set; }
        public static SqlBuildOptions Options { get; private set; }

        public static void Configure(SqlBuildOptions options = null)
        {
            if (options == null)
                Options = new SqlBuildOptions();
            else
                Options = options;
            Initialized = true;
        }

        /// <summary>
        /// Creates a new SELECT command.
        /// </summary>
        public static BuiltSelectCommand Select()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltSelectCommand();
        }

        /// <summary>
        /// Creates a new SELECT command.
        /// </summary>
        /// <typeparam name="T">The main table to be used in the select.</typeparam>
        /// <param name="selectAllColumns">Whether to add all columns in this table type to the select.</param>
        public static BuiltSelectCommand Select<T>(bool selectAllColumns = true)
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");
            var bsc = new BuiltSelectCommand();
            bsc.AddTable<T>(selectAllColumns);
            return bsc;
        }

        /// <summary>
        /// Creates a new CREATE TABLE command.
        /// </summary>
        /// <typeparam name="T">The table type to be created.</typeparam>
        /// <returns></returns>
        public static BuiltCreateCommand<T> CreateTable<T>()
        {
            return new BuiltCreateCommand<T>();
        }

        /// <summary>
        /// Creates a new INSERT command to insert into the specified table. Uses all columns in the table.
        /// </summary>
        public static BuiltInsertCommand<Table> InsertInto<Table>()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltInsertCommand<Table>();
        }

        /// <summary>
        /// Creates a new INSERT command to insert into the specified table.
        /// </summary>
        /// <typeparam name="Table">The table to insert into.</typeparam>
        /// <param name="columns">The columns to be used for the insert.</param>
        public static BuiltInsertCommand<Table> InsertInto<Table>(params string[] columns)
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltInsertCommand<Table>(columns);
        }

        /// <summary>
        /// Creates a new UPDATE command to update an existing value in the table.
        /// </summary>
        /// <typeparam name="Table">The table whose values are to be updated.</typeparam>
        public static BuiltUpdateCommand<Table> Update<Table>()
        {
            if(!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltUpdateCommand<Table>();
        }

        /// <summary>
        /// Creates a new DELETE command.
        /// </summary>
        public static BuiltDeleteCommand Delete()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");
            return new BuiltDeleteCommand();
        }

        /// <summary>
        /// Creates a new ALTER TABLE command.
        /// </summary>
        /// <typeparam name="T">The table to be altered.</typeparam>
        public static BuiltAlterTableCommand<T> AlterTable<T>()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltAlterTableCommand<T>();
        }
    }
}