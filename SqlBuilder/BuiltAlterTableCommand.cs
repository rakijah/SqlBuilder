using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    /// <summary>
    /// An ALTER TABLE command.
    /// </summary>
    /// <typeparam name="T">The table to be altered.</typeparam>
    public class BuiltAlterTableCommand<T>
    {
        private readonly string _table;
        private readonly List<string> _components;

        internal BuiltAlterTableCommand()
        {
            _table = SqlTable.GetTableName<T>();
            _components = new List<string>();
        }

        /// <summary>
        /// Renames the table.
        /// </summary>
        /// <param name="to">The new name of the table.</param>
        public BuiltAlterTableCommand<T> RenameTable(string to)
        {
            _components.Add($"RENAME {Util.FormatSQL(to)}");
            return this;
        }

        /// <summary>
        /// Deletes one or more columns.
        /// </summary>
        /// <param name="columns">The columns to be dropped.</param>
        public BuiltAlterTableCommand<T> Drop(params string[] columns)
        {
            foreach (string col in columns)
            {
                _components.Add($"DROP COLUMN {Util.FormatSQL(col)}");
            }
            return this;
        }

        /// <summary>
        /// Adds a column.
        /// </summary>
        /// <param name="column">The column name to be added.</param>
        /// <param name="type">The type of the column. For example "INT UNSIGNED".</param>
        /// <param name="nullable">Whether the column should allow NULL as a valid value.</param>
        /// <param name="autoIncrement">Whether the column should be auto incrementing.</param>
        public BuiltAlterTableCommand<T> Add(string column, string type, bool nullable = true, bool autoIncrement = false)
        {
            string result = $"ADD {Util.FormatSQL(column)} {type.ToUpper()}";
            if (!nullable)
                result += " NOT NULL";

            if (autoIncrement)
                result += " AUTO_INCREMENT";

            _components.Add(result);
            return this;
        }

        /// <summary>
        /// Adds a primary key.
        /// </summary>
        /// <param name="column">The column to be made a primary key.</param>
        public BuiltAlterTableCommand<T> AddPrimaryKey(string column)
        {
            _components.Add($"ADD PRIMARY KEY ({Util.FormatSQL(column)})");
            return this;
        }

        /// <summary>
        /// Alters the type of a column. Requires SqlBuild.Provider to be set.
        /// </summary>
        /// <param name="column">The column to be altered.</param>
        /// <param name="newType">The new type of the column.</param>
        public BuiltAlterTableCommand<T> ChangeColumnType(string column, string newType)
        {
            string colAndType = $"{Util.FormatSQL(column)} {newType}";

            switch (SqlBuild.Options.Provider)
            {
                case DatabaseProvider.SqlServer:
                case DatabaseProvider.MSAccess:
                    _components.Add($"ALTER COLUMN {colAndType}");
                    break;

                case DatabaseProvider.MySql:
                case DatabaseProvider.OracleBefore10G:
                    _components.Add($"MODIFY COLUMN {colAndType}");
                    break;

                case DatabaseProvider.Oracle10GOrLater:
                    _components.Add($"MODIFY {colAndType}");
                    break;

                case DatabaseProvider.SQLite:
                    throw new Exception("SQLite does not support altering columns.");
                default:
                    throw new Exception("SqlBuild.Provider needs to be set before calling this function.");
            }
            return this;
        }

        /// <summary>
        /// Generates the ALTER TABLE command string.
        /// </summary>
        public string Generate()
        {
            if (_components.Count == 0)
                throw new Exception("Can't create an ALTER TABLE command without any tasks.");

            StringBuilder sb = new StringBuilder($"ALTER TABLE {Util.FormatSQL(_table)} ");
            sb.Append(_components.Zip(", "));
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}