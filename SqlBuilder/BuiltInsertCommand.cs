using System;
using System.Collections.Generic;
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
        private string _table;
        private List<string> _columns;
        private List<BuiltInsertValue<T>> _rowValues;

        internal BuiltInsertCommand()
        {
            _rowValues = new List<BuiltInsertValue<T>>();
            _table = SqlTable.GetTableName<T>();
            _columns = SqlTable.GetColumnNames<T>();
        }

        internal BuiltInsertCommand(params string[] columns)
        {
            _rowValues = new List<BuiltInsertValue<T>>();
            _table = SqlTable.GetTableName<T>();
            _columns = new List<string>(columns);
        }

        /// <summary>
        /// Add a row of values to this INSERT command.
        /// </summary>
        public BuiltInsertValue<T> AddValues()
        {
            if (_columns == null)
                throw new Exception("Use Into to initialise the table and columns before calling this function.");

            var newRow = new BuiltInsertValue<T>(this, _columns);
            _rowValues.Add(newRow);
            return newRow;
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

            var row = new BuiltInsertValue<T>(this, _columns);
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
        public string Generate()
        {
            if (string.IsNullOrWhiteSpace(_table))
                throw new Exception("Use Into to initialise the table and columns before calling ToString.");

            if (_columns == null || _columns.Count == 0)
                throw new Exception("Use Into to initialise the columns of the table before calling ToString.");

            if (_rowValues.Count == 0)
                throw new Exception("Use CreateValues before calling ToString.");

            StringBuilder sb = new StringBuilder($"INSERT INTO {Util.FormatSQL(_table)} (");
            var colFormatted = _columns.Select(c => Util.FormatSQL(c)).ToList().Zip(", ");
            sb.Append(colFormatted);
            sb.Append(") VALUES ");
            for (int i = 0; i < _rowValues.Count; i++)
            {
                sb.Append(_rowValues[i].Generate());
                if (i != _rowValues.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}