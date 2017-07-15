using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    /// <summary>
    /// Generates an UPDATE command.
    /// </summary>
    /// <typeparam name="T">The table that should be updated.</typeparam>
    public class BuiltUpdateCommand<T>
    {
        private string _table;
        private Dictionary<string, string> _newColumnValues;
        private BuiltSqlCondition _condition;
        private List<SqlColumn> _attributes;

        public BuiltUpdateCommand()
        {
            _newColumnValues = new Dictionary<string, string>();
            _attributes = SqlTable.GetColumnAttributes<T>();
        }

        /// <summary>
        /// Sets the value for a specified column.
        /// </summary>
        /// <param name="column">The column to be updated.</param>
        /// <param name="value">The value to be used in the update.</param>
        public BuiltUpdateCommand<T> Set(string column, string value)
        {
            if (!_newColumnValues.ContainsKey(column))
                _newColumnValues.Add(column, value);
            else
                _newColumnValues[column] = value;
            
            return this;
        }

        /// <summary>
        /// Sets a condition to limit which datasets are updated.
        /// </summary>
        /// <param name="condition">The condition to be used.</param>
        public BuiltUpdateCommand<T> Where(BuiltSqlCondition condition)
        {
            _condition = condition;
            return this;
        }

        /// <summary>
        /// Generates the actual UPDATE command string.
        /// </summary>
        public string Generate()
        {
            if (_newColumnValues.Count == 0)
                throw new Exception("Can't update table without columns to be updated.");
            StringBuilder sb = new StringBuilder($"UPDATE {_table} SET ");
            foreach(var colVal in _newColumnValues)
            {
                var attr = _attributes.Single(x => x.ColumnName == colVal.Key);
                string formattedValue = attr.FormatValueFor(colVal.Value);
                sb.Append($"{Util.FormatSQL(colVal.Key)}={formattedValue}");
                if (colVal.Key != _newColumnValues.Last().Key)
                    sb.Append(", ");
            }

            if (_condition != null)
                sb.Append(_condition.Generate());
            return sb.ToString();
        }
    }
}
