using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    public class BuiltSqlCondition<T>
    {
        private List<string> _conditionExpressions { get; }
        private readonly T _parent;
        private int _currentBlockLayer;
        private bool _lastComponentWasLogicExpression = true;

        internal BuiltSqlCondition(T parent)
        {
            _conditionExpressions = new List<string>();
            _parent = parent;
        }

        /// <summary>
        /// Adds a condition to the WHERE clause that compares two columns.
        /// </summary>
        /// <typeparam name="TFirst">The first (lefthand) table of the comparison.</typeparam>
        /// <typeparam name="TSecond">The second (righthand) table of the comparison.</typeparam>
        /// <param name="firstColumn">The column of the lefthand table to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="secondColumn">The column of the righthand table to be compared.</param>
        public BuiltSqlCondition<T> AddCondition<TFirst, TSecond>(string firstColumn, string comparisonOperator, string secondColumn)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;

            _conditionExpressions.Add($"{Util.FormatSQL(SqlTable.GetTableName<TFirst>(), firstColumn)}{comparisonOperator}{Util.FormatSQL(SqlTable.GetTableName<TSecond>(), secondColumn)}");
            return this;
        }

        /// <summary>
        /// Adds an "equals" condition to the WHERE clause that compares a column to a static value.
        /// </summary>
        /// <typeparam name="Table">The table to be used in the comparison.</typeparam>
        /// <param name="column">The column to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="value">The value to be compared against.</param>
        /// <param name="putAroundValue">Optional character that encloses the value if != \0</param>
        public BuiltSqlCondition<T> AddCondition<Table>(string column, string comparisonOperator, string value, char putAroundValue = '\0')
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;
            string condition = $"{Util.FormatSQL(SqlTable.GetTableName<Table>(), column)}{comparisonOperator}";
            if (putAroundValue == '\0')
                condition += value;
            else
                condition += $"{putAroundValue}{value}{putAroundValue}";
            
            _conditionExpressions.Add(condition);
            return this;
        }

        /// <summary>
        /// !! UNSAFE !! Adds a directly specified condition to the clause.
        /// This is automatically considered to be a non-logic expression.
        /// </summary>
        /// <param name="condition">The condition to be added (example: SubStr(column, 4)="test"</param>
        public BuiltSqlCondition<T> AddConditionDirect(string condition)
        {
            _lastComponentWasLogicExpression = false;
            _conditionExpressions.Add(condition.Trim());
            return this;
        }

        /// <summary>
        /// Adds an IS NULL comparison to the WHERE clause.
        /// </summary>
        /// <typeparam name="Table">The table to be used in the comparison.</typeparam>
        /// <param name="column">The column to be compared.</param>
        public BuiltSqlCondition<T> IsNull<Table>(string column)
        {
            if(!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;
            _conditionExpressions.Add($"{Util.FormatSQL(SqlTable.GetTableName<Table>(), column)} IS NULL");
            return this;
        }

        /// <summary>
        /// Starts a block of conditions.
        /// </summary>
        public BuiltSqlCondition<T> BeginBlock()
        {
            //A block must be preceded by a logic expression but can't preceed another logic expression
            //This is fine:
            //... AND (TBL.COL=...)
            //This is not:
            //... OR (AND TBL.COL=...)
            if (!_lastComponentWasLogicExpression)
                throw new Exception("Can't begin block right after a condition without a logic expressin in between.");
            _lastComponentWasLogicExpression = true;

            _conditionExpressions.Add("(");
            _currentBlockLayer++;
            return this;
        }

        /// <summary>
        /// Ends a block of conditions.
        /// </summary>
        public BuiltSqlCondition<T> EndBlock()
        {
            if (_currentBlockLayer == 0)
                throw new ArgumentException("Can't end block when not in block.");

            if (_lastComponentWasLogicExpression)
                throw new Exception("A block can't end with a logic expression.");
            //A block can preced a logic expresseion but not a regular condition.
            //This is fine:
            // ...) AND
            //This is not:
            // ...) TBL.COL=...
            _lastComponentWasLogicExpression = false;

            _currentBlockLayer--;
            _conditionExpressions.Add(")");

            return this;
        }

        /// <summary>
        /// Adds an AND between the previous and the next condition.
        /// </summary>
        public BuiltSqlCondition<T> And()
        {
            if (_lastComponentWasLogicExpression)
                throw new Exception("Can't add two logic expressions right behind each other.");
            _conditionExpressions.Add("AND");
            _lastComponentWasLogicExpression = true;
            return this;
        }

        /// <summary>
        /// Adds an OR between the previous and the next condition.
        /// </summary>
        public BuiltSqlCondition<T> Or()
        {
            if (_lastComponentWasLogicExpression)
                throw new Exception("Can't add two logic expressions right behind each other.");
            _conditionExpressions.Add("OR");
            _lastComponentWasLogicExpression = true;
            return this;
        }

        /// <summary>
        /// Finishes building this condition and returns the parent BuiltSelectCommand.
        /// </summary>
        public T Finish()
        {
            if (_currentBlockLayer > 0)
                throw new Exception("Can't finish condition while still in block.");

            return _parent;
        }
        
        /// <summary>
        /// Generates the condition string, always starting with "WHERE" ending either with an arbitrary condition or a ")".
        /// </summary>
        public string Generate()
        {
            if (_conditionExpressions.Count == 0)
                throw new Exception("A WHERE clause can't contain 0 expressions.");

            if (_lastComponentWasLogicExpression)
                throw new Exception("A WHERE clause can't end with a logic expression.");

            StringBuilder sb = new StringBuilder("WHERE ");
            for (int i = 0; i < _conditionExpressions.Count; i++)
            {
                string c = _conditionExpressions[i];
                sb.Append(c);
                if (i == _conditionExpressions.Count - 1 || c == "(") continue;
                if (i > _conditionExpressions.Count - 2 || _conditionExpressions[i + 1] != ")")
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }
}