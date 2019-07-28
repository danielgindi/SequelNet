using System.Collections.Generic;

namespace SequelNet
{
    internal class GroupByList : List<GroupBy> { }
    internal class GroupBy
    {
        private string _TableName;
        private object _ColumnName;
        private SortDirection _SortDirection;
        internal bool _IsLiteral = false;

        public GroupBy(object ColumnName)
        {
            _ColumnName = ColumnName;
        }

        public GroupBy(object ColumnName, SortDirection SortDirection)
        {
            _ColumnName = ColumnName;
            _SortDirection = SortDirection;
        }

        public GroupBy(string TableName, string ColumnName)
        {
            _TableName = TableName;
            _ColumnName = ColumnName;
        }

        public GroupBy(string TableName, string ColumnName, SortDirection SortDirection)
        {
            _TableName = TableName;
            _ColumnName = ColumnName;
            _SortDirection = SortDirection;
        }

        public GroupBy(object ColumnName, bool IsLiteral)
        {
            _TableName = null;
            _ColumnName = ColumnName;
            _IsLiteral = IsLiteral;
        }

        public GroupBy(object ColumnName, bool IsLiteral, SortDirection SortDirection)
        {
            _TableName = null;
            _ColumnName = ColumnName;
            _IsLiteral = IsLiteral;
            _SortDirection = SortDirection;
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

        internal bool IsLiteral
        {
            get { return _IsLiteral; }
            set { _IsLiteral = value; }
        }
    }
}
