using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class WhereList : List<Where>
    {
        public void BuildCommand(StringBuilder sb, ConnectorBase connection, Query qry)
        {
            bool bFirst = true;
            foreach (Where where in this)
            {
                where.BuildCommand(sb, bFirst, connection, qry);
                if (bFirst) bFirst = false;
            }
        }

        public WhereList AND(string columnName, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }
        public WhereList AND(string columnName, WhereComparision comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }
        public WhereList AND(WhereList whereList)
        {
            this.Add(new Where(WhereCondition.AND, whereList));
            return this;
        }
        public WhereList AND(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
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
        public WhereList OR(string columnName, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }
        public WhereList OR(string columnName, WhereComparision comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }
        public WhereList OR(WhereList whereList)
        {
            this.Add(new Where(WhereCondition.OR, whereList));
            return this;
        }
        public WhereList OR(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, columnValue));
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
