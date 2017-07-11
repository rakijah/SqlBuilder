using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltInsertCommand
    {
        private string _table;
        private List<string> _columns;
        private List<BuiltInsertValue> _rowValues;

        internal BuiltInsertCommand()
        {
            _rowValues = new List<BuiltInsertValue>();
        }

        /// <summary>
        /// Specify the table to insert into and its column names.
        /// </summary>
        /// <typeparam name="T">The table to insert into.</typeparam>
        /// <param name="columns">The names of the columns you wish to specify values for.</param>
        public BuiltInsertCommand Into<T>(params string[] columns)
        {
            _table = SqlTable.GetTableName<T>();
            _columns = new List<string>(columns);
            return this;
        }

        /// <summary>
        /// Specify the table to insert into. Assumes you want to use all columns for this insert.
        /// </summary>
        /// <typeparam name="T">The table to insert into.</typeparam>
        public BuiltInsertCommand Into<T>()
        {
            _table = SqlTable.GetTableName<T>();
            _columns = SqlTable.GetColumnNames<T>();
            return this;
        }

        /// <summary>
        /// Add a row of values to this INSERT command.
        /// Must be called after Into().
        /// </summary>
        public BuiltInsertValue AddValues()
        {
            if (_columns == null)
                throw new Exception("Use Into to initialise the table and columns before calling this function.");

            var newRow = new BuiltInsertValue(this, _columns);
            _rowValues.Add(newRow);
            return newRow;
        }
        
        /// <summary>
        /// Adds a row to this INSERT command by providing values for every column (in the correct order).
        /// This function does not allow you to surround the values with a specific character, though.
        /// </summary>
        /// <param name="orderedValues">The values to be inserted.</param>
        public BuiltInsertCommand AddRow(params string[] orderedValues)
        {
            if (_columns == null)
                throw new Exception("Use Into to initialise the table and columns before calling this function.");

            if (orderedValues.Length != _columns.Count)
                throw new Exception("Column count does not match amount of specified values.");


            var row = new BuiltInsertValue(this, _columns);
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
                var row = _rowValues[i];
                sb.Append(row.Generate());
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
