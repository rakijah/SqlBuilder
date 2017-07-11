using System;
using System.Text;

namespace SqlBuilder
{
    public class BuiltDeleteCommand
    {
        private BuiltSqlCondition<BuiltDeleteCommand> _condition;
        private string _table;

        internal BuiltDeleteCommand()
        {

        }

        /// <summary>
        /// Start creation of a contidion to apply to this DELETE command.
        /// </summary>
        public BuiltSqlCondition<BuiltDeleteCommand> Where()
        {
            _condition = new BuiltSqlCondition<BuiltDeleteCommand>(this);
            return _condition;
        }

        /// <summary>
        /// Specify the table to delete from.
        /// </summary>
        /// <typeparam name="T">The table you want to delete from.</typeparam>
        public BuiltDeleteCommand From<T>()
        {
            _table = SqlTable.GetTableName<T>();
            return this;
        }

        /// <summary>
        /// Generates the DELETE command string. 
        /// </summary>
        public string Generate()
        {
            if (string.IsNullOrEmpty(_table))
                throw new Exception("Table is not set.");

            StringBuilder sb = new StringBuilder($"DELETE FROM {Util.FormatSQL(_table)}");
            if(_condition != null)
            {
                sb.Append($" {_condition.Generate()}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }

    }
}
