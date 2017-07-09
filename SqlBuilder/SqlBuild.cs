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
        /// Creates a new DELETE command.
        /// </summary>
        public static BuiltDeleteCommand Delete()
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltDeleteCommand();
        }

        public static BuiltAlterTableCommand AlterTable(string table)
        {
            if (!Initialized)
                throw new Exception("Configure() must be called before building commands.");

            return new BuiltAlterTableCommand(table);
        }
    }
}
