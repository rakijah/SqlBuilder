using System;

namespace SqlBuilder
{
    public static class SqlBuild
    {
        public static bool Initialized { get; internal set; }

        public static DatabaseProvider Provider { get; internal set; }
        public static bool UseSquareBrackets { get; internal set; }

        public static void Configure(DatabaseProvider provider, bool useSquareBrackets)
        {
            Provider = provider;
            UseSquareBrackets = useSquareBrackets;
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

        //* //
        public static BuiltSelectCommand Select<T>(bool selectAllColumns = true)
        {
            if(!Initialized)
                throw new Exception("Configure() must be called before building commands.");
            var bsc = new BuiltSelectCommand(); ;
            bsc.AddTable<T>(selectAllColumns);
            return bsc;
        }
        // */

        /// <summary>
        /// Creates a new INSERT command.
        /// </summary>
        public static BuiltInsertCommand Insert()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltInsertCommand();
        }

        /// <summary>
        /// Creates a new INSERT command to insert into the specified table.
        /// </summary>
        public static BuiltInsertCommand Insert<Into>()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");
            var bic = new BuiltInsertCommand();
            bic.Into<Into>();
            return bic;
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

        public static BuiltAlterTableCommand<T> AlterTable<T>()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltAlterTableCommand<T>();
        }
    }
}
