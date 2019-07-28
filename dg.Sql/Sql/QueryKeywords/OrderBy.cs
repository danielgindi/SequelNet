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
        public ValueWrapper Value = new ValueWrapper();

        public SortDirection SortDirection;
        internal bool Randomize = false;

        #region Constructors

        public OrderBy(string columnName, SortDirection sortDirection)
        {
            Value.Value = columnName;
            Value.Type = ValueObjectType.ColumnName;
            this.SortDirection = sortDirection;
        }

        public OrderBy(string tableName, string columnName, SortDirection sortDirection)
        {
            Value.TableName = tableName;
            Value.Value = columnName;
            Value.Type = ValueObjectType.ColumnName;
            this.SortDirection = sortDirection;
        }

        public OrderBy(object value, SortDirection sortDirection, bool isLiteral)
        {
            Value.Value = value;
            Value.Type = isLiteral ? ValueObjectType.Literal : ValueObjectType.Value;
            this.SortDirection = sortDirection;
        }

        public OrderBy(object value, ValueObjectType valueType, SortDirection sortDirection)
        {
            Value.Value = value;
            Value.Type = valueType;
            this.SortDirection = sortDirection;
        }

        public OrderBy(IPhrase phrase, SortDirection sortDirection)
        {
            Value.Value = phrase;
            Value.Type = ValueObjectType.Value;
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

        public OrderByList Then(object value, SortDirection sortDirection, bool isLiteral)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(value, sortDirection, isLiteral));
            return list;
        }

        public OrderByList Then(string tableName, string columnName, SortDirection sortDirection)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(columnName, columnName, sortDirection));
            return list;
        }

        public OrderByList Then(object value, ValueObjectType valueType, SortDirection sortDirection)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(value, valueType, sortDirection));
            return list;
        }

        public OrderByList Then(IPhrase phrase, SortDirection sortDirection)
        {
            var list = new OrderByList();
            list.Add(this);
            list.Add(new OrderBy(phrase, sortDirection));
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
                        if (Value != null)
                        {
                            outputBuilder.Append(@"RND(" + Value.Build(conn) + @")");
                        }
                        else
                        {
                            outputBuilder.Append(@"RND(NULL)");
                        }
                        break;
                }
            }
            else
            {
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS && relatedQuery != null && relatedQuery.Schema != null && Value.Type != ValueObjectType.Literal)
                {
                    TableSchema.Column col = relatedQuery.Schema.Columns.Find(Value.Value.ToString());
                    if (col != null && col.ActualDataType == DataType.Boolean) outputBuilder.Append(@" NOT ");
                }

                Value.Build(outputBuilder, conn);

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
