using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class ConditionExpression
    {
        public ConditionExpressionType Type { get; }
        public Type LeftTable { get; }
        public string LeftTableName { get; }
        public string LeftColumn { get; }

        public Type RightTable { get; }
        public string RightTableName { get; }
        public string RightColumn { get; }

        public string ComparisonOperator { get; }
        public string Value { get; }
        public DbType ValueType { get; }

        public BlockOrLogicOperator BlockOrLogicOperator { get; }


        public ConditionExpression(Type leftTable, string leftColumn, string comparisonOperator, Type rightTable, string rightColumn)
        {
            Type = ConditionExpressionType.CompareToColumn;
            LeftTable = leftTable;
            LeftTableName = SqlTableHelper.GetTableName(leftTable);
            LeftColumn = leftColumn;
            ComparisonOperator = comparisonOperator;
            RightTable = rightTable;
            RightTableName = SqlTableHelper.GetTableName(rightTable);
            RightColumn = rightColumn;
        }

        public ConditionExpression(Type leftTable, string leftColumn, string comparisonOperator, string value, DbType valueType)
        {
            Type = ConditionExpressionType.CompareToValue;
            LeftTable = leftTable;
            LeftTableName = SqlTableHelper.GetTableName(leftTable);
            LeftColumn = leftColumn;
            ComparisonOperator = comparisonOperator;
            Value = value;
            ValueType = valueType;
        }

        /// <summary>
        /// Checks if the provided column is NULL.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        public ConditionExpression(Type table, string column)
        {
            Type = ConditionExpressionType.IsNull;
            LeftTable = table;
            LeftTableName = SqlTableHelper.GetTableName(table);
            LeftColumn = column;
        }

        public ConditionExpression(BlockOrLogicOperator blockOrLogicOperator)
        {
            Type = ConditionExpressionType.BlockOrLogicOperator;
            BlockOrLogicOperator = blockOrLogicOperator;
        }

        public string GenerateStatement()
        {
            StringBuilder sb = new StringBuilder();
            switch (Type)
            {
                case ConditionExpressionType.CompareToColumn:
                    return $"{Util.FormatSQL(LeftTableName, LeftColumn)}{ComparisonOperator}{Util.FormatSQL(RightTableName, RightColumn)}";
                case ConditionExpressionType.CompareToValue:
                    return $"{Util.FormatSQL(LeftTableName, LeftColumn)}{ComparisonOperator}{Value}";
                case ConditionExpressionType.IsNull:
                    return $"{Util.FormatSQL(LeftTableName, LeftColumn)} IS NULL";
                case ConditionExpressionType.BlockOrLogicOperator:
                    return BlockOrLogicOperator.OperatorToString();
                default:
                    return "";
            }
        }

        public void GenerateCommand(DbCommand command)
        {
            switch (Type)
            {
                case ConditionExpressionType.CompareToColumn:
                    command.CommandText += $"{Util.FormatSQL(LeftTableName, LeftColumn)}{ComparisonOperator}{Util.FormatSQL(RightTableName, RightColumn)}";
                    break;
                case ConditionExpressionType.CompareToValue:
                    string paramName = Util.GetUniqueParameterName();
                    var param = command.CreateParameter();
                    param.ParameterName = paramName;
                    param.Value = Value;
                    param.DbType = ValueType;
                    command.Parameters.Add(param);
                    command.CommandText +=
                        $"{Util.FormatSQL(LeftTableName, LeftColumn)}{ComparisonOperator}{ProviderSpecific.ParameterPrefix}{paramName}";
                    break;
                case ConditionExpressionType.IsNull:
                    command.CommandText += $"{Util.FormatSQL(LeftTableName, LeftColumn)} IS NULL";
                    break;
                case ConditionExpressionType.BlockOrLogicOperator:
                    command.CommandText += BlockOrLogicOperator.OperatorToString();
                    break;
            }
        }

        public override string ToString()
        {
            return GenerateStatement();
        }
    }

    public enum ConditionExpressionType
    {
        CompareToColumn,
        CompareToValue,
        IsNull,
        BlockOrLogicOperator
    }

    public enum BlockOrLogicOperator
    {
        NotSet,
        OpenParenthesis,
        ClosedParenthesis,
        And,
        Or
    }

    internal static class BlockOrLogicOperatorExtensions
    {
        public static string OperatorToString(this BlockOrLogicOperator oper)
        {
            switch (oper)
            {
                case BlockOrLogicOperator.OpenParenthesis:
                    return "(";
                case BlockOrLogicOperator.ClosedParenthesis:
                    return ")";
                case BlockOrLogicOperator.And:
                    return "AND";
                case BlockOrLogicOperator.Or:
                    return "OR";
                default:
                    return "UNKNOWNOPERATOR";
            }
        }
    }
}
