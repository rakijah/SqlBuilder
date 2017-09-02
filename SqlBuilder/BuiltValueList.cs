using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltValueList<T>
    {
        private readonly List<ColumnWithValue> _columnsWithValues = new List<ColumnWithValue>();
        private List<SqlColumn> _columnAttributes = new List<SqlColumn>();

        private bool ContainsColumn(string column)
        {
            return _columnsWithValues.Any(x => x.Column == column);
        }

        /// <summary>
        /// Creates a new value list using the specified columns.
        /// </summary>
        public BuiltValueList(List<string> columns)
        {
            _columnAttributes = SqlTableHelper.GetColumnAttributes<T>();
            foreach (var col in columns)
            {
                SqlColumn columnAttribute = _columnAttributes.FirstOrDefault(attr => attr.ColumnName == col);
                if (columnAttribute == null)
                    throw new Exception($"Table \"{typeof(T).FullName}\" does not contain a column named \"{col}\"");

                _columnsWithValues.Add(new ColumnWithValue
                {
                    Column = col,
                    Value = "",
                    ValueType = columnAttribute.Type
                });
            }
        }

        /// <summary>
        /// Creates a new value list using all columns from the table.
        /// </summary>
        public BuiltValueList()
        {
            _columnAttributes = SqlTableHelper.GetColumnAttributes<T>();
            foreach (var column in _columnAttributes)
            {
                _columnsWithValues.Add(new ColumnWithValue
                {
                    Column = column.ColumnName,
                    Value = "",
                    ValueType = column.Type
                });
            }
        }

        /// <summary>
        /// Checks whether a value for the specified column has been set.
        /// </summary>
        /// <param name="column">The column to be checked.</param>
        /// <returns></returns>
        public bool ContainsValueFor(string column)
        {
            return _columnsWithValues.Any(col => col.Column == column && !string.IsNullOrWhiteSpace(col.Value));
        }

        /// <summary>
        /// Adds or sets a value for a specific column.
        /// </summary>
        /// <param name="column">The column that is going to hold the value.</param>
        /// <param name="value">The value to be inserted.</param>
        public BuiltValueList<T> AddValueFor(string column, string value)
        {
            if (_columnsWithValues.All(col => col.Column != column))
                throw new Exception($"This BuiltInsertValue does not contain a column named \"{column}\"");

            if (SqlTableHelper.GetColumnNames<T>().All(columnName => columnName != column))
                throw new Exception($"Table \"{typeof(T).FullName}\" does not contain a column named \"{column}\"");

            if (ContainsColumn(column))
            {
                var columnWithValue = _columnsWithValues.Single(col => col.Column == column);
                _columnsWithValues.Remove(columnWithValue);
                columnWithValue.Value = value;
                _columnsWithValues.Add(columnWithValue);
            }
            else
            _columnsWithValues.Add(new ColumnWithValue
            {
                Column = column,
                Value = value
            });

            return this;
        }

        /// <summary>
        /// Generates the actual row value string, optionally surrounded with "()".
        /// </summary>
        public string GenerateStatement(bool surroundWithParentheses = true)
        {
            if (_columnsWithValues.Any(col => string.IsNullOrWhiteSpace(col.Value)))
                throw new Exception("Please use AddValueFor to set a value for each column before generating the SQL command string.");

            StringBuilder sb = new StringBuilder();
            
            if (surroundWithParentheses)
                sb.Append("(");
            for (int i = 0; i < _columnsWithValues.Count; i++)
            {
                var colVal = _columnsWithValues[i];
                sb.Append(colVal.Value);
                if (i < _columnsWithValues.Count - 1)
                    sb.Append(", ");
            }
            if (surroundWithParentheses)
                sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Appends a comma separated value list to the provided DbCommand using DbParameters, optionally surrounded with parentheses.
        /// </summary>
        public void GenerateCommand(DbCommand command, bool surroundWithParentheses = true)
        {
            if (_columnsWithValues.Any(col => string.IsNullOrWhiteSpace(col.Value)))
                throw new Exception("Please use AddValueFor to set a value for each column before generating the SQL command string.");
            
            if (surroundWithParentheses)
                command.CommandText += "(";
            for (int i = 0; i < _columnsWithValues.Count; i++)
            {
                var colVal = _columnsWithValues[i];
                var param = command.CreateParameter();
                var paramName = Util.GetUniqueParameterName();
                param.ParameterName = paramName;
                param.Value = colVal.Value;
                param.DbType = colVal.ValueType;
                command.Parameters.Add(param);

                command.CommandText += paramName;
                if (i < _columnsWithValues.Count - 1)
                    command.CommandText += ", ";
            }
            if(surroundWithParentheses)
                command.CommandText += ")";
        }
    }
}