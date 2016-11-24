using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class SelectColumnList : List<SelectColumn> { }
    internal class SelectColumn
    {
        public string TableName;
        public object Value;
        public ValueObjectType ObjectType = ValueObjectType.ColumnName;
        public string Alias;

        public SelectColumn(string columnName, bool literal)
        {
            Value = columnName;
            ObjectType = literal ? ValueObjectType.Literal : ValueObjectType.ColumnName;
        }

        public SelectColumn(string columnName, string alias, bool literal)
        {
            Value = columnName;
            Alias = alias;
            ObjectType = literal ? ValueObjectType.Literal : ValueObjectType.ColumnName;
        }

        public SelectColumn(string columnName, string alias)
        {
            Value = columnName;
            Alias = alias;
        }

        public SelectColumn(string tableName, string columnName, string alias)
        {
            TableName = tableName;
            Value = columnName;
            Alias = alias;
        }

        public SelectColumn(object value, string alias)
        {
            Value = value;
            Alias = alias;
            ObjectType = ValueObjectType.Value;
        }

        public SelectColumn(IPhrase phrase, string alias = null)
        {
            Value = phrase;
            Alias = alias;
            ObjectType = ValueObjectType.Value;
        }

        public SelectColumn(Query query, string alias = null)
        {
            Value = query;
            Alias = alias;
            ObjectType = ValueObjectType.Value;
        }
    }
}
