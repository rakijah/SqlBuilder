using SqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltCreateCommand<T>
    {
        private readonly string _tableName;
        private readonly List<SqlColumn> _columns;
        private string _primaryKeyColumn;

        /// <summary>
        /// Creates a CREATE command from the table class template. Uses the first int column as primary key.
        /// </summary>
        public BuiltCreateCommand()
        {
            _tableName = SqlTable.GetTableName<T>();
            _columns = SqlTable.GetColumnAttributes<T>();
            if (_columns.Count == 0)
                throw new Exception("Can't create empty table.");
        }

        /// <summary>
        /// Set a column to be given the PRIMARY KEY property. Does not check column type. Calling this function a second time will overwrite the first call.
        /// </summary>
        /// <param name="columnName">The column to be made PRIMARY KEY.</param>
        public BuiltCreateCommand<T> SetPrimaryKey(string columnName)
        {
            if (_columns.All(x => x.ColumnName != columnName))
                throw new Exception("");

            _primaryKeyColumn = columnName;
            return this;
        }

        /// <summary>
        /// Generates the CREATE TABLE command string.
        /// </summary>
        public string Generate()
        {
            StringBuilder sb = new StringBuilder($"CREATE TABLE {_tableName}(");
            for (int i = 0; i < _columns.Count; i++)
            {
                var col = _columns[i];
                sb.Append($"{col.ColumnName} ");
                switch (col.Type)
                {
                    case SqlColumnType.Integer:
                        sb.Append("int");
                        break;

                    case SqlColumnType.String:
                        sb.Append("varchar(50)");
                        break;

                    case SqlColumnType.Date:
                        sb.Append(ProviderSpecific.DateType);
                        break;
                }
                if (col.ColumnName == _primaryKeyColumn)
                    sb.Append(" PRIMARY KEY");

                if (i != _columns.Count - 1)
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