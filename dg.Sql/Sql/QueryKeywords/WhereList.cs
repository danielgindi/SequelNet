using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class WhereList : List<Where>
    {
        public void BuildCommand(StringBuilder OutputBuilder, ConnectorBase Connection, Query RelatedQuery)
        {
            bool bFirst = true;
            bool isForJoinList = this is JoinColumnPair;
            foreach (Where where in this)
            {
                where.BuildCommand(OutputBuilder, bFirst, Connection, RelatedQuery, null, null);
                if (bFirst) bFirst = false;
            }
        }

        public void BuildCommand(StringBuilder OutputBuilder, Query RelatedQuery)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                bool bFirst = true;
                bool isForJoinList = this is JoinColumnPair;
                foreach (Where where in this)
                {
                    where.BuildCommand(OutputBuilder, bFirst, conn, RelatedQuery, null, null);
                    if (bFirst) bFirst = false;
                }
            }
        }

        public void BuildCommand(StringBuilder OutputBuilder, ConnectorBase Connection, Query RelatedQuery, TableSchema RightTableSchema, string RightTableName)
        {
            bool bFirst = true;
            bool isForJoinList = this is JoinColumnPair;
            foreach (Where where in this)
            {
                where.BuildCommand(OutputBuilder, bFirst, Connection, RelatedQuery, RightTableSchema, RightTableName);
                if (bFirst) bFirst = false;
            }
        }

        public void BuildCommand(StringBuilder OutputBuilder, Query RelatedQuery, TableSchema RightTableSchema, string RightTableName)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                bool bFirst = true;
                bool isForJoinList = this is JoinColumnPair;
                foreach (Where where in this)
                {
                    where.BuildCommand(OutputBuilder, bFirst, conn, RelatedQuery, RightTableSchema, RightTableName);
                    if (bFirst) bFirst = false;
                }
            }
        }

        public WhereList Where(string ColumnName, object ColumnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, Comparison, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(WhereList WhereList)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, WhereList));
            return this;
        }

        public WhereList Where(string TableName, string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, TableName, ColumnName, Comparison, ColumnValue));
            return this;
        }

        public WhereList Where(string TableName, string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            where.FirstTableName = TableName;
            this.Clear();
            this.Add(where);
            return this;
        }

        public WhereList Where(string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            this.Clear();
            this.Add(where);
            return this;
        }

        public WhereList AND(string ColumnName, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList AND(string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, Comparison, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList AND(WhereList WhereList)
        {
            this.Add(new Where(WhereCondition.AND, WhereList));
            return this;
        }

        public WhereList AND(string TableName, string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.AND, TableName, ColumnName, Comparison, ColumnValue));
            return this;
        }

        public WhereList AND(string TableName, string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            where.FirstTableName = TableName;
            this.Add(where);
            return this;
        }

        public WhereList AND(string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            this.Add(where);
            return this;
        }

        public WhereList OR(string ColumnName, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList OR(string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, Comparison, ColumnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList OR(WhereList WhereList)
        {
            this.Add(new Where(WhereCondition.OR, WhereList));
            return this;
        }

        public WhereList OR(string TableName, string ColumnName, WhereComparision Comparison, object ColumnValue)
        {
            this.Add(new Where(WhereCondition.OR, TableName, ColumnName, Comparison, ColumnValue));
            return this;
        }

        public WhereList OR(string TableName, string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            where.FirstTableName = TableName;
            this.Add(where);
            return this;
        }

        public WhereList OR(string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            this.Add(where);
            return this;
        }
    }
}
