using SequelNet.Connector;
using System.Collections.Generic;
using System.Text;

namespace SequelNet
{
    public class AssignmentColumnList : List<AssignmentColumn> { }
    public class AssignmentColumn
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

        public void BuildSecond(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            if (SecondType == ValueObjectType.Literal)
            {
                sb.Append(Second);
            }
            else if (SecondType == ValueObjectType.Value)
            {
                if (Second is Query)
                {
                    sb.Append('(');
                    sb.Append(((Query)Second).BuildCommand(conn));
                    sb.Append(')');
                }
                else
                {
                    Query.PrepareColumnValue(relatedQuery.Schema.Columns.Find(ColumnName), Second, sb, conn, relatedQuery);
                }
            }
            else if (SecondType == ValueObjectType.ColumnName)
            {
                if (SecondTableName != null)
                {
                    sb.Append(conn.Language.WrapFieldName(SecondTableName));
                    sb.Append(@".");
                }
                sb.Append(conn.Language.WrapFieldName(Second.ToString()));
            }
        }
    }
}
