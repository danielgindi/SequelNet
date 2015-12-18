using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class WhereList : List<Where>
    {
        public void BuildCommand(StringBuilder outputBuilder, ConnectorBase conn, Query relatedQuery)
        {
            bool isFirst = true;
            bool isForJoinList = this is JoinColumnPair;
            foreach (Where where in this)
            {
                where.BuildCommand(outputBuilder, isFirst, conn, relatedQuery, null, null);
                if (isFirst) isFirst = false;
            }
        }

        public void BuildCommand(StringBuilder outputBuilder, Query relatedQuery)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                bool isFirst = true;
                bool isForJoinList = this is JoinColumnPair;
                foreach (Where where in this)
                {
                    where.BuildCommand(outputBuilder, isFirst, conn, relatedQuery, null, null);
                    if (isFirst) isFirst = false;
                }
            }
        }

        public void BuildCommand(StringBuilder outputBuilder, ConnectorBase conn, Query relatedQuery, TableSchema rightTableSchema, string rightTableName)
        {
            bool isFirst = true;
            bool isForJoinList = this is JoinColumnPair;
            foreach (Where where in this)
            {
                where.BuildCommand(outputBuilder, isFirst, conn, relatedQuery, rightTableSchema, rightTableName);
                if (isFirst) isFirst = false;
            }
        }

        public void BuildCommand(StringBuilder outputBuilder, Query relatedQuery, TableSchema rightTableSchema, string rightTableName)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                bool isFirst = true;
                bool isForJoinList = this is JoinColumnPair;
                foreach (Where where in this)
                {
                    where.BuildCommand(outputBuilder, isFirst, conn, relatedQuery, rightTableSchema, rightTableName);
                    if (isFirst) isFirst = false;
                }
            }
        }

        public WhereList ClearWhere()
        {
            this.Clear();
            return this;
        }

        public WhereList Where(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
            return this;
        }

        public WhereList Where(string columnName, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(string columnName, WhereComparision comparison, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(string literalExpression)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal));
            return this;
        }

        public WhereList Where(IPhrase phrase)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal));
            return this;
        }

        public WhereList Where(WhereList whereList)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, whereList));
            return this;
        }

        public WhereList Where(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
            return this;
        }

        public WhereList Where(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList Where(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            this.Clear();
            this.Add(where);
            return this;
        }

        public WhereList Where(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            this.Clear();
            this.Add(where);
            return this;
        }

        public WhereList AND(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
            return this;
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

        public WhereList AND(string literalExpression)
        {
            this.Add(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal));
            return this;
        }

        public WhereList AND(IPhrase phrase)
        {
            this.Add(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal));
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

        public WhereList AND(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList AND(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            this.Add(where);
            return this;
        }

        public WhereList AND(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            this.Add(where);
            return this;
        }

        public WhereList OR(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Add(new Where(WhereCondition.OR, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
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

        public WhereList OR(string literalExpression)
        {
            this.Add(new Where(WhereCondition.OR, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal));
            return this;
        }

        public WhereList OR(IPhrase phrase)
        {
            this.Add(new Where(WhereCondition.OR, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal));
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

        public WhereList OR(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList OR(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            this.Add(where);
            return this;
        }

        public WhereList OR(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            this.Add(where);
            return this;
        }
    }
}
