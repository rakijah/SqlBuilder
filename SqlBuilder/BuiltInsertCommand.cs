using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    /// <summary>
    /// Generates an INSERT INTO command.
    /// </summary>
    /// <typeparam name="T">The table to insert into.</typeparam>
    public class BuiltInsertCommand<T>
    {
        private readonly string _table;
        private readonly List<string> _columns;
        private readonly List<BuiltValueList<T>> _rowValues;

        internal BuiltInsertCommand()
        {
            _rowValues = new List<BuiltValueList<T>>();
            _table = SqlTableHelper.GetTableName<T>();
            _columns = SqlTableHelper.GetColumnNames<T>();
        }

        internal BuiltInsertCommand(params string[] columns)
        {
            _rowValues = new List<BuiltValueList<T>>();
            _table = SqlTableHelper.GetTableName<T>();
            _columns = new List<string>(columns);
        }

        public BuiltInsertCommand<T> AddItem(T item)
        {
            var value = new BuiltValueList<T>(_columns);
            foreach (var p2c in SqlTableHelper.PropertiesToColumnNames<T>())
            {
                if (!_columns.Contains(p2c.Value))
                    continue;
                
                value.AddValueFor(p2c.Value, p2c.Key.GetValue(item, null).ToString());
            }
            return this;
        }

        /// <summary>
        /// Add a row of values to this INSERT command.
        /// </summary>
        public BuiltInsertCommand<T> AddValues(BuiltValueList<T> values)
        {
            _rowValues.Add(values);
            return this;
        }

        /// <summary>
        /// Adds a row to this INSERT command by providing values for every column (in the correct order).
        /// This function does not allow you to surround the values with a specific character, though.
        /// </summary>
        /// <param name="orderedValues">The values to be inserted.</param>
        public BuiltInsertCommand<T> AddRow(params string[] orderedValues)
        {
            if (_columns == null)
                throw new Exception("Use Into to initialise the table and columns before calling this function.");

            if (orderedValues.Length != _columns.Count)
                throw new Exception("Column count does not match amount of specified values.");

            var row = new BuiltValueList<T>(_columns);
            for (int i = 0; i < orderedValues.Length; i++)
            {
                row.AddValueFor(_columns[i], orderedValues[i]);
            }
            _rowValues.Add(row);
            return this;
        }

        /// <summary>
        /// Generates the INSERT command string, starting with "INSERT INTO" and always ending with a ")".
        /// </summary>
        public string GenerateStatement()
        {
            if (string.IsNullOrWhiteSpace(_table))
                throw new Exception("Use Into to initialise the table and columns before calling ToString.");

            if (_columns == null || _columns.Count == 0)
                throw new Exception("Use Into to initialise the columns of the table before calling ToString.");

            if (_rowValues.Count == 0)
                throw new Exception("Use CreateValues before calling ToString.");

            StringBuilder sb = new StringBuilder($"INSERT INTO {Util.FormatSQL(_table)} (");
            var colFormatted = _columns.Select(Util.FormatSQL).ToList().Zip(", ");
            sb.Append(colFormatted);
            sb.Append(") VALUES ");
            for (int i = 0; i < _rowValues.Count; i++)
            {
                sb.Append(_rowValues[i].GenerateStatement());
                if (i != _rowValues.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates an INSERT INTO command from the provided connection using DbParameters.
        /// </summary>
        public DbCommand GenerateCommand(DbConnection connection)
        {
            if (string.IsNullOrWhiteSpace(_table))
                throw new Exception("Use Into to initialise the table and columns before calling ToString.");

            if (_columns == null || _columns.Count == 0)
                throw new Exception("Use Into to initialise the columns of the table before calling ToString.");

            if (_rowValues.Count == 0)
                throw new Exception("Use CreateValues before calling ToString.");
            var command = connection.CreateCommand();
            StringBuilder sb = new StringBuilder($"INSERT INTO {Util.FormatSQL(_table)} (");
            var colFormatted = _columns.Select(Util.FormatSQL).ToList().Zip(", ");
            sb.Append(colFormatted);
            sb.Append(") VALUES ");
            command.CommandText += sb.ToString();
            
            for (int i = 0; i < _rowValues.Count; i++)
            {
                _rowValues[i].GenerateCommand(command);
                if (i != _rowValues.Count - 1)
                    command.CommandText += ", ";
            }
            return command;
        }

        public override string ToString()
        {
            return GenerateStatement();
        }
    }
}