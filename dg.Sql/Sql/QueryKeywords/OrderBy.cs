using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class OrderByList : List<OrderBy> { }
    internal class OrderBy
    {
        private string _TableName;
        private object _ColumnName;
        private SortDirection _SortDirection;
        internal bool _Randomize = false;
        internal bool _IsLiteral = false;

        public OrderBy(string columnName, SortDirection sortDirection)
        {
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
        }

        public OrderBy(object columnName, SortDirection sortDirection, bool IsLiteral)
        {
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
            this.IsLiteral = IsLiteral;
        }

        public OrderBy(string tableName, string columnName, SortDirection sortDirection)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
        }

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }

        public object ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        public SortDirection SortDirection
        {
            get { return _SortDirection; }
            set { _SortDirection = value; }
        }

        internal bool Randomize
        {
            get { return _Randomize; }
            set { _Randomize = value; }
        }

        internal bool IsLiteral
        {
            get { return _IsLiteral; }
            set { _IsLiteral = value; }
        }
    }
}
