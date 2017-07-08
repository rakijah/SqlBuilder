using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="table">The name of the table you want to delete from.</param>
        /// <returns></returns>
        public BuiltDeleteCommand From(string table)
        {
            _table = table;
            return this;
        }

        public string Generate()
        {
            if (string.IsNullOrEmpty(_table))
                throw new Exception("Table is not set.");

            StringBuilder sb = new StringBuilder($"DELETE FROM {_table}");
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
