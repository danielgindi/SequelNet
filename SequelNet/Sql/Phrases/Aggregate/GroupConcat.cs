using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class GroupConcat : IPhrase
    {
        public bool Distinct;
        public string Separator;
        public ValueWrapper Value;
        public OrderByList OrderBy;

        #region Constructors
        
        public GroupConcat()
        {
            this.Value = new ValueWrapper();
        }

        public GroupConcat(bool distinct, string tableName, string columnName, string separator, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, string tableName, string columnName, string separator)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator;
        }

        public GroupConcat(bool distinct, string tableName, string columnName, char separator, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator.ToString();
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, string tableName, string columnName, char separator)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator.ToString();
        }

        public GroupConcat(bool distinct, string tableName, string columnName, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, string tableName, string columnName)
        {
            this.Distinct = distinct;
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public GroupConcat(bool distinct, ValueWrapper value, string separator, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, ValueWrapper value, string separator)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.Separator = separator;
        }

        public GroupConcat(bool distinct, ValueWrapper value, char separator, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.Separator = separator.ToString();
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, ValueWrapper value, char separator)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.Separator = separator.ToString();
        }

        public GroupConcat(bool distinct, ValueWrapper value, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, ValueWrapper value)
        {
            this.Distinct = distinct;
            this.Value = value;
        }

        public GroupConcat(string tableName, string columnName, string separator, OrderByList orderBy)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(string tableName, string columnName, string separator)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator;
        }

        public GroupConcat(string tableName, string columnName, char separator, OrderByList orderBy)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator.ToString();
            this.OrderBy = orderBy;
        }

        public GroupConcat(string tableName, string columnName, char separator)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Separator = separator.ToString();
        }

        public GroupConcat(string tableName, string columnName, OrderByList orderBy)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.OrderBy = orderBy;
        }

        public GroupConcat(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public GroupConcat(ValueWrapper value, string separator, OrderByList orderBy)
        {
            this.Value = value;
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(ValueWrapper value, string separator)
        {
            this.Value = value;
            this.Separator = separator;
        }

        public GroupConcat(ValueWrapper value, char separator, OrderByList orderBy)
        {
            this.Value = value;
            this.Separator = separator.ToString();
            this.OrderBy = orderBy;
        }

        public GroupConcat(ValueWrapper value, char separator)
        {
            this.Value = value;
            this.Separator = separator.ToString();
        }

        public GroupConcat(ValueWrapper value, OrderByList orderBy)
        {
            this.Value = value;
            this.OrderBy = orderBy;
        }

        public GroupConcat(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            string rawOrderBy = null;
            if (OrderBy != null && OrderBy.Count > 0)
            {
                var sb2 = new StringBuilder();
                OrderBy.BuildCommand(sb2, conn, relatedQuery, false);
                rawOrderBy = sb2.ToString();
            }

            sb.Append(conn.Language.GroupConcat(Distinct, Value.Build(conn, relatedQuery), rawOrderBy, Separator));
        }
    }
}
