using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    /// <summary>
    /// Generates an UPDATE command.
    /// </summary>
    /// <typeparam name="T">The table that should be updated.</typeparam>
    public class BuiltUpdateCommand<T>
    {
        private readonly List<ColumnWithValue> _newColumnValues;
        private BuiltSqlCondition _condition;

        public BuiltUpdateCommand()
        {
            _newColumnValues = new List<ColumnWithValue>();
        }

        /// <summary>
        /// Sets the value for a specified column.
        /// </summary>
        /// <param name="column">The column to be updated.</param>
        /// <param name="value">The value to be used in the update.</param>
        public BuiltUpdateCommand<T> Set(string column, string value, DbType valueType)
        {
            if (_newColumnValues.All(cwv => cwv.Column != column))
                _newColumnValues.Add(new ColumnWithValue {Column = column, Value = value, ValueType = valueType});
            else
            {
                var col = _newColumnValues.Find(cwv => cwv.Column == column);
                _newColumnValues.Remove(col);
                col.Value = value;
                col.ValueType = valueType;
                _newColumnValues.Add(col);
            }
            
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
        public string GenerateStatement()
        {
            if (_newColumnValues.Count == 0)
                throw new Exception("Can't update table without columns to be updated.");

            StringBuilder sb = new StringBuilder($"UPDATE {SqlTableHelper.GetTableName<T>()} SET ");
            for (int i = 0; i < _newColumnValues.Count; i++)
            {
                var colVal = _newColumnValues[i];

                sb.Append($"{Util.FormatSQL(colVal.Column)}={colVal.Value}");
                if (i < _newColumnValues.Count - 1)
                    sb.Append(", ");
            }

            sb.Append(_condition?.GenerateStatement());
            return sb.ToString();
        }

        /// <summary>
        /// Creates an UPDATE command from the provided connection using DbParameters.
        /// </summary>
        /// <param name="command"></param>
        public DbCommand GenerateCommand(DbConnection connection)
        {
            if (_newColumnValues.Count == 0)
                throw new Exception("Can't update table without columns to be updated.");
            var command = connection.CreateCommand();
            StringBuilder sb = new StringBuilder($"UPDATE {SqlTableHelper.GetTableName<T>()} SET ");
            for (int i = 0; i < _newColumnValues.Count; i++)
            {
                var colVal = _newColumnValues[i];
                var param = command.CreateParameter();
                var paramName = Util.GetUniqueParameterName();
                param.ParameterName = paramName;
                param.Value = colVal.Value;
                param.DbType = colVal.ValueType;
                command.Parameters.Add(param);

                sb.Append($"{Util.FormatSQL(colVal.Column)}={ProviderSpecific.ParameterPrefix}{paramName}");
                if (i < _newColumnValues.Count - 1)
                    sb.Append(", ");
            }
            command.CommandText += sb.ToString();
            _condition?.GenerateCommand(command);
            return command;
        }

        public override string ToString()
        {
            return GenerateStatement();
        }
    }

    internal struct ColumnWithValue
    {
        public string Column { get; set; }
        public string Value { get; set; }
        public DbType ValueType { get; set; }
    }
}
