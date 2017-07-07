using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class BuiltSqlCondition<T>
    {
        private List<string> _conditionExpressions { get; }
        private T _parent;
        private int _currentBlockLayer = 0;
        private bool _lastComponentWasLogicExpression = true;

        internal BuiltSqlCondition(T parent)
        {
            _conditionExpressions = new List<string>();
            _parent = parent;
        }

        /// <summary>
        /// Adds a condition to the WHERE clause that compares two columns.
        /// </summary>
        /// <param name="firstTable">The lefthand table used for the comparison.</param>
        /// <param name="firstColumn">The column of the lefthand table to be compared.</param>
        /// <param name="secondTable">The righthhand table used for the comparison.</param>
        /// <param name="secondColumn">The column of the righthand table to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '<>' '>' etc.</param>
        /// <returns></returns>
        public BuiltSqlCondition<T> AddCondition(string firstTable, string firstColumn, string secondTable, string secondColumn, string comparisonOperator)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;

            _conditionExpressions.Add($"{firstTable}.{firstColumn}{comparisonOperator}{secondTable}.{secondColumn}");
            return this;
        }

        /// <summary>
        /// Adds an "equals" condition to the WHERE clause that compares a column to a static value.
        /// </summary>
        /// <param name="table">The table to be used in the comparison.</param>
        /// <param name="column">The column to be compared.</param>
        /// <param name="value">The value to be compared against.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '<>' '>' etc.</param>
        /// <param name="putAroundValue">Optional character that encloses the value if != \0</param>
        public BuiltSqlCondition<T> AddCondition(string table, string column, string value, string comparisonOperator, char putAroundValue = '\0')
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;
            string condition = $"{table}.{column}{comparisonOperator}";
            if (putAroundValue == '\0')
                condition += value;
            else
                condition += $"{putAroundValue}{value}{putAroundValue}";
            
            _conditionExpressions.Add(condition);
            return this;
        }

        /// <summary>
        /// Adds an IS NULL comparison to the WHERE clause.
        /// </summary>
        /// <param name="table">The table to be used in the comparison.</param>
        /// <param name="column">The column to be compared.</param>
        /// <returns></returns>
        public BuiltSqlCondition<T> AddCondition(string table, string column)
        {
            if(!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;
            _conditionExpressions.Add($"{table}.{column} IS NULL");
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
                if (i != _conditionExpressions.Count - 1 &&
                   c != "(")
                {
                    if (i > _conditionExpressions.Count - 2 || _conditionExpressions[i + 1] != ")")
                    {
                        sb.Append(" ");
                    }
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