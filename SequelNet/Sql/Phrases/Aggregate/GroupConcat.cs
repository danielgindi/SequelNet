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
            this.Value = new ValueWrapper(tableName, columnName);
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, string tableName, string columnName, string separator)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper(tableName, columnName);
            this.Separator = separator;
        }

        public GroupConcat(bool distinct, string tableName, string columnName, OrderByList orderBy)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper(tableName, columnName);
            this.OrderBy = orderBy;
        }

        public GroupConcat(bool distinct, string tableName, string columnName)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper(tableName, columnName);
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
            this.Value = new ValueWrapper(tableName, columnName);
            this.Separator = separator;
            this.OrderBy = orderBy;
        }

        public GroupConcat(string tableName, string columnName, string separator)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.Separator = separator;
        }

        public GroupConcat(string tableName, string columnName, OrderByList orderBy)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.OrderBy = orderBy;
        }

        public GroupConcat(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
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

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            StringBuilder sb = new StringBuilder();

            if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                sb.Append("string_agg(");

                if (Distinct)
                {
                    sb.Append("DISTINCT ");
                }

                sb.Append(Value.Build(conn, relatedQuery));

                if (Separator != null)
                {
                    sb.Append("," + conn.Language.PrepareValue(Separator));
                }
                else
                {
                    sb.Append(",','");
                }

                if (OrderBy != null && OrderBy.Count > 0)
                {
                    OrderBy.BuildCommand(sb, conn, relatedQuery, false);
                }

                sb.Append(")");
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
            {
                sb.Append("GROUP_CONCAT(");

                if (Distinct)
                {
                    sb.Append("DISTINCT ");
                }

                sb.Append(Value.Build(conn, relatedQuery));

                if (OrderBy != null && OrderBy.Count > 0)
                {
                    OrderBy.BuildCommand(sb, conn, relatedQuery, false);
                }

                if (Separator != null)
                {
                    sb.Append(" SEPARATOR " + conn.Language.PrepareValue(Separator));
                }

                sb.Append(")");
            }
            else
            {
                throw new NotSupportedException("SHA1 is not supported by " + conn.TYPE.ToString());
            }

            return sb.ToString();
        }
    }
}
