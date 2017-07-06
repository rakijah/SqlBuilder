﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class BuiltSqlSort
    {
        private BuiltSelectCommand _parent;
        private List<string> _sortingParametersAscending;
        private List<string> _sortingParametersDescending;

        internal BuiltSqlSort(BuiltSelectCommand parent)
        {
            _parent = parent;
            _sortingParametersAscending = new List<string>();
            _sortingParametersDescending = new List<string>();
        }

        /// <summary>
        /// Add a column to be sorted.
        /// </summary>
        /// <param name="table">The table that the column resides in.</param>
        /// <param name="column">The column to be used for sorting.</param>
        /// <param name="mode">The sorting mode to be used for this column.</param>
        /// <returns></returns>
        public BuiltSqlSort SortBy(string table, string column, SqlSortMode mode)
        {
            switch(mode)
            {
                case SqlSortMode.ASCENDING:
                    _sortingParametersAscending.Add($"{table}.{column}");
                    break;
                case SqlSortMode.DESCENDING:
                    _sortingParametersDescending.Add($"{table}.{column}");
                    break;
            }
            
            return this;
        }

        /// <summary>
        /// Finishes building this sort and returns the parent BuiltSelectCommand.
        /// </summary>
        /// <returns></returns>
        public BuiltSelectCommand Finish()
        {
            return _parent;
        }

        /// <summary>
        /// Generates the ORDER BY clause created by this BuiltSqlSort.
        /// </summary>
        public override string ToString()
        {
            if(_sortingParametersAscending.Count == 0 && _sortingParametersDescending.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder("ORDER BY ");
            if (_sortingParametersAscending.Count > 0)
            {
                sb.Append(_sortingParametersAscending.Zip(", "));
                sb.Append($" ASC, ");
            }

            if (_sortingParametersDescending.Count > 0)
            {
                sb.Append(_sortingParametersDescending.Zip(", "));
                sb.Append($" DESC");
            }
            return sb.ToString();
        }
    }

    public enum SqlSortMode
    {
        ASCENDING,
        DESCENDING,
        NONE
    }
}
