using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltValueList<T>
    {
        private List<string> _columns;
        private Dictionary<string, string> _values;
        private List<SqlColumn> _attributes;

        /// <summary>
        /// Creates a new value list using the specified columns.
        /// </summary>
        public BuiltValueList(List<string> columns)
        {
            _columns = new List<string>(columns);
            _values = new Dictionary<string, string>();
            _attributes = SqlTable.GetColumnAttributes<T>();
        }

        /// <summary>
        /// Creates a new value list using all columns from the table.
        /// </summary>
        public BuiltValueList()
        {
            _columns = new List<string>(SqlTable.GetColumnNames<T>());
            _values = new Dictionary<string, string>();
            _attributes = SqlTable.GetColumnAttributes<T>();
        }

        /// <summary>
        /// Checks whether a value for the specified column has been set.
        /// </summary>
        /// <param name="column">The column to be checked.</param>
        /// <returns></returns>
        public bool ContainsValueFor(string column)
        {
            return _values.ContainsKey(column);
        }

        /// <summary>
        /// Adds or sets a value for a specific column.
        /// </summary>
        /// <param name="column">The column that is going to hold the value.</param>
        /// <param name="value">The value to be inserted.</param>
        /// <param name="putAroundValue"></param>
        public BuiltValueList<T> AddValueFor(string column, string value)
        {
            if (!_columns.Contains(column))
                throw new Exception($"This BuiltInsertValue does not contain a column named \"{column}\"");

            if (!_attributes.Any(x => x.ColumnName == column))
                throw new Exception($"Table \"{typeof(T).FullName}\" does not contain a column named \"{column}\"");

            var attr = _attributes.Single(x => x.ColumnName == column);
            string formattedValue = attr.FormatValueFor(value);

            if (_values.ContainsKey(column))
                _values[column] = formattedValue;
            else
                _values.Add(column, formattedValue);

            return this;
        }

        /// <summary>
        /// Generates the actual row value string, optionally surrounded with "()".
        /// </summary>
        public string Generate(bool surroundWithParentheses = true)
        {
            if (!_columns.All(c => _values.ContainsKey(c)))
                throw new Exception("Please use AddValueFor to set a value for each column before generating the SQL command string.");

            StringBuilder sb = new StringBuilder();
            if (surroundWithParentheses)
                sb.Append("(");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_values[_columns[i]]);
                if (i < _columns.Count - 1)
                    sb.Append(", ");
            }
            if(surroundWithParentheses)
                sb.Append(")");
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}