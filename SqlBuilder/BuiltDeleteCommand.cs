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

        public BuiltSqlCondition<BuiltDeleteCommand> Where()
        {
            _condition = new BuiltSqlCondition<BuiltDeleteCommand>(this);
            return _condition;
        }
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
