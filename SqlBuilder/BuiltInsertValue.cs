using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltInsertValue<T>
    {
        private List<string> _columns;
        private Dictionary<string, string> _values;
        private BuiltInsertCommand<T> _parent;
        private List<SqlColumn> _attributes;

        internal BuiltInsertValue(BuiltInsertCommand<T> parent, List<string> columns)
        {
            _parent = parent;
            _columns = new List<string>(columns);
            _values = new Dictionary<string, string>();
            _attributes = SqlTable.GetColumnAttributes<T>();
        }

        /// <summary>
        /// Adds a value for a specific column.
        /// </summary>
        /// <param name="column">The column that is going to hold the value.</param>
        /// <param name="value">The value to be inserted.</param>
        /// <param name="putAroundValue"></param>
        public BuiltInsertValue<T> AddValueFor(string column, string value)
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
        /// Returns the parent BuiltInsertCommand to allow for continuous chaining.
        /// </summary>
        public BuiltInsertCommand<T> Finish()
        {
            return _parent;
        }

        /// <summary>
        /// Generates the actual row value string, always starting with "(" and ending in ")".
        /// </summary>
        public string Generate()
        {
            if (!_columns.All(c => _values.ContainsKey(c)))
                throw new Exception("Please use AddValueFor to set a value for each column before generating the SQL command string.");

            StringBuilder sb = new StringBuilder("(");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_values[_columns[i]]);
                if (i < _columns.Count - 1)
                    sb.Append(", ");
            }

            sb.Append(")");
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}