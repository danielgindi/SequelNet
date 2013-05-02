using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class GroupByList : List<GroupBy> { }
    internal class GroupBy
    {
        private string _TableName;
        private object _ColumnName;
        internal bool _IsLiteral = false;

        public GroupBy(object columnName)
        {
            _ColumnName = columnName;
        }
        public GroupBy(string tableName, string columnName)
        {
            _TableName = tableName;
            _ColumnName = columnName;
        }
        public GroupBy(object columnName, bool IsLiteral)
        {
            _TableName = null;
            _ColumnName = columnName;
            _IsLiteral = IsLiteral;
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
        internal bool IsLiteral
        {
            get { return _IsLiteral; }
            set { _IsLiteral = value; }
        }
    }
}
