using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlBuilder
{
    public class BuiltSelectCommand
    {
        private List<string> _fromTables;
        private List<string> _selectedColumns;
        private List<SqlJoin> _joins;
        private BuiltSqlSort _sort;
        private BuiltSqlCondition<BuiltSelectCommand> _condition;
        
        internal BuiltSelectCommand()
        {
            _fromTables = new List<string>();
            _selectedColumns = new List<string>();
            _joins = new List<SqlJoin>();
        }

        /// <summary>
        /// Adds a table to used in the FROM clause.
        /// </summary>
        /// <typeparam name="T">The table to be added.</typeparam>
        /// <param name="selectAllColumns">Whether all columns from this table should be added to the selection.</param>
        public BuiltSelectCommand AddTable<T>(bool selectAllColumns = true)
        {
            string tableName = SqlTable.GetTableName<T>();
            _fromTables.Add(tableName);

            if (selectAllColumns)
                AddColumns<T>(SqlTable.GetColumnNames<T>().ToArray());

            return this;
        }

        /// <summary>
        /// Add columns to be selected.
        /// </summary>
        /// <typeparam name="T">The table containing the columns.</typeparam>
        /// <param name="columns">The columns to be selected.</param>
        public BuiltSelectCommand AddColumns<T>(params string[] columns)
        {
            string tableName = SqlTable.GetTableName<T>();
            if (!SqlTable.ContainsAllColumns<T>(columns))
                throw new Exception($"Table \"{SqlTable.GetTableName<T>()}\" does not contain all columns specified.");

            foreach (string column in columns)
            {
                string fullyQualified = $"{Util.FormatSQL(tableName, column)}";
                if(!_selectedColumns.Contains(fullyQualified))
                    _selectedColumns.Add(fullyQualified);
            }
            return this;
        }

        /// <summary>
        /// Add all columns from the specified table to be selected.
        /// </summary>
        /// <typeparam name="T">The table containing the columns.</typeparam>
        public BuiltSelectCommand AddColumns<T>()
        {
            foreach (string column in SqlTable.GetColumnNames<T>())
            {
                string fullyQualified = $"{Util.FormatSQL(SqlTable.GetTableName<T>(), column)}";
                if (!_selectedColumns.Contains(fullyQualified))
                    _selectedColumns.Add(fullyQualified);
            }
            return this;
        }

        /// <summary>
        /// !! UNSAFE !! Adds a directly specified string as a column. Use this to apply functions to columns before selecting.
        /// </summary>
        /// <param name="column">The column string to be added to the selection.</param>
        public BuiltSelectCommand AddColumnDirect(string column)
        {
            if (!_selectedColumns.Contains(column))
            {
                _selectedColumns.Add(column);
            }
            return this;
        }

        /// <summary>
        /// Adds a JOIN to this BuiltSelectCommand.
        /// </summary>
        /// <typeparam name="ToJoin">The new table of this join.</typeparam>
        /// <typeparam name="On">The existing table to be used for comparison.</typeparam>
        /// <param name="toJoinColumn">The column to use from the new table.</param>
        /// <param name="onColumn">The column to use from the existing table.</param>
        public BuiltSelectCommand Join<ToJoin, On>(string toJoinColumn, string onColumn)
        {
            if (!SqlTable.ContainsColumn<ToJoin>(toJoinColumn) ||
               !SqlTable.ContainsColumn<On>(onColumn))
                throw new Exception("The specified tables do not contain the specified columns.");


            _joins.Add(new SqlJoin
            {
                FirstTable = SqlTable.GetTableName<On>(),
                SecondTable = SqlTable.GetTableName<ToJoin>(),
                FirstColumn = onColumn,
                SecondColumn = toJoinColumn
            });
            return this;
        }

        /// <summary>
        /// Creates a WHERE clause for this BuiltSqlCommand. 
        /// Calling this twice on a BuiltSelectCommand will overwrite the first call.
        /// </summary>
        public BuiltSqlCondition<BuiltSelectCommand> Where()
        {
            _condition = new BuiltSqlCondition<BuiltSelectCommand>(this);
            return _condition;
        }

        /// <summary>
        /// Creates a sort for this BuiltSelectCommand.
        /// Calling this twice on a BuiltSelectCommand overwrites the first call.
        /// </summary>
        public BuiltSqlSort OrderBy()
        {
            _sort = new BuiltSqlSort(this);
            return _sort;
        }

        /// <summary>
        /// Generates the SELECT command string.
        /// </summary>
        public string Generate()
        {
            if (_fromTables.Count == 0)
                throw new FormatException("Use .AddTable at least once before generating the Sql_Server string.");

            StringBuilder sb = new StringBuilder("SELECT ");
            sb.Append(_selectedColumns.Count == 0 ? "*" : $"{_selectedColumns.Zip(", ")}");

            sb.Append($" FROM {_fromTables.Select(t => Util.FormatSQL(t)).ToList().Zip(", ")}");


            if (_condition != null)
            {
                sb.Append($" {_condition}");
            }
            if (_joins.Count > 0)
            {
                sb.Append($" {_joins.Select(j => j.ToString()).ToList().Zip(" ")}");
            }

            if (_sort != null)
            {
                sb.Append($" {_sort}");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }

    public struct SqlJoin
    {
        public string FirstTable { get; set; }
        public string SecondTable { get; set; }
        public string FirstColumn { get; set; }
        public string SecondColumn { get; set; }

        public override string ToString()
        {
            return $"JOIN {Util.FormatSQL(SecondTable)} ON {Util.FormatSQL(FirstTable, FirstColumn)}={Util.FormatSQL(SecondTable, SecondColumn)}";
        }
    }
}
