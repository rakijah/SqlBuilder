﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class BuiltInsertCommand
    {
        private string _table;
        private List<string> _columns;
        private BuiltInsertValue _values;

        internal BuiltInsertCommand()
        {

        }

        public BuiltInsertCommand Into(string table, params string[] columns)
        {
            _table = table;
            _columns = new List<string>(columns);
            return this;
        }

        public BuiltInsertValue CreateValues()
        {
            if (_columns == null)
                throw new Exception("Use Into to initialise the table and columns before calling CreateValues.");

            _values = new BuiltInsertValue(this, _columns);
            return _values;
        }
        
        public string Generate()
        {
            if (string.IsNullOrWhiteSpace(_table))
                throw new Exception("Use Into to initialise the table and columns before calling ToString.");

            if (_columns == null || _columns.Count == 0)
                throw new Exception("Use Into to initialise the columns of the table before calling ToString.");

            if (_values == null)
                throw new Exception("Use CreateValues before calling ToString.");

            StringBuilder sb = new StringBuilder($"INSERT INTO { _table } (");
            sb.Append(_columns.Zip(", "));
            sb.Append($") VALUES {_values.ToString()}");
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}