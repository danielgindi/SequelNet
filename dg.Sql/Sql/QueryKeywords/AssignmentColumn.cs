using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class AssignmentColumnList : List<AssignmentColumn> { }
    internal class AssignmentColumn
    {
        private string _TableName = null;
        private string _ColumnName = null;
        private string _SecondTableName = null;
        private object _Second = null;
        private ValueObjectType _SecondType = ValueObjectType.Literal;

        public AssignmentColumn(string tableName, string columnName,  
            string secondTableName, object second, ValueObjectType secondType)
        {
            _TableName = tableName;
            _ColumnName = columnName;
            _SecondTableName = secondTableName;
            _Second = second;
            _SecondType = secondType;
        }

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }
        public object Second
        {
            get { return _Second; }
            set { _Second = value; }
        }
        public ValueObjectType SecondType
        {
            get { return _SecondType; }
            set { _SecondType = value; }
        }
        public string SecondTableName
        {
            get { return _SecondTableName; }
            set { _SecondTableName = value; }
        }
    }
}
