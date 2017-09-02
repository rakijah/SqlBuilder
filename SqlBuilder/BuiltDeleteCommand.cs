using System;
using System.Data.Common;
using System.Text;

namespace SqlBuilder
{
    public class BuiltDeleteCommand
    {
        private BuiltSqlCondition _condition;
        private string _table;

        internal BuiltDeleteCommand()
        {
        }

        /// <summary>
        /// Adds a condition to apply to this DELETE command.
        /// </summary>
        public BuiltDeleteCommand Where(BuiltSqlCondition condition)
        {
            _condition = condition;
            return this;
        }

        /// <summary>
        /// Specify the table to delete from.
        /// </summary>
        /// <typeparam name="T">The table you want to delete from.</typeparam>
        public BuiltDeleteCommand From<T>()
        {
            _table = SqlTableHelper.GetTableName<T>();
            return this;
        }

        /// <summary>
        /// Generates the DELETE command string.
        /// </summary>
        public string GenerateStatement()
        {
            if (string.IsNullOrEmpty(_table))
                throw new Exception("Table is not set.");

            StringBuilder sb = new StringBuilder($"DELETE FROM {Util.FormatSQL(_table)}");
            if (_condition != null)
            {
                sb.Append($" {_condition.GenerateStatement()}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a DELETE command from the provided connection using DbParameters.
        /// </summary>
        /// <param name="command"></param>
        public DbCommand GenerateCommand(DbConnection connection)
        {
            if (string.IsNullOrEmpty(_table))
                throw new Exception("Table is not set.");
            var command = connection.CreateCommand();
            command.CommandText += $"DELETE FROM {Util.FormatSQL(_table)} ";
            if (_condition != null)
            {
                _condition.GenerateCommand(command);
                command.CommandText += " ";
            }
            return command;
        }

        public override string ToString()
        {
            return GenerateStatement();
        }
    }
}