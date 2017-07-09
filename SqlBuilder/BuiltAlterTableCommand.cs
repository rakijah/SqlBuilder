using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    public class BuiltAlterTableCommand
    {
        private string _table;
        private List<string> _components;

        internal BuiltAlterTableCommand(string table)
        {
            _table = table;
            _components = new List<string>();
        }

        /// <summary>
        /// Renames the table.
        /// </summary>
        /// <param name="to">The new name of the table.</param>
        public BuiltAlterTableCommand RenameTable(string to)
        {
            _components.Add($"RENAME {Util.FormatSQL(to)}");
            return this;
        }


        /// <summary>
        /// Deletes one or more columns.
        /// </summary>
        /// <param name="columns">The columns to be dropped.</param>
        public BuiltAlterTableCommand Drop(params string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                _components.Add($"DROP COLUMN {Util.FormatSQL(columns[i])}");
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
        public BuiltAlterTableCommand Add(string column, string type, bool nullable = true, bool autoIncrement = false)
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
        public BuiltAlterTableCommand AddPrimaryKey(string column)
        {
            _components.Add($"ADD PRIMARY KEY ({Util.FormatSQL(column)})");
            return this;
        }

        /// <summary>
        /// Alters the type of a column. Requires SqlBuild.Provider to be set.
        /// </summary>
        /// <param name="column">The column to be altered.</param>
        /// <param name="newType">The new type of the column.</param>
        public BuiltAlterTableCommand ChangeColumnType(string column, string newType)
        {
            string colAndType = $"{Util.FormatSQL(column)} {newType}";

            switch(SqlBuild.Provider)
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
                case DatabaseProvider.Unknown:
                    throw new Exception("SqlBuild.Provider needs to be set before calling this function.");
            }
            return this;
        }

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
