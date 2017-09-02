using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    /// <summary>
    /// The WHERE clause of a SQL statement.
    /// </summary>
    public class BuiltSqlCondition
    {
        private List<ConditionExpression> _conditionExpressions { get; }
        private int _currentBlockLayer;
        private bool _lastComponentWasLogicExpression = true;

        public BuiltSqlCondition()
        {
            _conditionExpressions = new List<ConditionExpression>();
        }

        /// <summary>
        /// Adds a condition to the WHERE clause that compares two columns.
        /// </summary>
        /// <typeparam name="TFirst">The first (lefthand) table of the comparison.</typeparam>
        /// <typeparam name="TSecond">The second (righthand) table of the comparison.</typeparam>
        /// <param name="firstColumn">The column of the lefthand table to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="secondColumn">The column of the righthand table to be compared.</param>
        public BuiltSqlCondition AddCondition<TFirst, TSecond>(string firstColumn, string comparisonOperator, string secondColumn)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;

            _conditionExpressions.Add(new ConditionExpression(typeof(TFirst), firstColumn, comparisonOperator, typeof(TSecond), secondColumn));
            return this;
        }

        /// <summary>
        /// Adds a condition to the WHERE clause that compares two columns.
        /// </summary>
        /// <param name="first">The first (lefthand) table of the comparison.</param>
        /// <param name="second">The second (righthand) table of the comparison.</param>
        /// <param name="firstColumn">The column of the lefthand table to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="secondColumn">The column of the righthand table to be compared.</param>
        public BuiltSqlCondition AddCondition(Type first, Type second, string firstColumn, string comparisonOperator, string secondColumn)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;

            _conditionExpressions.Add(new ConditionExpression(first, firstColumn, comparisonOperator, second, secondColumn));
            return this;
        }

        /// <summary>
        /// Adds an "equals" condition to the WHERE clause that compares a column to a static value.
        /// </summary>
        /// <typeparam name="Table">The table to be used in the comparison.</typeparam>
        /// <param name="column">The column to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="value">The value to be compared against.</param>
        public BuiltSqlCondition AddCondition<Table>(string column, string comparisonOperator, string value, DbType valueType)
        {
            return AddCondition(typeof(Table), column, comparisonOperator, value, valueType);
        }

        /// <summary>
        /// Adds an "equals" condition to the WHERE clause that compares a column to a static value.
        /// </summary>
        /// <param name="tableType">The table to be used in the comparison.</param>
        /// <param name="column">The column to be compared.</param>
        /// <param name="comparisonOperator">The sorting operator to be used. For example: '=', '&lt;&gt;' '&gt;' etc.</param>
        /// <param name="value">The value to be compared against.</param>
        public BuiltSqlCondition AddCondition(Type tableType, string column, string comparisonOperator, string value, DbType valueType)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;
            
            _conditionExpressions.Add(new ConditionExpression(tableType, column, comparisonOperator, value, valueType));
            return this;
        }

        /// <summary>
        /// Adds an IS NULL comparison to the WHERE clause.
        /// </summary>
        /// <typeparam name="Table">The table to be used in the comparison.</typeparam>
        /// <param name="column">The column to be compared.</param>
        public BuiltSqlCondition IsNull<Table>(string column)
        {
            if (!_lastComponentWasLogicExpression)
                throw new ArgumentException("Can't add another condition without a logic expression in between.");
            _lastComponentWasLogicExpression = false;

            _conditionExpressions.Add(new ConditionExpression(typeof(Table), column));
            return this;
        }

        /// <summary>
        /// Starts a block of conditions.
        /// </summary>
        public BuiltSqlCondition BeginBlock()
        {
            //A block must be preceded by a logic expression but can't preceed another logic expression
            //This is fine:
            //... AND (TBL.COL=...)
            //This is not:
            //... OR (AND TBL.COL=...)
            if (!_lastComponentWasLogicExpression)
                throw new Exception("Can't begin block right after a condition without a logic expressin in between.");
            _lastComponentWasLogicExpression = true;

            _conditionExpressions.Add(new ConditionExpression(BlockOrLogicOperator.OpenParenthesis));
            _currentBlockLayer++;
            return this;
        }

        /// <summary>
        /// Ends a block of conditions.
        /// </summary>
        public BuiltSqlCondition EndBlock()
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
            _conditionExpressions.Add(new ConditionExpression(BlockOrLogicOperator.ClosedParenthesis));

            return this;
        }

        /// <summary>
        /// Adds an AND between the previous and the next condition.
        /// </summary>
        public BuiltSqlCondition And()
        {
            if (_lastComponentWasLogicExpression)
                throw new Exception("Can't add two logic expressions right behind each other.");
            _conditionExpressions.Add(new ConditionExpression(BlockOrLogicOperator.And));
            _lastComponentWasLogicExpression = true;
            return this;
        }

        /// <summary>
        /// Adds an OR between the previous and the next condition.
        /// </summary>
        public BuiltSqlCondition Or()
        {
            if (_lastComponentWasLogicExpression)
                throw new Exception("Can't add two logic expressions right behind each other.");
            _conditionExpressions.Add(new ConditionExpression(BlockOrLogicOperator.Or));
            _lastComponentWasLogicExpression = true;
            return this;
        }

        /// <summary>
        /// Generates the condition string, always starting with "WHERE" ending either with an arbitrary condition or a ")".
        /// </summary>
        public string GenerateStatement()
        {
            if (_conditionExpressions.Count == 0)
                throw new Exception("A WHERE clause can't contain 0 expressions.");

            if (_lastComponentWasLogicExpression)
                throw new Exception("A WHERE clause can't end with a logic expression.");

            StringBuilder sb = new StringBuilder("WHERE ");
            for (int i = 0; i < _conditionExpressions.Count; i++)
            {
                string c = _conditionExpressions[i].ToString();
                sb.Append(c);
                if (i == _conditionExpressions.Count - 1 || c == "(") continue;
                if (i > _conditionExpressions.Count - 2 || _conditionExpressions[i + 1].BlockOrLogicOperator != BlockOrLogicOperator.ClosedParenthesis)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Appends the condition to the provided DbCommand using DbParameters.
        /// </summary>
        public void GenerateCommand(DbCommand command)
        {
            if (_conditionExpressions.Count == 0)
                throw new Exception("A WHERE clause can't contain 0 expressions.");

            if (_lastComponentWasLogicExpression)
                throw new Exception("A WHERE clause can't end with a logic expression.");
            
            command.CommandText += " WHERE ";
            for (int i = 0; i < _conditionExpressions.Count; i++)
            {
                _conditionExpressions[i].GenerateCommand(command);
                if (i != _conditionExpressions.Count - 1)
                    command.CommandText += " ";
            }
        }

        public override string ToString()
        {
            return GenerateStatement();
        }
    }
}