using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public class OrderByList : List<OrderBy>
    {
        public OrderByList Then(string columnName, SortDirection sortDirection)
        {
            Add(new OrderBy(columnName, sortDirection));
            return this;
        }

        public OrderByList Then(object columnName, SortDirection sortDirection, bool isLiteral)
        {
            Add(new OrderBy(columnName, sortDirection, isLiteral));
            return this;
        }

        public OrderByList Then(string tableName, string columnName, SortDirection sortDirection)
        {
            Add(new OrderBy(columnName, columnName, sortDirection));
            return this;
        }

        public void BuildCommand(StringBuilder outputBuilder, ConnectorBase conn, Query relatedQuery, bool invert = false)
        {
            if (this.Count > 0)
            {
                outputBuilder.Append(@" ORDER BY ");
                bool bFirst = true;

                foreach (OrderBy orderBy in this)
                {
                    if (bFirst) bFirst = false;
                    else outputBuilder.Append(',');

                    orderBy.BuildCommand(outputBuilder, conn, relatedQuery, invert);
                }
            }
        }
    }

    public class OrderBy
    {
        public string TableName;
        public object ColumnName;
        public SortDirection SortDirection;
        internal bool Randomize = false;
        internal bool IsLiteral = false;

        #region Constructors

        public OrderBy(string columnName, SortDirection sortDirection)
        {
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
        }

        public OrderBy(object columnName, SortDirection sortDirection, bool isLiteral)
        {
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
            this.IsLiteral = isLiteral;
        }

        public OrderBy(string tableName, string columnName, SortDirection sortDirection)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.SortDirection = sortDirection;
        }

        #endregion

        #region Chainable

        public OrderByList Then(string columnName, SortDirection sortDirection)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(columnName, sortDirection));
            return list;
        }

        public OrderByList Then(object columnName, SortDirection sortDirection, bool isLiteral)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(columnName, sortDirection, isLiteral));
            return list;
        }

        public OrderByList Then(string tableName, string columnName, SortDirection sortDirection)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(columnName, columnName, sortDirection));
            return list;
        }

        #endregion
        
        public void BuildCommand(StringBuilder outputBuilder, ConnectorBase conn, Query relatedQuery, bool invert = false)
        {
            if (this.Randomize)
            {
                switch (conn.TYPE)
                {
                    case ConnectorBase.SqlServiceType.MSSQL:
                        outputBuilder.Append(@"NEWID()");
                        break;

                    case ConnectorBase.SqlServiceType.POSTGRESQL:
                        outputBuilder.Append(@"RANDOM()");
                        break;

                    default:
                    case ConnectorBase.SqlServiceType.MYSQL:
                        outputBuilder.Append(@"RAND()");
                        break;

                    case ConnectorBase.SqlServiceType.MSACCESS:
                        if (this.TableName != null) outputBuilder.Append(@"RND(" + conn.WrapFieldName(this.TableName) + @"." + conn.WrapFieldName(this.ColumnName.ToString()) + @")");
                        else outputBuilder.Append(@"RND(" + conn.WrapFieldName(this.ColumnName.ToString()) + @")");
                        break;
                }
            }
            else
            {
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS && relatedQuery != null && relatedQuery.Schema != null && !this.IsLiteral)
                {
                    TableSchema.Column col = relatedQuery.Schema.Columns.Find(this.ColumnName.ToString());
                    if (col != null && col.ActualDataType == DataType.Boolean) outputBuilder.Append(@" NOT ");
                }

                if (this.ColumnName is dg.Sql.IPhrase)
                {
                    outputBuilder.Append(((dg.Sql.IPhrase)this.ColumnName).BuildPhrase(conn, relatedQuery));
                }
                else if (this.ColumnName is dg.Sql.Where)
                {
                    ((dg.Sql.Where)this.ColumnName).BuildCommand(outputBuilder, true, conn, relatedQuery);
                }
                else if (this.IsLiteral)
                {
                    outputBuilder.Append(this.ColumnName);
                }
                else
                {
                    if (this.TableName != null)
                    {
                        outputBuilder.Append(conn.WrapFieldName(this.TableName) + @"." + conn.WrapFieldName(this.ColumnName.ToString()));
                    }
                    else
                    {
                        outputBuilder.Append(conn.WrapFieldName(this.ColumnName.ToString()));
                    }
                }

                switch (this.SortDirection)
                {
                    default:
                    case SortDirection.None:
                        break;

                    case SortDirection.ASC:
                        outputBuilder.Append(invert ? @" DESC" : @" ASC");
                        break;

                    case SortDirection.DESC:
                        outputBuilder.Append(invert ? @" ASC" : @" DESC");
                        break;
                }
            }
        }
    }
}
