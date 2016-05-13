using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class SelectColumnList : List<SelectColumn> { }
    internal class SelectColumn
    {
        private string _ColumnName;
        private string _TableName;
        private string _Alias;
        private ValueObjectType _ObjectType = ValueObjectType.ColumnName;
        private object _Value;

        public SelectColumn(string columnName, bool literal)
        {
            ColumnName = columnName;
            ObjectType = literal ? ValueObjectType.Literal : ValueObjectType.ColumnName;
        }

        public SelectColumn(string columnName, string alias, bool literal)
        {
            ColumnName = columnName;
            Alias = alias;
            ObjectType = literal ? ValueObjectType.Literal : ValueObjectType.ColumnName;
        }

        public SelectColumn(string columnName, string alias)
        {
            ColumnName = columnName;
            Alias = alias;
        }

        public SelectColumn(string tableName, string columnName, string alias)
        {
            TableName = tableName;
            ColumnName = columnName;
            Alias = alias;
        }

        public SelectColumn(object value, string alias)
        {
            Value = value;
            Alias = alias;
            ObjectType = ValueObjectType.Value;
        }

        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }

        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        public ValueObjectType ObjectType
        {
            get { return _ObjectType; }
            set { _ObjectType = value; }
        }

        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }
}
